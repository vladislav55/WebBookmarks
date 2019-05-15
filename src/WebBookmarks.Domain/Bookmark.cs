using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebBookmarks.Domain
{
    public class Bookmark
    {
        [BsonElement("BookmarkId")]
        public Guid BookmarkId { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Uri")]
        public string Uri { get; set; }

        [BsonElement("CreateDate")]
        public DateTime CreateDate { get; set; }

        [BsonElement("UpdateDate")]
        public DateTime UpdateDate { get; set; }
    }
}
