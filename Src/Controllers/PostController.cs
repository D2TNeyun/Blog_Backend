using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.Post;
using Src.Interface;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/post")]

    public class PostController(PostService postService, ApplicationDBContext context) : ControllerBase
    {
        private readonly PostService _postService = postService;
        private readonly ApplicationDBContext _context = context;
        // private readonly IPostService _postServices = postServices;

        [Authorize(Roles = "Admin,Employee")]

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

        [Authorize(Roles = "Admin,Employee")]
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

        [Authorize(Roles = "Admin,Employee")]
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

        [AllowAnonymous]
        [HttpPost("increment-view/{postId}")]
        public async Task<IActionResult> IncrementPostView(int postId)
        {
            var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var now = DateTime.UtcNow;

            // Kiểm tra bài viết
            if (_context.PostViewHistories == null || _context.Posts == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostID == postId);
            if (post == null)
                return NotFound(new { Message = "Post not found." });

            var existingView = await _context.PostViewHistories
                .FirstOrDefaultAsync(pv => pv.PostId == postId &&
                    (pv.UserId == userId || pv.IPAddress == ipAddress) &&
                    EF.Functions.DateDiffMinute(pv.ViewedAt, now) < 10); // Giới hạn 10p            

            if (existingView != null)
            {
                return Ok(new { Message = "View already counted recently." });
            }

            // Tăng view cho bài viết
            post.Views += 1;
            _context.Posts.Update(post);

            // Ghi lại lịch sử xem bài viết
            _context.PostViewHistories.Add(new PostViewHistory
            {
                PostId = postId,
                UserId = userId ?? string.Empty,
                IPAddress = ipAddress ?? string.Empty,
                ViewedAt = now
            });

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Post view incremented successfully.", Views = post.Views });
        }

        //GetAllviewHistories
        [HttpGet("viewHistories/all")]
        public async Task<ActionResult<IEnumerable<PostViewHistory>>> GetAllPostViewHistories()
        {
            if (_context.PostViewHistories == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            var viewHistories = await _context.PostViewHistories.ToListAsync();
            return Ok(new { viewHistories });
        }

    }

}
