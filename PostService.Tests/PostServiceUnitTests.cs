using Microsoft.AspNetCore.Http.HttpResults;
using PostService.API.Context;
using PostService.API.Models;
using PostService.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Tests
{
    public class PostServiceUnitTests
    {
        public PostServiceUnitTests()
        {
        }
        [Fact]
        public async Task GetPostById_ShouldReturnPostWithCorrectId()
        {
            //Arrange
            IPostService _service = new API.Services.PostService(new TestDAL());

            // Act
            var p1 = await _service.GetPost("post1");
            var p2 = await _service.GetPost("post2");

            // Assert
            Assert.NotNull(p1);
            Assert.Equal("post1", p1.Id);

            Assert.NotNull(p2);
            Assert.Equal("post2", p2.Id);
        }
        [Fact]
        public async Task GetAllPosts_ShouldReturnPosts()
        {
            // Arrange
            IPostService _service = new API.Services.PostService(new TestDAL());

            // Act
            var posts = await _service.GetPosts();

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Equal(3, posts.Count);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task GetAllPostByName_ShouldReturnPostsThatContainName()
        {
            // Arrange
            IPostService _service = new API.Services.PostService(new TestDAL());

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
            IPostService _service = new API.Services.PostService(new TestDAL());

            // Act
            var posts = await _service.GetPostsByThreadId("tid1");

            // Assert
            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Single(posts);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal("tid1", resultItem.ThreadId);
        }
        [Fact]
        public async Task GetAllPostByAuthorId_ShouldReturnPostsWithAuthorId()
        {
            // Arrange
            IPostService _service = new API.Services.PostService(new TestDAL());

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
            IPostService _service = new API.Services.PostService(new TestDAL());
            Post post = new() { Id = "post4", ThreadId = "tid", ThreadName = "test", AuthorId = 0, AuthorName = "asd", Name = "Test", Content = "test", Comments = 0 };

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
            Assert.Equal(4, posts.Count);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task UpdateOnePost_ShouldUpdatePost()
        {
            // Arrange
            IPostService _service = new API.Services.PostService(new TestDAL());

            // Act
            var p = await _service.GetPost("post3");
            p.Name = "Updated Name";
            p.Content = "Update Content";
            _ = await _service.UpdatePost(p);
            var posts = await _service.GetPosts();

            // Assert
            Assert.NotNull(p);
            Assert.Equal("post3", p.Id);
            Assert.Equal("Updated Name", p.Name);
            Assert.Equal("Update Content", p.Content);

            Assert.NotNull(posts);
            Assert.NotEmpty(posts);
            Assert.Equal(3, posts.Count);
            var resultItem = posts.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task DeleteOnePost_ShouldDeletePost()
        {
            // Arrange
            IPostService _service = new API.Services.PostService(new TestDAL());

            // Act
            await _service.DeletePost("post1");
            await _service.DeletePost("post2");
            await _service.DeletePost("post3");
            var posts = await _service.GetPosts();

            // Assert
            Assert.NotNull(posts);
            Assert.Empty(posts);
        }
        private class TestDAL : IPostContext
        {
            readonly List<Post> _posts = new List<Post>
            {
            new() { Id = "post1", ThreadId = "tid1", ThreadName = "Thread 1", AuthorId = 0, AuthorName = "asd", Name = "Post 1", Content = "Content of post 1", Comments = 0 },
            new() { Id = "post2", ThreadId = "tid2", ThreadName = "Thread 2", AuthorId = 0, AuthorName = "xyz", Name = "Post 2 Test", Content = "Content of post 2", Comments = 2 },
            new() { Id = "post3", ThreadId = "tid3", ThreadName = "Thread 3", AuthorId = 1, AuthorName = "abc", Name = "Post 3 Test", Content = "Content of post 3", Comments = 1 }
            };
            Task<Post?> IPostContext.CreateAsync(Post post)
            {
                _posts.Add(post);
                return Task.FromResult<Post?>(post);
            }

            Task<Post?> IPostContext.GetAsync(string id)
            {
                Post? post = _posts.FirstOrDefault(post => post.Id == id);
                if (post != null)
                {
                    return Task.FromResult<Post?>(post);
                }
                else
                {
                    return Task.FromResult<Post?>(null);
                }
            }

            Task<List<Post>> IPostContext.GetAsync()
            {
                return Task.FromResult(_posts);
            }

            Task<List<Post>> IPostContext.GetAsyncByAuthorId(int id)
            {
                var posts = _posts.Where(post => post.AuthorId == id).ToList();
                return Task.FromResult(posts);
            }

            Task<List<Post>> IPostContext.GetAsyncByThreadId(string id)
            {
                var posts = _posts.Where(post => post.ThreadId == id).ToList();
                return Task.FromResult(posts);
            }

            Task<List<Post>> IPostContext.GetAsyncNameSearch(string name)
            {
                var posts = _posts.Where(post => post.Name.Contains(name)).ToList();
                return Task.FromResult(posts);
            }

            Task<int> IPostContext.GetAsyncPostAmountByThreadId(string id)
            {
                var posts = _posts.Where(post => post.ThreadId == id).ToList();
                return Task.FromResult(posts.Count);
            }

            Task IPostContext.RemoveAsync(string id)
            {
                var postToRemove = _posts.FirstOrDefault(post => post.Id == id);
                if (postToRemove != null)
                {
                    _posts.Remove(postToRemove);
                }
                return Task.CompletedTask;
            }

            Task<Post?> IPostContext.UpdateAsync(Post post)
            {
                Post? existingPost = _posts.FirstOrDefault(p => p.Id == post.Id);
                if (existingPost != null)
                {
                    existingPost.ThreadId = post.ThreadId;
                    existingPost.ThreadName = post.ThreadName;
                    existingPost.AuthorId = post.AuthorId;
                    existingPost.AuthorName = post.AuthorName;
                    existingPost.Name = post.Name;
                    existingPost.Content = post.Content;
                    existingPost.Comments = post.Comments;

                    return Task.FromResult<Post?>(existingPost);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
