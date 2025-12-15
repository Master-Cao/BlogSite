using YjSite.DTOs;

namespace YjSite.Services.LifeShareService
{
    /// <summary>
    /// 生活分享服务接口
    /// </summary>
    public interface ILifeShareService
    {
        /// <summary>
        /// 获取生活分享列表（分页）
        /// </summary>
        Task<(List<LifeShareResponse> Shares, int Total)> GetSharesAsync(int page, int pageSize, string? userId = null);

        /// <summary>
        /// 按分类获取生活分享列表
        /// </summary>
        Task<(List<LifeShareResponse> Shares, int Total)> GetSharesByCategoryAsync(string category, int page, int pageSize);

        /// <summary>
        /// 按浏览量获取热门分享
        /// </summary>
        Task<(List<LifeShareResponse> Shares, int Total)> GetSharesByViewCountAsync(int page, int pageSize, int minViewCount = 0);

        /// <summary>
        /// 根据ID获取分享详情
        /// </summary>
        Task<LifeShareResponse?> GetShareByIdAsync(string id);

        /// <summary>
        /// 创建生活分享（支持匿名投递，userId可为null）
        /// </summary>
        Task<LifeShareResponse?> CreateShareAsync(CreateLifeShareRequest request, string? userId);

        /// <summary>
        /// 更新生活分享
        /// </summary>
        Task<LifeShareResponse?> UpdateShareAsync(string id, UpdateLifeShareRequest request, string userId);

        /// <summary>
        /// 删除生活分享
        /// </summary>
        Task<bool> DeleteShareAsync(string id, string userId);

        /// <summary>
        /// 增加浏览量
        /// </summary>
        Task<bool> IncrementViewCountAsync(string id);

        /// <summary>
        /// 点赞/取消点赞
        /// </summary>
        Task<bool> ToggleLikeAsync(string id, bool isLike);
    }
}

