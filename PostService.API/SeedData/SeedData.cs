using PostService.API.Models;

namespace PostService.API.SeedData
{
    public class SeedData
    {
        public static IEnumerable<Post> GetPosts()
        {
            return
            [
                new Post()
                {
                    Id = "662235bfc3337eb9aaf2b983",
                    ThreadId = "6622190fe21ab10bc3650d6b",
                    ThreadName = "General",
                    AuthorId = 0,
                    AuthorName = "asd",
                    Name = "First Post!",
                    Content = "First post of the forum !!!",
                    Comments = 0
                },
                new()
                {
                    Id = "662235bfc3337eb9aaf2b984",
                    ThreadId = "6622190fe21ab10bc3650d6b",
                    ThreadName = "General",
                    AuthorId = 0,
                    AuthorName = "asd",
                    Name = "Second Post!!",
                    Content = "Content",
                    Comments = 0
                }
            ];
        }
    }
}
