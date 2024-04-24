namespace PostService.API.Models
{
    public class InsertPostDTO
    {
        public string ThreadId { get; set; } = null!;
        public string? ThreadName { get; set; }
        public int AuthorId { get; set; } = 0;
        public string? AuthorName { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
