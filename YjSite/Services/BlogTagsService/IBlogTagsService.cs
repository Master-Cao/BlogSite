using Site.Domain.Entities;
using YjSite.DTOs;

namespace YjSite.Services.BlogTagsService
{
    public interface IBlogTagsService
    {
        Task<(List<BlogTagResponse> Tags, int Total)> GetBlogTagsAsync(int page, int pageSize);
        Task<BlogTagResponse> GetBlogTagByIdAsync(string id);
        Task<BlogTagResponse> CreateBlogTagAsync(CreateBlogTagRequest request, string userId);
        Task<BlogTagResponse> UpdateBlogTagAsync(string id, UpdateBlogTagRequest request, string userId);
        Task<bool> DeleteBlogTagAsync(string id, string userId);
    }
} 