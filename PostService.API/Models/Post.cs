using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PostService.API.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? ThreadId { get; set; }
        public string? AuthorId { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
