using YjSite.DTOs;

namespace YjSite.Services.DefaultImageService
{
    public interface IDefaultImageService
    {
        /// <summary>
        /// 获取默认图片列表
        /// </summary>
        Task<(List<DefaultImageResponse>, int)> GetDefaultImagesAsync(int page, int pageSize);
        
        /// <summary>
        /// 根据ID获取默认图片
        /// </summary>
        Task<DefaultImageResponse> GetDefaultImageByIdAsync(string id);
        
        /// <summary>
        /// 创建默认图片
        /// </summary>
        Task<DefaultImageResponse> CreateDefaultImageAsync(CreateDefaultImageRequest request, string userId);
        
        /// <summary>
        /// 更新默认图片
        /// </summary>
        Task<DefaultImageResponse> UpdateDefaultImageAsync(string id, UpdateDefaultImageRequest request, string userId);
        
        /// <summary>
        /// 删除默认图片
        /// </summary>
        Task<bool> DeleteDefaultImageAsync(string id, string userId);
    }
} 