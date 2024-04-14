using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PostService.API.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string ThreadId { get; set; } = null!;
        public string? ThreadName { get; set; } 
        public int AuthorId { get; set; } = 0;
        public string? AuthorName { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int Comments { get; set; } = 0;
    }
}
