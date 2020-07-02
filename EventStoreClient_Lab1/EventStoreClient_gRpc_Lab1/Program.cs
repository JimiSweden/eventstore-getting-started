using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using EventStore.Client;
using EventStore.Client.Streams;

namespace EventStoreClient_gRpc_Lab1
{/*
     * Following the getting started guide at
     * https://eventstore.com/docs/getting-started/index.html?tabs=tabid-1%2Ctabid-dotnet-client%2Ctabid-dotnet-client-connect%2Ctabid-5#first-call-to-http-api
     *  
     */
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World of Streams and Events!");
            Console.WriteLine("");

            var client = CreateClientWithConnection();

            //this is just toggle creation of new stream ids > new streams for each run-
            const bool generateNewId = false;
            var streamIdSuffix = generateNewId ? $"-{Guid.NewGuid()}" : "";

            //the stream name is where we want all events regarding this persons data 
            var streamName = $"gRPC-person-stream{streamIdSuffix}";

            //the first event should create the stream, and it should never succed if the stream exists.
            // naming is a little bit verbose here , just to be extra clear on things when getting started. 
            var personAddedEventData = CreateEventDataPayloadPersonAdded();
            var personUpdatedEventData = CreateEventDataPayloadPersonUpdated();

            Console.WriteLine($"appending to stream '{streamName}'");

            Console.WriteLine($"appending {personAddedEventData.Type} to stream");


            var myFirstEvents = new List<EventData> {personAddedEventData};
            client.AppendToStreamAsync(streamName, StreamState.NoStream, myFirstEvents).Wait();

            //client.AppendToStreamAsync(streamName, "-1", personAddedEventData).Wait();
            //connection.AppendToStreamAsync(streamName, ExpectedVersion.NoStream, personAddedEventData).Wait();

            Console.WriteLine("done - next");
            Console.WriteLine($"appending {personUpdatedEventData.Type} to stream");

            var myNextEvents = new List<EventData> { personUpdatedEventData };
            client.AppendToStreamAsync(streamName, StreamState.StreamExists, myNextEvents).Wait();
            //client.AppendToStreamAsync(streamName, "-4", personUpdatedEventData).Wait();
            //connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, personUpdatedEventData).Wait();

            Console.WriteLine("done - can we exit now please..?");

            Console.Read();
        }

        /*
         * note, when reading later..credentials are used like this.         *
         * new UserCredentials("admin", "changeit")
         *
         *var e = await client.ReadAllAsync(
           Direction.Forwards, 
           Position.Start, 
           userCredentials: new UserCredentials("admin", "changeit")).ToArrayAsync();
         */

        private static EventStoreClient CreateClientWithConnection()
        {
            /** https://discuss.eventstore.com/t/basic-eventstoredb-v20-example/2553
             *  settings workaround for certificate issues when trying out the client
             */
            var settingsWorkAround = new EventStoreClientSettings
            {
                CreateHttpMessageHandler = () =>
                    new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (message, certificate2, x509Chain, sslPolicyErrors) => true
                    },
                ConnectivitySettings = {
                    Address = new Uri("https://localhost:2113")
                }
            };

            //if this doesn't work, try the above.. 
            var settings = new EventStoreClientSettings
            {
                ConnectivitySettings = new EventStoreClientConnectivitySettings
                {
                    //Note: gRPC uses the https and thus needs to use port 2113 (same as admin UI), instead of 1113 as the tcp client uses.
                    Address = new Uri("https://localhost:2113/")
                }
            };

            var client = new EventStoreClient(settings);


            return client;


            ///*
            // *  In this example we used the EventStoreConnection.Create() overloaded method but others are available.
            // * https://eventstore.com/docs/dotnet-api/code/EventStore.ClientAPI.EventStoreConnection.html
            // *
            // *  instead of using tcp for the client connection gRPC is recommended. (but not yet in documentation)
            // */

            //var connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            ////var connection = EventStoreConnection.Create(new Uri("gRPC://admin:changeit@localhost:1113"));
            //connection.ConnectAsync().Wait();
            //return connection;
        }

        /**
         * To use the .NET client, use the following method, passing the name of the stream, the version, and the events to write:
           var streamName = "newstream";
           var eventType = "event-type";
           var data = "{ \"a\":\"2\"}";
           var metadata = "{}";

         //note: "isJson" is not used in gRPC version of EventData, otherwise they seem similar
           var eventPayload = new EventData(Guid.NewGuid(), eventType, true, Encoding.UTF8.GetBytes(data), Encoding.UTF8.GetBytes(metadata));
           conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventPayload).Wait();
         */

        private static EventData CreateEventDataPayloadPersonAdded()
        {

            var eventType = "personAdded";

            var data = "{ \"name\":\"jimi\"}";

            var metadata = "{}";
            var eventPayload = new EventData(
                Uuid.NewUuid(),
                eventType,
                Encoding.UTF8.GetBytes(data),
                Encoding.UTF8.GetBytes(metadata)
                );

            return eventPayload;
        }


        private static EventData CreateEventDataPayloadPersonUpdated()
        {

            var eventType = "personUpdated";

            var data = "{ \"name\":\"jimi lee friis\", \"linkedin\":\"https://www.linkedin.com/in/jimi-friis-b729155/\"}";

            var metadata = "{}";
            var eventPayload = new EventData(
                Uuid.NewUuid(),
                eventType,
                Encoding.UTF8.GetBytes(data),
                Encoding.UTF8.GetBytes(metadata)
            );

            return eventPayload;
        }

    }
}
