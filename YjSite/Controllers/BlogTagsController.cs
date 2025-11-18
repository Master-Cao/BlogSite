using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Site.Domain.Entities;
using YjSite.DTOs;
using YjSite.Helpers;
using YjSite.Services.BlogTagsService;
using YjSite.ViewModel;

namespace YjSite.Controllers;

public class BlogTagsController : BaseController
{
    private readonly IBlogTagsService _blogTagsService;

    public BlogTagsController(IBlogTagsService blogTagsService)
    {
        _blogTagsService = blogTagsService;
    }

    /// <summary>
    /// 获取博客标签列表
    /// </summary>
    [HttpGet("blog/tags")]
    public async Task<IActionResult> GetBlogTags([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (tags, total) = await _blogTagsService.GetBlogTagsAsync(page, pageSize);
        
        // 将实体转换为响应DTO
        var response = new BlogTagListResponse
        {
            Tags = tags,
            Total = total
        };
        
        return Ok(JsonView(response));
    }

    /// <summary>
    /// 获取单个博客标签详情
    /// </summary>
    [HttpGet("blog/tag/{id}")]
    public async Task<IActionResult> GetBlogTagById(string id)
    {
        var tag = await _blogTagsService.GetBlogTagByIdAsync(id);
        if (tag == null)
        {
            return NotFound(JsonView("标签不存在"));
        }
        
        return Ok(JsonView(tag));
    }

    /// <summary>
    /// 创建新博客标签
    /// </summary>
    [HttpPost("blog/tag")]
    [Authorize]
    public async Task<IActionResult> CreateBlogTag([FromBody] CreateBlogTagRequest request)
    {
        var userId = UserHelper.GetCurrentUserId(User);
        var tagResponse = await _blogTagsService.CreateBlogTagAsync(request, userId);
        
        if (tagResponse != null)
        {
            return Created($"/api/blog/tag/{tagResponse.Id}", JsonView(tagResponse));
        }
        
        return BadRequest(JsonView("创建标签失败"));
    }

    /// <summary>
    /// 更新已有博客标签
    /// </summary>
    [HttpPut("blog/tag/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateBlogTag(string id, [FromBody] UpdateBlogTagRequest request)
    {
        var tag = await _blogTagsService.GetBlogTagByIdAsync(id);
        if (tag == null)
        {
            return NotFound(JsonView("标签不存在"));
        }

        var userId = UserHelper.GetCurrentUserId(User);
        var updatedTag = await _blogTagsService.UpdateBlogTagAsync(id, request, userId);
        
        if (updatedTag != null)
        {
            return Ok(JsonView(updatedTag));
        }
        
        return BadRequest(JsonView("更新标签失败"));
    }

    /// <summary>
    /// 删除博客标签
    /// </summary>
    [HttpDelete("blog/tag/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteBlogTag(string id)
    {
        var tag = await _blogTagsService.GetBlogTagByIdAsync(id);
        if (tag == null)
        {
            return NotFound(JsonView("标签不存在"));
        }

        var userId = UserHelper.GetCurrentUserId(User);
        var success = await _blogTagsService.DeleteBlogTagAsync(id, userId);
        
        if (success)
        {
            return Ok(JsonView(true, "删除成功"));
        }
        
        return BadRequest(JsonView("删除标签失败"));
    }
} 