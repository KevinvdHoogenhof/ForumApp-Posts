using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using PostService.API;
using PostService.API.SeedData;
using System.Text;
using System.Text.Json;

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
            await collection.InsertOneAsync(new API.Models.Post { ThreadId = "tid1_1234512345123451234", ThreadName = "Test", AuthorId = 0, AuthorName = "Test", Name = "Test", Content = "Test", Comments= 0 });

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
        [Fact]
        public async Task GetPostsByName_ShouldReturnPostsThatContainName()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post1 = new() { ThreadId = "tid1_1234512345123451234", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            API.Models.Post post2 = new() { ThreadId = "tid1_1234512345123451234", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test2", Content = "test", Comments = 0 };
            API.Models.Post post3 = new() { ThreadId = "other_123451234512345123", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "different name", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post1);
            await collection.InsertOneAsync(post2);
            await collection.InsertOneAsync(post3);

            // Act
            var res = await _client.GetAsync("/post/getpostsbyname/Test");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<ICollection<API.Models.Post>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Equal(2, posts.Count);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Contains("Test", resultItem.Name);
        }
        [Fact]
        public async Task GetPostsByThreadId_ShouldReturnPostsWithThreadId()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post1 = new() { ThreadId = "tid1_1234512345123451234", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            API.Models.Post post2 = new() { ThreadId = "tid1_1234512345123451234", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test2", Content = "test", Comments = 0 };
            API.Models.Post post3 = new() { ThreadId = "other_123451234512345123", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "different name", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post1);
            await collection.InsertOneAsync(post2);
            await collection.InsertOneAsync(post3);

            // Act
            var res = await _client.GetAsync("/post/getpostsbythreadid/tid1_1234512345123451234");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<ICollection<API.Models.Post>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Equal(2, posts.Count);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal("tid1_1234512345123451234", resultItem.ThreadId);
        }
        [Fact]
        public async Task GetPostsByAuthorId_ShouldReturnPostsWithAuthorId()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post1 = new() { ThreadId = "tid1_1234512345123451234", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            API.Models.Post post2 = new() { ThreadId = "tid1_1234512345123451234", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test2", Content = "test", Comments = 0 };
            API.Models.Post post3 = new() { ThreadId = "other_123451234512345123", ThreadName = "test", AuthorId = 1, AuthorName = "Different author", Name = "different name", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post1);
            await collection.InsertOneAsync(post2);
            await collection.InsertOneAsync(post3);

            // Act
            var res = await _client.GetAsync("/post/getpostsbyauthorid/0");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<ICollection<API.Models.Post>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Equal(2, posts.Count);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal(0, resultItem.AuthorId);
        }
        [Fact]
        public async Task PostPost_ShouldPostPost()
        {
            //Arrange
            API.Models.InsertPostDTO post = new() { ThreadId = "tid1_1234512345123451234", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test" };

            // Act
            var res = await _client.PostAsync("/post/", new StringContent(JsonSerializer.Serialize(post), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var p = JsonSerializer.Deserialize<API.Models.Post>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var res2 = await _client.GetAsync("/post");
            res2.EnsureSuccessStatusCode();
            var content2 = await res2.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<ICollection<API.Models.Post>>(content2, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(p);
            Assert.Equal(post.Name, p.Name);
            Assert.Equal(post.Content, p.Content);
            Assert.Equal(0, p.Comments);

            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Single(posts);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task UpdatePost_ShouldUpdatePost()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post = new() { ThreadId = "tid1_1234512345123451234", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post);

            API.Models.UpdatePostDTO updatedpost = new() { Name = "Updated name", Content = "Updated content" };

            // Act
            var res = await _client.PutAsync($"/post/{post.Id}", new StringContent(JsonSerializer.Serialize(updatedpost), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var p = JsonSerializer.Deserialize<API.Models.Post>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var res2 = await _client.GetAsync("/post");
            res2.EnsureSuccessStatusCode();
            var content2 = await res2.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<ICollection<API.Models.Post>>(content2, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(p);
            Assert.Equal(post.ThreadId, p.ThreadId);
            Assert.Equal(post.ThreadName, p.ThreadName);
            Assert.Equal(updatedpost.Name, p.Name);
            Assert.Equal(updatedpost.Content, p.Content);
            Assert.Equal(0, p.Comments);

            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Single(posts);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task DeleteOnePost_ShouldDeletePost()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post = new() { ThreadId = "tid1_1234512345123451234", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post);

            // Act
            var res = await _client.DeleteAsync($"/post/{post.Id}");
            res.EnsureSuccessStatusCode();

            var res2 = await _client.GetAsync("/post");
            res2.EnsureSuccessStatusCode();
            var content = await res2.Content.ReadAsStringAsync();
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
