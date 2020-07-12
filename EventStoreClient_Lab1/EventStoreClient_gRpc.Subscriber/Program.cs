using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Grpc.Core;

namespace EventStoreClient_gRpc.Subscriber
{


    /*
     * Note:
     * see the test cases in https://github.com/EventStore/EventStore-Client-Dotnet/tree/master/test/EventStore.Client.PersistentSubscriptions.Tests
     * for implementation examples, as the documentation at the moment is in progress , i.e non-existing (2020-07-12)
     *
     */
    class Program
    {
        static async Task Main(string[] args)
        {


            var streamName = "gRPC-person-stream";
            var groupName = "gRPC-subscription-group";
            var userCredentials = new UserCredentials("admin", "changeit");

            var client = CreateEventStorePersistentSubscriptionsClient(userCredentials);


            var appeared = new TaskCompletionSource<StreamPosition>();
            var dropped = new TaskCompletionSource<bool>();


            /*
             * IMPORTANT note.
             * in tcp client the methods for creating a persistent subscription
             *  is found on the same connection class (EventStoreConnection) as the connection for appending events,
             *
             * her in the gRpc client you can subscribe to a stream
             *  from the same client class as you use to append events
             *  but to create a persistent subscription (a group) you need to use the 'EventStorePersistentSubscriptionsClient'
             *
             * (I was a little confused before digging into the source code and the tests,
             * se further down where I stumbled and only found he non-persistent subscriber..)
             */

            /*
             * do not resolve linktos (todo: add explanation here. reference blog article.
             * start subscription from current position in stream,
             * - meaning only act on events coming in after this subscription is created.
             * - you can start from the beginning, StartFromBeginning, or any given position
             */
            var settings = new PersistentSubscriptionSettings(
                startFrom: StreamPosition.End,
                resolveLinkTos: false
            );


            /*
             * since a subscription might already have been created
             * we need to wrap the creation, catch the exception and "silence" it.
             */
            try
            {
                await client.CreateAsync(
                    streamName,
                    groupName,
                    settings,
                    userCredentials
                );

            }
            /*
             * Note: in the tcp client we get a message on "InvalidOperationException"
             * but here we get a inner exception of type Grpc.Core.RpcException
             * with a StatusCode and Detail (i.e. error message)
             * this is nice, it makes it "easier" to do a catch-when directly on the StatusCode instead of on the message
             * perhaps catching on the string message is easier on the eye,
             *  but this is "more statically typed" and thus less prone of accidental typo-errors, 
             */
            catch (InvalidOperationException e )
            when (e.InnerException != null && ((RpcException)e.InnerException).StatusCode == StatusCode.AlreadyExists)
            {

                var temp = e.Message;
                Console.WriteLine($"subscription {groupName} already exist, continuing");
            }

            /*
             * as long as the console is not closed the subscription will log new events
             */
            var _subscription = await client.SubscribeAsync(
                streamName: streamName,
                groupName: groupName,
                //eventAppeared: EventAppeared, 
                eventAppeared: (subscription, resolvedEvent, number, cancellationToken) =>
                {
                    Console.WriteLine("Received: " + Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray()));

                    //not sure why I should/would do this.. from test code in the client source. 
                    appeared.TrySetResult(resolvedEvent.OriginalEventNumber);
                    return Task.CompletedTask;
                },
                //subscriptionDropped: SubscriptionDropped,
                subscriptionDropped: (subscription, droppedReason, ex) =>
                {
                    //not sure why I should/would do this.. from test code in the client source. 
                    Console.WriteLine($"dropped subscription '{droppedReason}'");
                    dropped.TrySetResult(true);
                },
                userCredentials: userCredentials
            );


            Console.WriteLine($"waiting... new events in stream '{streamName}' will be logged to the console. press Enter to Quit");
            Console.Read();

        }


        private static EventStorePersistentSubscriptionsClient CreateEventStorePersistentSubscriptionsClient(UserCredentials userCredentials)
        {
            var settings = new EventStoreClientSettings
            {
                ConnectivitySettings = new EventStoreClientConnectivitySettings
                {
                    /*
                     * Note: gRPC uses the https and thus needs to use port 2113 (same as admin UI),
                     * instead of 1113 as the tcp client uses.
                     */
                    Address = new Uri("https://localhost:2113/")
                },
                DefaultCredentials = userCredentials
            };

            var client = new EventStorePersistentSubscriptionsClient(settings);
            return client;
        }



