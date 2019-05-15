using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebBookmarks.Domain
{
    public class BookmarkStorageState
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("AccessDataUser")]
        public ICollection<BookmarkStorageState> AccessDataUser { get; set; }

        [BsonElement("Bookmarks")]
        public ICollection<Bookmark> Bookmarks { get; set; }

        [BsonElement("BookmarkGroups")]
        public ICollection<BookmarkGroup> BookmarkGroups { get; set; }

    }
}
