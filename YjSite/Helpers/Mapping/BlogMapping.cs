using Site.Domain.Entities;
using YjSite.DTOs;

namespace YjSite.Helpers.Mapping
{
    public static class BlogMapping
    {
        // 将实体转换为响应DTO
        public static BlogResponse ToBlogResponse(this Blogs blog)
        {
            return new BlogResponse
            {
                Id = blog.Id,
                Title = blog.Title,
                Summary = blog.Summary,
                Content = blog.Content,
                ContentHtml = blog.ContentHtml,
                Tages = blog.Tages,
                CoverImage = blog.CoverImage,
                CreateUserId = blog.CreateUserId,
                CreateTime = blog.CreateTime,
                State = blog.State,
                Common = blog.Common,
                View = blog.View
            };
        }

        // 将请求DTO转换为实体
        public static Blogs ToBlogEntity(this CreateBlogRequest request, string userId)
        {
            return new Blogs
            {
                Title = request.Title,
                Summary = request.Summary,
                Content = request.Content,
                ContentHtml = request.ContentHtml,
                Tages = request.Tages,
                CoverImage = request.CoverImage,
                CreateUserId = userId,
                State = request.State, // 默认为草稿状态
                Common = 0,
                View = 0
            };
        }

        // 将请求DTO更新实体
        public static void UpdateEntity(this UpdateBlogRequest request, Blogs blog)
        {
            blog.Title = request.Title;
            blog.Summary = request.Summary;
            blog.Content = request.Content;
            blog.ContentHtml = request.ContentHtml;
            blog.Tages = request.Tages;
            blog.CoverImage = request.CoverImage;
            blog.State = request.State;
        }
    }
} 