using Microsoft.Extensions.Options;
using MongoDB.Driver;
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

        public async Task<List<Post>> GetPostsByThreadID(string id)
        {
            return await _context.GetAsyncByThreadID(id);
        }

        public async Task<List<Post>> GetPostsByAuthorID(int id)
        {
            return await _context.GetAsyncByAuthorID(id);
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
