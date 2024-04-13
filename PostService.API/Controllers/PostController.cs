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
            await _service.GetPosts(); //Also add get for all posts under thread later, or change this to that

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
        public async Task<IActionResult> Post(Post post) // Make sure to also ask for thread id and account id
        {
            await _service.InsertPost(post);
            return CreatedAtAction(nameof(Get), new { id = post.Id }, post);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Post post)
        {
            var p = await _service.GetPost(id);

            if (p is null)
            {
                return NotFound();
            }

            await _service.UpdatePost(post);

            return NoContent();
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
