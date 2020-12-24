using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using PipelineConsoleNetCore.Models;
using System;

namespace PipelineConsoleNetCore {
    class Program {
        static IConfiguration AppConfiguration { get; set; }

        static async System.Threading.Tasks.Task Main(string[] args) {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            AppConfiguration = configBuilder.Build();

            string pid = "";

            if (args.Length > 0 && args[0] == "-pid") {
                pid = args[1];
            } else {
                Console.WriteLine("Please, start program with pipeline id:\nPipelineConsole.exe -pid YOUR_PIPELINE_ID_HERE");
                Environment.Exit(0);
            }

            string connectionString = AppConfiguration["ConnectionStrings:MongoDb"];
            Console.WriteLine(connectionString);
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase("test");
            var collection = database.GetCollection<BsonDocument>("pipelines");
            var taskCollection = database.GetCollection<BsonDocument>("tasks");

            int pipelineRunTime = 0;

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("_id", ObjectId.Parse(pid));
            BsonDocument bson = await collection.Find(filter).FirstOrDefaultAsync();
            Pipeline pipeline = BsonSerializer.Deserialize<Pipeline>(bson);

            foreach (var taskId in pipeline.TaskIds) {
                var taskFilterBuilder = Builders<BsonDocument>.Filter;
                var taskFilter = taskFilterBuilder.Eq("_id", taskId);
                BsonDocument bsonTask = await taskCollection.Find(taskFilter).FirstOrDefaultAsync();
                PipelineTask task = BsonSerializer.Deserialize<PipelineTask>(bsonTask);
                pipeline.Tasks.Add(task);
                pipelineRunTime += task.AverageTime;
            }

            Console.WriteLine(pipelineRunTime + " seconds");
            Environment.Exit(0);
        }
    }
}
