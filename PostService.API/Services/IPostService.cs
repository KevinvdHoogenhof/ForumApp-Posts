using PostService.API.Models;

namespace PostService.API.Services
{
    public interface IPostService
    {
        public Task<Post?> GetPost(string id);
        public Task<List<Post>> GetPosts();
        public Task<List<Post>> GetPostsByName(string name);
        public Task<List<Post>> GetPostsByThreadID(string id);
        public Task<List<Post>> GetPostsByAuthorID(int id);
        public Task<Post?> InsertPost(Post post);
        public Task<Post?> UpdatePost(Post post);
        public Task DeletePost(string id);
    }
}
