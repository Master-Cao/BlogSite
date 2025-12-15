using Site.Domain.Entities;
using YjSite.DTOs;

namespace YjSite.Helpers.Mapping
{
    /// <summary>
    /// 生活分享实体映射扩展方法
    /// </summary>
    public static class LifeShareMapping
    {
        /// <summary>
        /// 将实体转换为响应DTO
        /// </summary>
        public static LifeShareResponse ToLifeShareResponse(this LifeShares entity)
        {
            return new LifeShareResponse
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                CoverImage = entity.CoverImage,
                Images = entity.Images,
                Category = entity.Category,
                Tags = entity.Tags,
                ViewCount = entity.ViewCount,
                LikeCount = entity.LikeCount,
                AuthorName = entity.AuthorName,
                AuthorAvatar = entity.AuthorAvatar,
                CreateUserId = entity.CreateUserId,
                CreateTime = entity.CreateTime
            };
        }

        /// <summary>
        /// 将创建请求转换为实体（支持匿名投递，userId可为null）
        /// </summary>
        public static LifeShares ToLifeShareEntity(this CreateLifeShareRequest request, string? userId, string? authorName = null, string? authorAvatar = null)
        {
            return new LifeShares
            {
                Title = request.Title,
                Content = request.Content,
                CoverImage = request.CoverImage,
                Images = request.Images,
                Category = request.Category,
                Tags = request.Tags,
                CreateUserId = userId,
                AuthorName = authorName,
                AuthorAvatar = authorAvatar,
                ViewCount = 0,
                LikeCount = 0
            };
        }

        /// <summary>
        /// 将更新请求应用到实体
        /// </summary>
        public static void UpdateEntity(this UpdateLifeShareRequest request, LifeShares entity)
        {
            entity.Title = request.Title;
            entity.Content = request.Content;
            entity.CoverImage = request.CoverImage;
            entity.Images = request.Images;
            entity.Category = request.Category;
            entity.Tags = request.Tags;
        }
    }
}

