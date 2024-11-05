using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Src.Dtos.Post;
using Src.Interface;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/post")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class PostController(PostService postService) : ControllerBase
    {
        private readonly PostService _postService = postService;
        // private readonly IPostService _postServices = postServices;


        [HttpPost("create")]
        public async Task<ActionResult<Post>> CreatePost([FromForm] CreatePostDto createPostDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var postDto = new PostDto
                {
                    AppUserID = createPostDto.AppUserID,
                    CategoryID = createPostDto.CategoryID,
                    TagID = createPostDto.TagID,
                    Description = createPostDto.Description ?? string.Empty,
                    Title = createPostDto.Title,
                    Content = createPostDto.Content ?? string.Empty,
                    PublishedDate = DateTime.Now,
                    Views = 0
                };
                var createdPost = await _postService.CreatePostAsync(postDto, createPostDto.Image);
                return Ok(createdPost);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetAllPosts([FromQuery] PostQuery postQuery)
        {
            var posts = await _postService.GetAllPostsAsync(postQuery);
            return Ok(new { posts });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPostById(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound("Post not found");
            }
            return Ok(new { post });
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePost(int id, [FromForm] PostUpdateDto postUpdate, IFormFile? Image)
        {
            try
            {
                var updatedPost = await _postService.UpdatePostAsync(id, postUpdate, Image);
                return Ok(new { message = "Update successfully", updatedPost });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating post: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost(int id)
        {
            try
            {
                await _postService.DeletePostAsync(id);
                return Ok(new { message = "Delete successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}
