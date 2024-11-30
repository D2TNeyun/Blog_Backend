using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Src.Dtos.Tag;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/tag")]
    public class TagController(TagService tagService) : ControllerBase
    {
        public readonly TagService _tagService = tagService;

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag([FromForm] CreateTagDto createTagDto)
        {
            // Tạo tag mới thông qua dịch vụ
            var newTag = await _tagService.CreateTagAsync(createTagDto);

            // Chuyển đổi đối tượng Tag thành TagDto
            var tagDto = new TagDto
            {
                TagID = newTag.TagID,
                TagName = newTag.TagName,
                CategoryID = newTag.CategoryID
            };

            // Trả về TagDto thay vì Tag
            return Ok(tagDto);
        }


        [HttpGet]
        public async Task<ActionResult<List<Tag>>> GetTags()
        {
            var tags = await _tagService.GetTagsAsync();
            return Ok(new { tags });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTagById(int id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            if (tag == null)
            {
                return NotFound("Tag not found");
            }
            return Ok(new { tag });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTag(int id, [FromForm] TagUpdateDto tagUpdate)
        {
            var UpdateTag = await _tagService.UpdateTagAsync(id, tagUpdate);
            if (UpdateTag == null)
            {
                return NotFound();
            }
            return Ok(UpdateTag);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTag(int id)
        {
            var tag = await _tagService.DeleteTagAsync(id);
            if (!tag)
            {
                return NotFound();
            }
            return Ok(new { message = "Tag deleted successfully" });
        }
    }
}