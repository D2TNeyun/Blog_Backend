using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.Category;
using Src.Dtos.Post;
using Src.Dtos.Tag;
using Src.Models;

namespace Src.Services
{
    public class TagService(ApplicationDBContext context, IMapper mapper)
    {
        public readonly ApplicationDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Tag> CreateTagAsync(CreateTagDto createTagDto)
        {
            var tag = new Tag
            {
                CategoryID = createTagDto.CategoryID,
                TagName = createTagDto.TagName,
            };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<IEnumerable<TagDto>> GetTagsAsync()
        {
            var tags = await _context.Tags.ToListAsync();
            var tagDtos = new List<TagDto>();
            foreach (var tag in tags)
            {
                tagDtos.Add(new TagDto
                {
                    TagID = tag.TagID,
                    TagName = tag.TagName,
                    CategoryID = tag.CategoryID,
                    // Include other properties if needed
                    Posts = _mapper.Map<List<PostDto>>(await _context.Posts.Where(p => p.TagID == tag.TagID).ToListAsync())
                });
            }
            return tagDtos;

        }

        public async Task<TagDto?> GetTagByIdAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return null;
            }

            var tagDto = new TagDto
            {
                TagID = tag.TagID,
                TagName = tag.TagName,
                CategoryID = tag.CategoryID,
                // Include other properties if needed
                Posts = _mapper.Map<List<PostDto>>(await _context.Posts.Where(p => p.TagID == tag.TagID).ToListAsync())

            };

            return tagDto;
        }

        public async Task<Tag?> UpdateTagAsync(int id, TagUpdateDto tagUpdate)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return null;
            }
            tag.TagName = tagUpdate.TagName;

            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return false;
            }
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}