        /*
         *
         *
         * UNUSED CODE >>> BUT READ IF YOU LIKE.. =) 
         *
         *
         * the below code is only kept for own reference.. do not bother.. =)
         *
         *  this was where I ended up when trying to create a "PersistentSubscription"
         * before.. I figured out I had to use the EventStorePersistentSubscriptionsClient
         */
        [Obsolete("do not use")]
        private static async Task CreateSubscription()
        {
            var streamName = $"gRpc-person-stream";
            //var groupName = "gRpc-subscription-group";
            var userCredentials = new UserCredentials("admin", "changeit");

            var client = CreateEventStoreClientWithConnection();

            var appeared = new TaskCompletionSource<StreamPosition>();
            var dropped = new TaskCompletionSource<bool>();


            /*
             * to provide some context I have added the signatures of the parameters inline.
             */
            using var _ = await client.SubscribeToStreamAsync(
                streamName: streamName,
                start: StreamPosition.End,
                // Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> eventAppeared,
                eventAppeared: (subscription, resolvedEvent, cancellationToken) =>
                {
                    Console.WriteLine($"subscription id: {subscription.SubscriptionId}");
                    Console.WriteLine($"resolvedEvent id: {resolvedEvent.Event.EventId}");
                    Console.WriteLine($"resolvedEvent data: {resolvedEvent.Event.Data}");


                    appeared.TrySetResult(resolvedEvent.OriginalEventNumber); //not sure why I should/would do this.. from test code in the client source. 
                    return Task.CompletedTask;
                },
                //bool resolveLinkTos = false,
                resolveLinkTos: false,
                //Action<StreamSubscription, SubscriptionDroppedReason, Exception?>? subscriptionDropped = null,
                subscriptionDropped: (subscription, droppedReason, ex) =>
                {

                    dropped.TrySetResult(true); //not sure why I should/would do this.. from test code in the client source. 

                },
                //Action<EventStoreClientOperationOptions>? configureOperationOptions = null,
                configureOperationOptions: null,
                /*
                 * Not sure, but guessing the timeout tells how long the client should try to connect
                 * and bool ThrowOnAppendFailure is to tell if we care about the failure
                 * - the name though, suggest it has to do with failure appending events, not the reading.
                 * -- perhaps it is to have a bailout and cancel subscription if someone failed appending events?
                 * -- to nod read broken data?
                 *
                 */
                //configureOperationOptions: (options) =>
                //{
                //    options.TimeoutAfter = new TimeSpan(0,0,0, 10);
                //    /*
                //     *EventStoreClientOperationOptions
                //       {
                //       public TimeSpan? TimeoutAfter { get; set; }                       
                //       public bool ThrowOnAppendFailure { get; set; }
                //        }
                //    */
                //},

                //UserCredentials? userCredentials = null,
                userCredentials: userCredentials
            );

        }

        [Obsolete("do not use")]
        private static EventStoreClient CreateEventStoreClientWithConnection()
        {
            /** https://discuss.eventstore.com/t/basic-eventstoredb-v20-example/2553
             *  read this for settings workaround for certificate issues when trying out the client
             *  I didn't have this problem but if you are running event store in --dev mode this might be an issue'
             * @see example code in .Writer project (same as in link above)
             */

            var settings = new EventStoreClientSettings
            {
                ConnectivitySettings = new EventStoreClientConnectivitySettings
                {
                    /*
                     * Note: gRPC uses the https and thus needs to use port 2113 (same as admin UI),
                     * instead of 1113 as the tcp client uses.
                     */
                    Address = new Uri("https://localhost:2113/")
                }
            };

            var client = new EventStoreClient(settings);

            return client;

            /*
             *  In this example we used the EventStoreConnection.Create() overloaded method but others are available.
             * https://eventstore.com/docs/dotnet-api/code/EventStore.ClientAPI.EventStoreConnection.html
             *  instead of using tcp for the client connection gRPC is recommended. (but not yet in documentation)
             */
        }

    }

}
