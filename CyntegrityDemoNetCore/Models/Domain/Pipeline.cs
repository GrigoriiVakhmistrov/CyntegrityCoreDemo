using CyntegrityDemoNetCore.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CyntegrityDemoNetCore.Models {
    [BsonIgnoreExtraElements]
    public class Pipeline {
        [BsonId]
        [BsonRequired]
        [ModelBinder(BinderType = typeof(ObjectIdModelBinder))]
        public ObjectId Id { get; set; }
        [BsonElement("name")]
        [BsonRequired]
        [Required(ErrorMessage = "Name is required")]
        public String Name { get; set; }
        [BsonElement("tasks")]
        [ModelBinder(BinderType = typeof(ListObjectIdModelBinder))]
        public List<ObjectId> TaskIds { get; set; } = new List<ObjectId>();
        [BsonIgnore]
        public List<PipelineTask> Tasks { get; set; } = new List<PipelineTask>();
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
    }
}
