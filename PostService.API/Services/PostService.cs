using PostService.API.Context;
using PostService.API.Models;

namespace PostService.API.Services
{
    public class PostService : IPostService
    {
        private readonly IPostContext _context;
        public PostService(IPostContext context)
        {
            _context = context;
        }

        public async Task<Post?> GetPost(string id)
        {
            return await _context.GetAsync(id);
        }

        public async Task<List<Post>> GetPosts()
        {
            return await _context.GetAsync();
        }

        public async Task<List<Post>> GetPostsByName(string name)
        {
            return await _context.GetAsyncNameSearch(name);
        }

        public async Task<List<Post>> GetPostsByThreadId(string id)
        {
            return await _context.GetAsyncByThreadId(id);
        }

        public async Task<int> GetAmountOfPostsByThreadId(string id)
        {
            return await _context.GetAsyncPostAmountByThreadId(id);
        }

        public async Task<List<Post>> GetPostsByAuthorId(int id)
        {
            return await _context.GetAsyncByAuthorId(id);
        }

        public async Task<Post?> InsertPost(Post post)
        {
            return await _context.CreateAsync(post);
        }

        public async Task<Post?> UpdatePost(Post post)
        {
            return await _context.UpdateAsync(post);
        }

        public async Task DeletePost(string id)
        {
            await _context.RemoveAsync(id);
        }
    }
}
