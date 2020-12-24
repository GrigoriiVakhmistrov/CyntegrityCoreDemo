using CyntegrityDemoNetCore.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace CyntegrityDemoNetCore.Models {
    [BsonIgnoreExtraElements]
    public class PipelineTask {
        [ModelBinder(BinderType = typeof(ObjectIdModelBinder))]
        [BsonId]
        [BsonRequired]
        public ObjectId Id { get; set; }
        [BsonElement("name")]
        [BsonRequired]
        [Required(ErrorMessage = "Name is required")]
        public String Name { get; set; }
        [BsonElement("averageTime")]
        [BsonRequired]
        [Required(ErrorMessage = "Average time is required")]
        [Range(typeof(int), "0", "2147483647", ErrorMessage = "Average time must be in range 0..214748367")]
        public int AverageTime { get; set; }
        [BsonElement("createdAt")]
        [BsonRequired]
        public DateTime CreatedAt { get; set; }
        [BsonElement("userId")]
        [BsonRequired]
        public string UserId { get; set; }
        [BsonIgnore]
        public string UserName { get; set; }
    }
}
