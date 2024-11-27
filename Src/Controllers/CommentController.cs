using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Src.Dtos.Comment;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/comments")]
    [ApiController]
    // [Authorize]
    public class CommentController(CommentService commentService) : ControllerBase
    {
        private readonly CommentService _commentService = commentService;

        [HttpGet]
        public async Task<ActionResult<List<Comment>>> GetComments()
        {
            var comments = await _commentService.GetAllCmtAsync();
            return Ok(new { comments });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetCommentById(int id)
        {
            var comment = await _commentService.GetCmtByIdAsync(id);
            if (comment == null)
            {
                return NotFound("Comment not found");
            }
            return Ok(new { comment });
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromForm] CreateCmtDto createCmt)
        {
            try
            {
                var cmtDto = new CommentsDto
                {
                    Content = createCmt.Content,
                    PostId = createCmt.PostId,
                    AppUserID = createCmt.AppUserID
                };
                var comment = await _commentService.CreateCmtAsync(cmtDto);

                return Ok(new { comment });
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            try
            {
                await _commentService.DeleteCmtAsync(id);
                return Ok(new { message = "Delete successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromForm] UpdateCommentDto updateCmt)
        {
            var updatedCmt = await _commentService.UpdateCmtAsync(id, updateCmt);
            if (updatedCmt == null)
            {
                return NotFound("Comment not found");
            }
            return Ok(updatedCmt);
        }

    }
}
