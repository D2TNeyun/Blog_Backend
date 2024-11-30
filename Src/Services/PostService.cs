using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.Category;
using Src.Dtos.Comment;
using Src.Dtos.Post;
using Src.Dtos.Tag;
using Src.Dtos.User;
using Src.Interface;
using Src.Models;

namespace Src.Services
{
    public class PostService(Cloudinary cloudinary, ApplicationDBContext context, UserManager<AppUser> userManager, IMapper mapper)
    {
        private readonly Cloudinary _cloudinary = cloudinary;
        private readonly ApplicationDBContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        public async Task<PostDto> CreatePostAsync(PostDto postDto, IFormFile image)
        {
            // Kiểm tra nếu không có AppUserID hoặc CategoryID hợp lệ
            if (_context.Categories == null || _context.Posts == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            var appUser = await _userManager.FindByIdAsync(postDto.AppUserID);
            var category = await _context.Categories.FindAsync(postDto.CategoryID);


            if (appUser == null || category == null)
            {
                throw new ArgumentException("Invalid AppUserID or CategoryID.");
            }

            // Tạo thực thể Post từ PostDto
            var post = new Post
            {
                AppUserID = postDto.AppUserID,
                CategoryID = postDto.CategoryID,
                TagID = postDto.TagID,
                Title = postDto.Title,
                Description = postDto.Description,
                Content = postDto.Content,
                PublishedDate = DateTime.UtcNow,
                Views = 0
            };

            // Thêm post vào cơ sở dữ liệu nhưng chưa lưu
            _context.Posts.Add(post);

            try
            {
                // Nếu có ảnh, tải ảnh lên Cloudinary
                if (image != null)
                {
                    var uploadResult = new ImageUploadResult();

                    using (var stream = image.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(image.FileName, stream),
                            Folder = "BlogProject",
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("center")
                        };

                        uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    }

                    // Lưu đường dẫn ảnh vào cơ sở dữ liệu
                    post.Image = uploadResult.SecureUrl.AbsoluteUri;
                    await _context.SaveChangesAsync();
                }
                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Trả về PostDto thay vì đối tượng Post đầy đủ
                var createdPostDto = _mapper.Map<PostDto>(post);
                return createdPostDto;
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, xóa post vừa thêm và không tải ảnh lên
                if (post.PostID != 0)
                {
                    _context.Posts.Remove(post);
                    await _context.SaveChangesAsync();
                    // Xóa ảnh từ Cloudinary
                    await _cloudinary.DeleteUploadMappingAsync(post?.Image?.Split('/').Last());
                }
                throw new Exception("Error creating post: " + ex.Message);
            }
        }

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync(PostQuery postQuery)
        {
            if (_context.Categories == null || _context.Posts == null || _context.Tags == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            var posts = await _context.Posts.ToListAsync();
            if (!string.IsNullOrWhiteSpace(postQuery.Title))
            {
                posts = posts.Where(p => p.Title.ToLower().Contains(postQuery.Title.ToLower())).ToList();
            }
            var postDtos = new List<PostDto>();
            foreach (var post in posts)
            {
                postDtos.Add(new PostDto
                {
                    PostID = post.PostID,
                    AppUserID = post.AppUserID ?? throw new ArgumentNullException(nameof(post.AppUserID)),
                    CategoryID = post.CategoryID,
                    TagID = post.TagID,
                    Title = post.Title,
                    Description = post.Description ?? string.Empty,
                    Content = post.Content ?? string.Empty,
                    PublishedDate = post.PublishedDate,
                    Views = post.Views,
                    Image = post.Image ?? string.Empty,
                    AppUser = _mapper.Map<UserDto>(await _userManager.FindByIdAsync(post.AppUserID)),
                    Category = _mapper.Map<CategoryDto>(await _context.Categories.FindAsync(post.CategoryID)),
                    Tag = _mapper.Map<TagDto>(await _context.Tags.FindAsync(post.TagID))


                });
            }

            return postDtos;
        }


        public async Task<PostDto?> GetPostByIdAsync(int postId)
        {
            if (_context.Comments == null || _context.Categories == null || _context.Posts == null || _context.Tags == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return null;
            }

            var postDto = new PostDto
            {
                PostID = post.PostID,
                AppUserID = post.AppUserID ?? throw new ArgumentNullException(nameof(post.AppUserID)),
                CategoryID = post.CategoryID,
                TagID = post.TagID,
                Title = post.Title,
                Description = post.Description ?? string.Empty,
                Content = post.Content ?? string.Empty,
                PublishedDate = post.PublishedDate,
                Views = post.Views,
                Image = post.Image ?? string.Empty,
                AppUser = _mapper.Map<UserDto>(await _userManager.FindByIdAsync(post.AppUserID)),
                Category = _mapper.Map<CategoryDto>(await _context.Categories.FindAsync(post.CategoryID)),
                Tag = _mapper.Map<TagDto>(await _context.Tags.FindAsync(post.TagID)),
                // Comments = _mapper.Map<CommentsDto>(await _context.Comments.FindAsync(post.Comments)),//-
                Comments = _mapper.Map<List<CommentsDto>>(await _context.Comments.Where(c => c.PostId == post.PostID).ToListAsync()),//+
            };

            return postDto;
        }

        public async Task<PostDto> UpdatePostAsync(int postId, PostUpdateDto postDto, IFormFile? image)
        {
            // Tìm post cần cập nhật
            if (_context.Posts == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                throw new ArgumentException("Post not found.");
            }
            // Chỉ cập nhật các trường nếu có dữ liệu mới không rỗng hoặc null
            post.CategoryID = postDto.CategoryID ?? post.CategoryID;
            post.TagID = postDto.TagID ?? post.TagID;
            post.Title = !string.IsNullOrEmpty(postDto.Title) ? postDto.Title : post.Title;
            post.Description = !string.IsNullOrEmpty(postDto.Description) ? postDto.Description : post.Description;
            post.Content = !string.IsNullOrEmpty(postDto.Content) ? postDto.Content : post.Content;

            try
            {
                if (image != null)
                {
                    if (!string.IsNullOrEmpty(post.Image))
                    {
                        await _cloudinary.DeleteUploadMappingAsync(post.Image.Split('/').Last());
                    }

                    var uploadResult = new ImageUploadResult();

                    using (var stream = image.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(image.FileName, stream),
                            Folder = "BlogProject",
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("center")
                        };

                        uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    }

                    post.Image = uploadResult.SecureUrl.AbsoluteUri;
                }

                await _context.SaveChangesAsync();

                var updatedPostDto = _mapper.Map<PostDto>(post);
                return updatedPostDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating post: " + ex.Message);
            }
        }


        public async Task DeletePostAsync(int postId)
        {
            // Tìm post cần xóa
            if (_context.Posts == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            var post = await _context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.PostID == postId);
            if (post == null)
            {
                throw new ArgumentException("Post not found.");
            }

            // Xóa ảnh trên Cloudinary nếu tồn tại
            if (!string.IsNullOrEmpty(post.Image))
            {
                await _cloudinary.DeleteUploadMappingAsync(post.Image.Split('/').Last());
            }

            // Xóa tất cả bình luận liên quan
            if (_context.Comments == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            var comments = await _context.Comments.Where(c => c.PostId == postId).ToListAsync();
            _context.Comments.RemoveRange(comments);

            // Xóa post vào cơ sở dữ liệu
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }

}