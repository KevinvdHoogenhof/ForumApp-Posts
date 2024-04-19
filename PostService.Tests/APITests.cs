using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using PostService.API;
using PostService.API.SeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PostService.Tests
{
    public class APITests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly HttpClient _client;

        public APITests(MongoDbFixture fixture)
        {
            _fixture = fixture;
            var dataSeedingConfig = new DataSeedingConfiguration { SeedDataEnabled = false };
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<IMongoClient>();
                        services.AddSingleton<IMongoClient>(
                            (_) => _fixture.Client);
                        services.RemoveAll<IDataSeedingConfiguration>();
                        services.AddSingleton<IDataSeedingConfiguration>(dataSeedingConfig);
                    });
                });
            _client = appFactory.CreateClient();
        }
        [Fact]
        public async Task GetAllPosts_ShouldReturnPosts()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            await collection.InsertOneAsync(new API.Models.Post { ThreadId = "Test", ThreadName = "Test", AuthorId = 0, AuthorName = "Test", Name = "Test", Content = "Test", Comments= 0 });

            // Act
            var res = await _client.GetAsync("/post");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<ICollection<API.Models.Post>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Single(posts);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task GetAllPosts_NoPostsInDb_ShouldReturnNoPosts()
        {
            // Arrange

            // Act
            var res = await _client.GetAsync("/post");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<ICollection<API.Models.Post>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(posts);
            Assert.Empty(posts);
        }
        public void Dispose()
        {
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            collection.DeleteManyAsync(_ => true).Wait();
        }
    }
}
