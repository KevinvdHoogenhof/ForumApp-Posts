using PostService.API.Models;

namespace PostService.API.Services
{
    public interface IPostService
    {
        public Task<Post?> GetPost(string id);
        public Task<List<Post>> GetPosts();
        public Task InsertPost(Post post);
        public Task UpdatePost(Post post);
        public Task DeletePost(string id);
    }
}
