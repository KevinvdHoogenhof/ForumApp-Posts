using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PostService.API.Models;

namespace PostService.API.Context
{
    public class PostContext : IPostContext
    {
        private readonly IMongoCollection<Post> _posts;
        public PostContext(IOptions<PostDBSettings> postdbsettings)
        {
            var mongoClient = new MongoClient(postdbsettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(postdbsettings.Value.DatabaseName);
            _posts = mongoDatabase.GetCollection<Post>(postdbsettings.Value.CollectionName);
        }
        public async Task<Post?> GetAsync(string id)
        {
            return await _posts.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Post>> GetAsync()
        {
            return await _posts.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(Post post)
        {
            await _posts.InsertOneAsync(post);
        }

        public async Task UpdateAsync(Post post)
        {
            await _posts.ReplaceOneAsync(x => x.Id == post.Id, post);
        }

        public async Task RemoveAsync(string id)
        {
            await _posts.DeleteOneAsync(x => x.Id == id);
        }
    }
}
