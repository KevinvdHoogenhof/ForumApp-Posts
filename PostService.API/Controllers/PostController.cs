using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.API.Models;
using PostService.API.Services;

namespace PostService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _service;
        public PostController(IPostService service) =>
            _service = service;

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
        public async Task<ActionResult<Post>> Post(InsertPostDTO post) =>
            CreatedAtAction(nameof(Get), new
            {
                id = ((await _service.InsertPost(new Post { ThreadId = post.ThreadId, ThreadName = post.ThreadName, AuthorId = post.AuthorId, AuthorName = post.AuthorName, Name = post.Name, Content = post.Content, Comments = 0 }))?.Id)
                ?? throw new InvalidOperationException("Failed to insert the post.")
            }, post);

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<Post>> Update(string id, UpdatePostDTO post)
        {
            var p = await _service.GetPost(id);

            if (p is null)
            {
                return NotFound();
            }

            p.Name = post.Name;
            p.Content = post.Content;
            var po = await _service.UpdatePost(p);

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
