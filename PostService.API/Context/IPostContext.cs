using PostService.API.Models;

namespace PostService.API.Context
{
    public interface IPostContext
    {
        public Task<Post?> GetAsync(string id);
        public Task<List<Post>> GetAsync();
        public Task<List<Post>> GetAsyncNameSearch(string name);
        public Task<List<Post>> GetAsyncByThreadId(string id);
        public Task<int> GetAsyncPostAmountByThreadId(string id);
        public Task<List<Post>> GetAsyncByAuthorId(int id);
        public Task<Post?> CreateAsync(Post post);
        public Task<Post?> UpdateAsync(Post post);
        public Task RemoveAsync(string id);
    }
}
