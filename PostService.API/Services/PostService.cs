using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PostService.API.Models;

namespace PostService.API.Services
{
    public class PostService
    {
        private readonly IMongoCollection<Post> _posts;
        public PostService(IOptions<PostDBSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _posts = mongoDatabase.GetCollection<Post>(settings.Value.CollectionName);
        }
        public async Task<Post?> GetPost(string id)
        {
            return await _posts.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Post>> GetPosts()
        {
            return await _posts.Find(_ => true).ToListAsync();
        }

        public async Task InsertPost(Post post)
        {
            await _posts.InsertOneAsync(post);
        }

        public async Task UpdatePost(Post post)
        {
            await _posts.ReplaceOneAsync(x => x.Id == post.Id, post);
        }

        public async Task DeletePost(string id)
        {
            await _posts.DeleteOneAsync(x => x.Id == id);
        }
    }
}
