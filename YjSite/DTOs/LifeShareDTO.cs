namespace YjSite.DTOs
{
    /// <summary>
    /// 创建生活分享请求
    /// </summary>
    public class CreateLifeShareRequest
    {
        /// <summary>
        /// 标题（最大25字符）
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 内容（Markdown格式）
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 封面图片URL
        /// </summary>
        public string? CoverImage { get; set; }

        /// <summary>
        /// 图片列表（JSON格式）
        /// </summary>
        public string? Images { get; set; }

        /// <summary>
        /// 分类：life/travel/food/thoughts/tech/other
        /// </summary>
        public string Category { get; set; } = "life";

        /// <summary>
        /// 标签（逗号分隔）
        /// </summary>
        public string? Tags { get; set; }
    }

    /// <summary>
    /// 更新生活分享请求
    /// </summary>
    public class UpdateLifeShareRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? CoverImage { get; set; }
        public string? Images { get; set; }
        public string Category { get; set; } = "life";
        public string? Tags { get; set; }
    }

    /// <summary>
    /// 生活分享响应
    /// </summary>
    public class LifeShareResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? CoverImage { get; set; }
        public string? Images { get; set; }
        public string Category { get; set; } = "life";
        public string? Tags { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorAvatar { get; set; }
        public string? CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 生活分享列表响应
    /// </summary>
    public class LifeShareListResponse
    {
        public List<LifeShareResponse> Shares { get; set; } = new();
        public int Total { get; set; }
    }
}

