using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.Comment;
using Src.Models;

namespace Src.Services
{
    public class CommentService(ApplicationDBContext context, UserManager<AppUser> userManager)
    {
        private readonly ApplicationDBContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;

        // public async Task<Comment> CreateCommentAsync(CreateCommentDto createCommentDto)
        // {
        //     var user = await _userManager.FindByIdAsync(createCommentDto.UserID);
        //     var post = await _context.Posts.FindAsync(createCommentDto.PostID);

        //     if (user == null || post == null)
        //     {
        //         throw new InvalidOperationException("User or post not found.");
        //     }

        //     var comment = new Comment
        //     {
        //         Content = createCommentDto.Content,
        //         User = user,
        //         Post = post,
        //         // CreatedAt = DateTime.Now
        //     }

        // }

        public async Task<IEnumerable<CommentsDto>> GetAllCmtAsync()
        {
            var comments = await _context.Comments.ToListAsync();
            var commentsDtos = new List<CommentsDto>();
            foreach (var comment in comments)
            {
                commentsDtos.Add(new CommentsDto
                {
                    CommentId = comment.CommentId,
                    AppUserID = comment.AppUserID ?? throw new ArgumentNullException(nameof(comment.AppUserID)),
                    PostId = comment.PostId,
                    Content = comment.Content,
                });
            }

            return commentsDtos;
        }

        public async Task<CommentsDto?> GetCmtByIdAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }

            var cmtDto = new CommentsDto
            {
                CommentId = comment.CommentId,
                AppUserID = comment.AppUserID ?? throw new ArgumentNullException(nameof(comment.AppUserID)),
                PostId = comment.PostId,
                Content = comment.Content,
            };

            return cmtDto;
        }

        public async Task<CommentsDto> CreateCmtAsync(CommentsDto comments)
        {
            // Check if the user exists
            var appUser = await _userManager.FindByIdAsync(comments.AppUserID);

            // Check if the post exists
            var post = await _context.Posts.FindAsync(comments.PostId);

            if (appUser == null || post == null)
            {
                throw new ArgumentException("Invalid AppUserID or PostID.");
            }

            try
            {
                // Create a new comment
                var comment = new Comment
                {
                    AppUserID = comments.AppUserID,
                    PostId = comments.PostId,
                    Content = comments.Content,
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync(); // Save changes
            }
            catch (DbUpdateException dbEx)
            {
                // Log or inspect the inner exception for debugging
                var innerException = dbEx.InnerException?.Message;
                throw new Exception($"Database update error: {innerException}");
            }
            catch (Exception ex)
            {
                // Log the general exception
                throw new Exception($"An error occurred: {ex.Message}");
            }

            return comments; // Return the DTO with comment information
        }


        // public async Task<CommentsDto> UpdateCmtAsync(int id, UpdateCommentDto updateCommentDto)
        // {0e502108-1b84-42d0-b996-8475338b7972}


    }
}