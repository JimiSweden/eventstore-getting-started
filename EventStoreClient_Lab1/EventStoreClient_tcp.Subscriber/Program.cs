using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace EventStoreClient_tcp.Subscriber
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var streamName = $"tcp-person-stream";
            var groupName = "tcp-subscription-group";
            var userCredentials = new UserCredentials("admin", "changeit");

            var connection = CreateEventStoreConnection(userCredentials.Username, userCredentials.Password);

            /*
             * do not resolve linktos (todo: add explanation here. reference blog article.
             * start subscription from current position in stream,
             * - meaning only act on events coming in after this subscription is created.
             * - you can start from the beginning, StartFromBeginning, or any given position
             */
            var settings = PersistentSubscriptionSettings
                .Create()
                .DoNotResolveLinkTos()
                .StartFromCurrent();
            
            /*
             * since a subscription might already have been created
             * we need to wrap the creation, catch the exception and "silence" it.
             */
            try
            {
                await connection.CreatePersistentSubscriptionAsync(
                    streamName,
                    groupName,
                    settings,
                    userCredentials
                );

            }
            catch (InvalidOperationException e) 
                when(e.Message.Contains($"Subscription group {groupName} on stream {streamName} already exists")
                )
            {

                //InvalidOperationException: Subscription group tcp-subscription-group on stream tcp-person-stream already exists

                Console.WriteLine($"subscription {groupName} already exist, continuing");
                //throw;
            }

            /*
             * as long as the console is not closed the subscription will log new events
             */
            var subscription = connection.ConnectToPersistentSubscriptionAsync(
                stream: streamName, 
                groupName: groupName, 
                eventAppeared: (_, resolvedEvent) =>
                {
                    Console.WriteLine("Received: " + Encoding.UTF8.GetString(resolvedEvent.Event.Data));
                }).Result;

            Console.WriteLine($"waiting... new events in stream '{streamName}' will be logged to the console. press Enter to Quit");
            Console.Read();

        }

        private static IEventStoreConnection CreateEventStoreConnection(string userName, string password)
        {
            /*
             *  In this example we used the EventStoreConnection.Create() overloaded method but others are available.
             * https://eventstore.com/docs/dotnet-api/code/EventStore.ClientAPI.EventStoreConnection.html
             *
             *  instead of using tcp for the client connection gRPC is recommended. (but not yet in documentation)
             *  you will need to use the 'eventstore.client.grpc' nuget packages 
             *  see example in EventStoreClient_gRpc.Writer\EventStoreClient_gRpc.Writer
             */
            var connection = EventStoreConnection.Create(new Uri($"tcp://{userName}:{password}@localhost:1113"));
            connection.ConnectAsync().Wait();
            return connection;
        }
        
    }
}
