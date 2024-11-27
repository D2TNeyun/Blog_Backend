using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.Comment;
using Src.Dtos.User;
using Src.Models;

namespace Src.Services
{
    public class CommentService(ApplicationDBContext context, UserManager<AppUser> userManager, IMapper mapper)
    {
        private readonly ApplicationDBContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<CommentsDto>> GetAllCmtAsync()
        {
            var comments = await _context.Comments
            .Include(c => c.AppUser)
            .ToListAsync();
            var commentsDtos = new List<CommentsDto>();
            foreach (var comment in comments)
            {
                commentsDtos.Add(new CommentsDto
                {
                    CommentId = comment.CommentId,
                    AppUserID = comment.AppUserID ?? throw new ArgumentNullException(nameof(comment.AppUserID)),
                    PostId = comment.PostId,
                    Content = comment.Content,
                    AppUser = _mapper.Map<UserDto>(await _userManager.FindByIdAsync(comment.AppUserID)),
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

        public async Task<Comment> UpdateCmtAsync(int id, UpdateCommentDto updateComment)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                throw new ArgumentException("Comment not found.");//+
            }
            comment.Content = updateComment.Content;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
        public async Task DeleteCmtAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                throw new ArgumentException("Comment not found.");
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync(); 
            return;
        }


    }
}