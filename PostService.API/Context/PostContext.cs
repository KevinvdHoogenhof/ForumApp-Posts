﻿using MongoDB.Driver;
using PostService.API.Models;
using PostService.API.SeedData;

namespace PostService.API.Context
{
    public class PostContext : IPostContext
    {
        private readonly IMongoCollection<Post> _posts;
        public PostContext(IMongoClient mongoClient, IDataSeedingConfiguration dataSeedingConfig)
        {
            var mongoDatabase = mongoClient.GetDatabase("PostDB");
            _posts = mongoDatabase.GetCollection<Post>("Posts");
            if (dataSeedingConfig.SeedDataEnabled && !_posts.AsQueryable().Any())
            {
                _posts.InsertManyAsync(SeedData.SeedData.GetPosts());
            }
        }
        public async Task<Post?> GetAsync(string id)
        {
            return await _posts.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Post>> GetAsync()
        {
            return await _posts.Find(_ => true).ToListAsync();
        }

        public async Task<List<Post>> GetAsyncNameSearch(string name)
        {
            var filter = Builders<Post>.Filter.Where(p => p.Name.Contains(name));
            return await (await _posts.FindAsync(filter)).ToListAsync();
        }

        public async Task<List<Post>> GetAsyncByThreadId(string id)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.ThreadId, id);
            return await (await _posts.FindAsync(filter)).ToListAsync();
        }

        public async Task<int> GetAsyncPostAmountByThreadId(string id)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.ThreadId, id);
            return (int)await _posts.CountDocumentsAsync(filter);
        }

        public async Task<List<Post>> GetAsyncByAuthorId(int id)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.AuthorId, id);
            return await (await _posts.FindAsync(filter)).ToListAsync();
        }

        public async Task<Post?> CreateAsync(Post post)
        {
            await _posts.InsertOneAsync(post);
            return post;
        }

        public async Task<Post?> UpdateAsync(Post post)
        {
            return (await _posts.ReplaceOneAsync(x => x.Id == post.Id, post)).IsAcknowledged
                ? await _posts.Find(x => x.Id == post.Id).FirstOrDefaultAsync()
                : null;
        }

        public async Task RemoveAsync(string id)
        {
            await _posts.DeleteOneAsync(x => x.Id == id);
        }
    }
}
