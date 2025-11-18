using Site.Domain.Entities;
using YjSite.DTOs;

namespace YjSite.Services.BlogsService
{
    public interface IBlogService
    {
        Task<(List<BlogResponse> Blogs, int Total)> GetBlogsAsync(int page, int pageSize, string userId = null);
        Task<(List<BlogResponse> Blogs, int Total)> GetBlogsByViewCountAsync(int page, int pageSize, int minViewCount = 0);
        Task<(List<BlogResponse> Blogs, int Total)> GetBlogsByTagAsync(string tag, int page, int pageSize);
        Task<BlogResponse> GetBlogByIdAsync(string id);
        Task<BlogResponse> CreateBlogAsync(CreateBlogRequest request, string userId);
        Task<BlogResponse> UpdateBlogAsync(string id, UpdateBlogRequest request, string userId);
        Task<bool> DeleteBlogAsync(string id, string userId);
        Task<BlogResponse> PublishBlogAsync(CreateBlogRequest request, string userId);
    }
} 