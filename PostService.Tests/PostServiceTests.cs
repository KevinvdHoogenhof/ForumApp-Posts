using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using PostService.API;
using PostService.API.Context;
using PostService.API.SeedData;
using PostService.API.Services;

namespace PostService.Tests
{
    public class PostServiceTests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly HttpClient _client;
        private readonly IPostService _service;
        public PostServiceTests(MongoDbFixture fixture)
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
            _service = new API.Services.PostService(new PostContext(_fixture.Client, dataSeedingConfig));
        }
        [Fact]
        public async Task GetPostById_ShouldReturnPostWithCorrectId()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post1 = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            API.Models.Post post2 = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test2", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post1);
            await collection.InsertOneAsync(post2);

            // Act
            var p1 = await _service.GetPost(post1.Id);
            var p2 = await _service.GetPost(post2.Id);

            // Assert
            Assert.NotNull(p1);
            Assert.Equal(post1.Id, p1.Id);
            Assert.Equal(post1.ThreadId, p1.ThreadId);
            Assert.Equal(post1.ThreadName, p1.ThreadName);
            Assert.Equal(post1.AuthorId, p1.AuthorId);
            Assert.Equal(post1.AuthorName, p1.AuthorName);
            Assert.Equal(post1.Name, p1.Name);
            Assert.Equal(post1.Content, p1.Content);
            Assert.Equal(post1.Comments, p1.Comments);

            Assert.NotNull(p2);
            Assert.Equal(post2.Id, p2.Id);
            Assert.Equal(post2.ThreadId, p2.ThreadId);
            Assert.Equal(post2.ThreadName, p2.ThreadName);
            Assert.Equal(post2.AuthorId, p2.AuthorId);
            Assert.Equal(post2.AuthorName, p2.AuthorName);
            Assert.Equal(post2.Name, p2.Name);
            Assert.Equal(post2.Content, p2.Content);
            Assert.Equal(post2.Comments, p2.Comments);
        }
        [Fact]
        public async Task GetAllPosts_ShouldReturnPosts()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            await collection.InsertOneAsync(new API.Models.Post { ThreadId = "Test", ThreadName = "Test", AuthorId = 0, AuthorName = "Test", Name = "Test", Content = "Test", Comments = 0 });

            // Act
            var posts = await _service.GetPosts();

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Single(posts);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task GetAllPostByName_ShouldReturnPostsThatContainName()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post1 = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            API.Models.Post post2 = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test2", Content = "test", Comments = 0 };
            API.Models.Post post3 = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "different name", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post1);
            await collection.InsertOneAsync(post2);
            await collection.InsertOneAsync(post3);

            // Act
            var posts = await _service.GetPostsByName("Test");

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Equal(2, posts.Count);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Contains("Test", resultItem.Name);
        }
        [Fact]
        public async Task GetAllPostByThreadId_ShouldReturnPostsWithThreadId()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post1 = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            API.Models.Post post2 = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test2", Content = "test", Comments = 0 };
            API.Models.Post post3 = new() { ThreadId = "other", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "different name", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post1);
            await collection.InsertOneAsync(post2);
            await collection.InsertOneAsync(post3);

            // Act
            var posts = await _service.GetPostsByThreadId("tid");

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Equal(2, posts.Count);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal("tid", resultItem.ThreadId);
        }
        [Fact]
        public async Task GetAllPostByAuthorId_ShouldReturnPostsWithAuthorId()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post1 = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            API.Models.Post post2 = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test2", Content = "test", Comments = 0 };
            API.Models.Post post3 = new() { ThreadId = "other", ThreadName = "test", AuthorId = 1, AuthorName = "other", Name = "different name", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post1);
            await collection.InsertOneAsync(post2);
            await collection.InsertOneAsync(post3);

            // Act
            var posts = await _service.GetPostsByAuthorId(0);

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Equal(2, posts.Count);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal(0, resultItem.AuthorId);
        }
        [Fact]
        public async Task InsertOnePost_ShouldInsertPost()
        {
            //Arrange
            API.Models.Post post = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };

            // Act
            var p = await _service.InsertPost(post);
            var posts = await _service.GetPosts();

            // Assert
            Assert.NotNull(p);
            Assert.Equal(post.Id, p.Id);
            Assert.Equal(post.ThreadId, p.ThreadId);
            Assert.Equal(post.ThreadName, p.ThreadName);
            Assert.Equal(post.AuthorId, p.AuthorId);
            Assert.Equal(post.AuthorName, p.AuthorName);
            Assert.Equal(post.Name, p.Name);
            Assert.Equal(post.Content, p.Content);
            Assert.Equal(post.Comments, p.Comments);

            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Single(posts);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task UpdateOnePost_ShouldUpdatePost()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("PostDB");
            var collection = _db.GetCollection<API.Models.Post>("Posts");
            API.Models.Post post = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post);

            // Act
            var p = await _service.GetPost(post.Id);
            p.Name = "Updated Name";
            p.Content = "Update Content";
            _ = await _service.UpdatePost(p);
            var posts = await _service.GetPosts();

            // Assert
            Assert.NotNull(p);
            Assert.Equal(post.Id, p.Id);
            Assert.Equal("Updated Name", p.Name);
            Assert.Equal("Update Content", p.Content);
            Assert.Equal(post.Comments, p.Comments);

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
            API.Models.Post post = new() { ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };
            await collection.InsertOneAsync(post);

            // Act
            await _service.DeletePost(post.Id);
            var posts = await _service.GetPosts();

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
