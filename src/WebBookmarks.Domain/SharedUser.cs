using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebBookmarks.Domain
{
    public class SharedUser
    {
        [BsonElement("UserId")]
        public Guid UserId { get; set; }

        [BsonElement("Access")]
        public Access Access { get; set; }
    }
}
