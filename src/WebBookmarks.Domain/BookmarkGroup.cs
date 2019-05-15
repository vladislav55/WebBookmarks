using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace WebBookmarks.Domain
{
    public class BookmarkGroup
    {
        public BookmarkGroup()
        {
            CreateDate = DateTime.UtcNow;
            BookmarkGroupId = Guid.NewGuid();
            Bookmarks = new HashSet<Bookmark>();
        }

        [BsonElement("BookmarkGroupId")]
        public Guid BookmarkGroupId { get; private set; }

        [BsonElement("ParentGroupId")]
        public Guid? ParentGroupId { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("CreateDate")]
        public DateTime CreateDate { get; private set; }

        [BsonElement("UpdateDate")]
        public DateTime UpdateDate { get; set; }

        [BsonElement("Bookmarks")]
        public ICollection<Bookmark> Bookmarks { get; set; }
    }
}
