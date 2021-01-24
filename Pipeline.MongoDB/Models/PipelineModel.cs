using Core.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Pipeline.MongoDB {
    [BsonIgnoreExtraElements]
    class PipelineMongoDbModel : IPipeline {
        [BsonId]
        [BsonRequired]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("name")]
        [BsonRequired]
        public string Name { get; set; }
        [BsonElement("tasks")]
        public IEnumerable<string> TaskIds { get; set; } = new List<string>();
        [BsonIgnore]
        public IEnumerable<IPipelineTask> Tasks { get; set; } = new List<IPipelineTask>();
        [BsonElement("createdAt")]
        [BsonRequired]
        public DateTime CreatedAt { get; set; }
        [BsonElement("runTime")]
        public int RunTime { get; set; }
        [BsonElement("userId")]
        [BsonRequired]
        public string UserId { get; set; }
        [BsonIgnore]
        public string UserName { get; set; }
        public PipelineMongoDbModel(IPipeline pipeline) {
            Id = pipeline.Id;
            Name = pipeline.Name;
            TaskIds = pipeline.TaskIds;
            Tasks = pipeline.Tasks;
            CreatedAt = pipeline.CreatedAt;
            RunTime = pipeline.RunTime;
            UserId = pipeline.UserId;
            UserName = pipeline.UserName;
        }
    }
}
