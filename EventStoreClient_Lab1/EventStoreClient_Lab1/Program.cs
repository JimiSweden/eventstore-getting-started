using System;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;


namespace EventStoreClient_Lab1
{


    /*
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

            var connection = CreateConnection();

            //this is just toggle creation of new stream ids > new streams for each run-
            const bool generateNewId = false; 
            var streamIdSuffix = generateNewId ? $"-{Guid.NewGuid()}" : ""; 
            
            //the stream name is where we want all events regarding this persons data 
            var streamName = $"person-stream{streamIdSuffix}";

            //the first event should create the stream, and it should never succed if the stream exists.
            // naming is a little bit verbose here , just to be extra clear on things when getting started. 
            var personAddedEventData = CreateEventDataPayloadPersonAdded();
            var personUpdatedEventData = CreateEventDataPayloadPersonUpdated();

            Console.WriteLine($"appending to stream '{streamName}'");

            Console.WriteLine($"appending {personAddedEventData.Type} to stream");
            connection.AppendToStreamAsync(streamName, ExpectedVersion.NoStream, personAddedEventData).Wait();
            
            Console.WriteLine("done - next");
            Console.WriteLine($"appending {personUpdatedEventData.Type} to stream");
            connection.AppendToStreamAsync(streamName, ExpectedVersion.StreamExists, personUpdatedEventData).Wait();
            
            Console.WriteLine("done - can we exit now please..?");

            Console.Read();
        }


        private static IEventStoreConnection CreateConnection()
        {
            /*
             *  In this example we used the EventStoreConnection.Create() overloaded method but others are available.
             * https://eventstore.com/docs/dotnet-api/code/EventStore.ClientAPI.EventStoreConnection.html
             *
             *  instead of using tcp for the client connection gRPC is recommended. (but not yet in documentation)
             *  you will need to use the 'eventstore.client.grpc' nuget packages 
             *  see example in EventStoreClient_gRpc_Lab1\EventStoreClient_gRpc_Lab1
             */
            var connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            connection.ConnectAsync().Wait();
            return connection;
        }

        /**
         * To use the .NET client, use the following method, passing the name of the stream, the version, and the events to write:
           var streamName = "newstream";
           var eventType = "event-type";
           var data = "{ \"a\":\"2\"}";
           var metadata = "{}";
           var eventPayload = new EventData(Guid.NewGuid(), eventType, true, Encoding.UTF8.GetBytes(data), Encoding.UTF8.GetBytes(metadata));
           conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventPayload).Wait();
         */

        private static EventData CreateEventDataPayloadPersonAdded()
        {
            
            var eventType = "personAdded";
            
            var data = "{ \"name\":\"jimi\"}";

            var metadata = "{}";
            var eventPayload = new EventData(
                Guid.NewGuid(), 
                eventType, 
                true,
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
                Guid.NewGuid(),
                eventType,
                true,
                Encoding.UTF8.GetBytes(data),
                Encoding.UTF8.GetBytes(metadata)
            );

            return eventPayload;
        }

    }

   
}
