using Core.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PipelineTask.MongoDB {
    [BsonIgnoreExtraElements]
    class PipelineTaskMongoDbModel : IPipelineTask {
        [BsonId]
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("name")]
        [BsonRequired]
        public string Name { get; set; }
        [BsonElement("averageTime")]
        [BsonRequired]
        public int AverageTime { get; set; }
        [BsonElement("createdAt")]
        [BsonRequired]
        public DateTime CreatedAt { get; set; }
        [BsonElement("userId")]
        [BsonRequired]
        public string UserId { get; set; }
        [BsonIgnore]
        public string UserName { get; set; }
        public PipelineTaskMongoDbModel(IPipelineTask task) {
            Id = task.Id;
            Name = task.Name;
            CreatedAt = task.CreatedAt;
            AverageTime = task.AverageTime;
            UserId = task.UserId;
            UserName = task.UserName;
        }
    }
}
