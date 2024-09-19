using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Src.Dtos.Tag;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class TagController(TagService tagService) : ControllerBase
    {
        public readonly TagService _tagService = tagService;

        [HttpPost]
        public async Task<ActionResult> CreateTag([FromForm] CreateTagDto createTagDto)
        {
            var newTag = await _tagService.CreateTagAsync(createTagDto);
            return Ok(newTag);
        }

        [HttpGet]
        public async Task<ActionResult<List<Tag>>> GetTags()
        {
            var tags = await _tagService.GetTagsAsync();
            return Ok(new {tags});
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
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTag(int id,[FromForm] TagDto tagDto)
        {
            var UpdateTag = await _tagService.UpdateTagAsync(id, tagDto );
            if(UpdateTag == null)
            {
                return NotFound();
            }
            return Ok(UpdateTag);
        }
    }
}