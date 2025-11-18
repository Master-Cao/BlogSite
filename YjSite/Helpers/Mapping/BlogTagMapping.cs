using Site.Domain.Entities;
using YjSite.DTOs;

namespace YjSite.Helpers.Mapping
{
    public static class BlogTagMapping
    {
        // 将博客标签实体转换为响应DTO
        public static BlogTagResponse ToBlogTagResponse(this BlogTags tag)
        {
            return new BlogTagResponse
            {
                Id = tag.Id,
                TagName = tag.TagName,
                SubTagName = tag.SubTagName,
                Icon = tag.Icon,
                Color = tag.Color,
                CreateUserId = tag.CreateUserId,
                CreateTime = tag.CreateTime
            };
        }

        // 将请求DTO转换为博客标签实体
        public static BlogTags ToBlogTagEntity(this CreateBlogTagRequest request, string userId)
        {
            return new BlogTags
            {
                TagName = request.TagName,
                SubTagName = request.SubTagName,
                Icon = request.Icon,
                Color = request.Color,
                CreateUserId = userId
            };
        }

        // 将请求DTO更新博客标签实体
        public static void UpdateEntity(this UpdateBlogTagRequest request, BlogTags tag)
        {
            tag.TagName = request.TagName;
            tag.SubTagName = request.SubTagName;
            tag.Icon = request.Icon;
            tag.Color = request.Color;
        }
    }
} 