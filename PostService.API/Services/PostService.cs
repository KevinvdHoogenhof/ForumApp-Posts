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

        public async Task InsertPost(Post post)
        {
            await _context.CreateAsync(post);
        }

        public async Task UpdatePost(Post post)
        {
            await _context.UpdateAsync(post);
        }

        public async Task DeletePost(string id)
        {
            await _context.RemoveAsync(id);
        }
    }
}
