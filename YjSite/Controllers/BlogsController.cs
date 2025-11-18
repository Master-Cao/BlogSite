using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Site.Domain.Entities;
using YjSite.DTOs;
using YjSite.Helpers;
using YjSite.Services.BlogsService;
using YjSite.ViewModel;

namespace YjSite.Controllers;

public class BlogsController : BaseController
{
    private readonly IBlogService _blogService;

    public BlogsController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    /// <summary>
    /// 获取博客列表
    /// </summary>
    [HttpGet("blogs")]
    public async Task<IActionResult> GetBlogs([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string userId = null)
    {
        var (blogs, total) = await _blogService.GetBlogsAsync(page, pageSize, userId);
        
        // 将实体转换为响应DTO
        var response = new BlogListResponse
        {
            Blogs = blogs,
            Total = total
        };
        
        return Ok(JsonView(response));
    }

    /// <summary>
    /// 获取热门博客列表（按观看量排序）
    /// </summary>
    [HttpGet("blogs/popular")]
    public async Task<IActionResult> GetPopularBlogs([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int minViewCount = 0)
    {
        var (blogs, total) = await _blogService.GetBlogsByViewCountAsync(page, pageSize, minViewCount);
        
        // 将实体转换为响应DTO
        var response = new BlogListResponse
        {
            Blogs = blogs,
            Total = total
        };
        
        return Ok(JsonView(response));
    }

    /// <summary>
    /// 根据标签获取博客列表
    /// </summary>
    [HttpGet("blogs/tag/{tag}")]
    public async Task<IActionResult> GetBlogsByTag(string tag, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (blogs, total) = await _blogService.GetBlogsByTagAsync(tag, page, pageSize);
        
        // 将实体转换为响应DTO
        var response = new BlogListResponse
        {
            Blogs = blogs,
            Total = total
        };
        
        return Ok(JsonView(response));
    }

    /// <summary>
    /// 获取单个博客详情
    /// </summary>
    [HttpGet("blog/{id}")]
    public async Task<IActionResult> GetBlogById(string id)
    {
        var blog = await _blogService.GetBlogByIdAsync(id);
        if (blog == null)
        {
            return NotFound(JsonView("博客不存在"));
        }
        
        return Ok(JsonView(blog));
    }

    /// <summary>
    /// 创建新博客
    /// </summary>
    [HttpPost("blog")]
    [Authorize]
    public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest request)
    {
        var userId = UserHelper.GetCurrentUserId(User);
        var blogResponse = await _blogService.CreateBlogAsync(request, userId);
        
        if (blogResponse != null)
        {
            return Created($"/api/blog/{blogResponse.Id}", JsonView(blogResponse));
        }
        
        return BadRequest(JsonView("创建博客失败"));
    }

    /// <summary>
    /// 更新已有博客
    /// </summary>
    [HttpPut("blog/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateBlog(string id, [FromBody] UpdateBlogRequest request)
    {
        var blog = await _blogService.GetBlogByIdAsync(id);
        if (blog == null)
        {
            return NotFound(JsonView("博客不存在"));
        }

        var userId = UserHelper.GetCurrentUserId(User);
        if (blog.CreateUserId != userId)
        {
            return Forbid();
        }

        var updatedBlog = await _blogService.UpdateBlogAsync(id, request, userId);
        
        if (updatedBlog != null)
        {
            return Ok(JsonView(updatedBlog));
        }
        
        return BadRequest(JsonView("更新博客失败"));
    }

    /// <summary>
    /// 删除博客
    /// </summary>
    [HttpDelete("blog/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteBlog(string id)
    {
        var blog = await _blogService.GetBlogByIdAsync(id);
        if (blog == null)
        {
            return NotFound(JsonView("博客不存在"));
        }

        var userId = UserHelper.GetCurrentUserId(User);
        if (blog.CreateUserId != userId)
        {
            return Forbid();
        }

        var success = await _blogService.DeleteBlogAsync(id, userId);
        
        if (success)
        {
            return Ok(JsonView(true, "删除成功"));
        }
        
        return BadRequest(JsonView("删除博客失败"));
    }

    /// <summary>
    /// 发布博客
    /// </summary>
    [HttpPost("publish/blog")]
    [Authorize]
    public async Task<IActionResult> PublishBlog([FromBody] CreateBlogRequest request)
    {
        var userId = UserHelper.GetCurrentUserId(User);
        var blogResponse = await _blogService.PublishBlogAsync(request, userId);
        
        if (blogResponse != null)
        {
            return Ok(JsonView(blogResponse));
        }
        
        return BadRequest(JsonView("发布博客失败"));
    }
}