using Site.Domain.Entities;

namespace YjSite.Services.Files
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<List<SiteImages>> GetImagesByBlogIdAsync(string blogId);
        Task<bool> DeleteImageAsync(string imageId);
        Task<bool> AssociateImagesWithBlogAsync(string blogId, List<string> imageIds);
    }
} 