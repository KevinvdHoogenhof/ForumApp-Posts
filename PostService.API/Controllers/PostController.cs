using Microsoft.AspNetCore.Mvc;
using PostService.API.Kafka;
using PostService.API.Models;
using PostService.API.Services;
using System.Text.Json;

namespace PostService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _service;
        private readonly IKafkaProducer _producer;
        private readonly IKafkaProducer2 _producer2;
        public PostController(IPostService service, IKafkaProducer producer, IKafkaProducer2 producer2)
        {
            _service = service;
            _producer = producer;
            _producer2 = producer2;
        }

        [HttpGet]
        public async Task<List<Post>> Get() =>
            await _service.GetPosts();

        [HttpGet("GetPostsByName")]
        public async Task<List<Post>> GetPostsByName(string name) =>
            await _service.GetPostsByName(name);

        [HttpGet("GetPostsByThreadId")]
        public async Task<List<Post>> GetPostsByThreadId(string id) =>
            await _service.GetPostsByThreadId(id);

        [HttpGet("GetPostsByAuthorId")]
        public async Task<List<Post>> GetPostsByAuthorId(int id) =>
            await _service.GetPostsByAuthorId(id);

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Post>> Get(string id) //For viewing one post = post + comments (current thread/threadname can be visible)
        {
            var post = await _service.GetPost(id);

            if (post is null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpPost]
        public async Task<ActionResult<Post>> Post(InsertPostDTO post, CancellationToken stoppingToken)
        {
            var insertedPost = await _service.InsertPost(new Post { ThreadId = post.ThreadId, ThreadName = post.ThreadName, AuthorId = post.AuthorId, AuthorName = post.AuthorName, Name = post.Name, Content = post.Content, Comments = 0 });

            int Posts = await _service.GetAmountOfPostsByThreadId(insertedPost.ThreadId);

            _ = _producer.Produce(JsonSerializer.Serialize(new { insertedPost?.ThreadId, Posts }), stoppingToken);

            return CreatedAtAction(nameof(Get), new
            {
                id = (insertedPost?.Id) ?? throw new InvalidOperationException("Failed to insert the post.")
            }, post);
        }

        /*[HttpPost]
        public async Task<ActionResult<Post>> Post(InsertPostDTO post) =>
            CreatedAtAction(nameof(Get), new
            {
                id = ((await _service.InsertPost(new Post { ThreadId = post.ThreadId, ThreadName = post.ThreadName, AuthorId = post.AuthorId, AuthorName = post.AuthorName, Name = post.Name, Content = post.Content, Comments = 0 }))?.Id)
                ?? throw new InvalidOperationException("Failed to insert the post.")
            }, post);*/

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<Post>> Update(string id, UpdatePostDTO post, CancellationToken stoppingToken)
        {
            var p = await _service.GetPost(id);

            if (p is null)
            {
                return NotFound();
            }

            p.Name = post.Name;
            p.Content = post.Content;
            var po = await _service.UpdatePost(p);

            if (p.Name != po?.Name)
            {
                await _producer2.Produce(JsonSerializer.Serialize(new { p.Id, po?.Name }), stoppingToken);
            }

            if (po is null)
            {
                return NotFound();
            }

            return po;
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var p = await _service.GetPost(id);

            if (p is null)
            {
                return NotFound();
            }

            await _service.DeletePost(id);

            return NoContent();
        }
    }
}
