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
                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

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

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
        {
            var posts = await _context.Posts.ToListAsync();
            var postDtos = new List<PostDto>();
            foreach (var post in posts)
            {
                var postDto = new PostDto
                {
                    PostID = post.PostID,
                    AppUserID = post.AppUserID ?? throw new ArgumentNullException(nameof(post.AppUserID)),
                    CategoryID = post.CategoryID,
                    TagID = post.TagID,
                    Title = post.Title,
                    Description = post.Description,
                    Content = post.Content,
                    PublishedDate = post.PublishedDate,
                    Views = post.Views,
                    Image = post.Image,
                    AppUser = _mapper.Map<UserDto>(await _userManager.FindByIdAsync(post.AppUserID)),
                    Category = _mapper.Map<CategoryDto>(await _context.Categories.FindAsync(post.CategoryID)),
                    Tag = _mapper.Map<TagDto>(await _context.Tags.FindAsync(post.TagID))
                };

                postDtos.Add(postDto);
            }

            return postDtos;
        }


        public async Task<PostDto?> GetPostByIdAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return null;
            }

            var postDto = new PostDto
            {
                PostID = post.PostID,
                AppUserID = post.AppUserID?? throw new ArgumentNullException(nameof(post.AppUserID)),
                CategoryID = post.CategoryID,
                TagID = post.TagID,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                PublishedDate = post.PublishedDate,
                Views = post.Views,
                Image = post.Image,
                AppUser = _mapper.Map<UserDto>(await _userManager.FindByIdAsync(post.AppUserID)),
                Category = _mapper.Map<CategoryDto>(await _context.Categories.FindAsync(post.CategoryID)),
                Tag = _mapper.Map<TagDto>(await _context.Tags.FindAsync(post.TagID))
            };

            return postDto;
        }
    }

}