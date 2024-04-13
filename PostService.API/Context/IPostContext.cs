using PostService.API.Models;

namespace PostService.API.Context
{
    public interface IPostContext
    {
        public Task<Post?> GetAsync(string id);
        public Task<List<Post>> GetAsync();
        public Task CreateAsync(Post post);
        public Task UpdateAsync(Post post);
        public Task RemoveAsync(string id);
    }
}
