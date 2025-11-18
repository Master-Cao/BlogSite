namespace YjSite.DTOs
{
    // 创建博客标签请求
    public class CreateBlogTagRequest
    {
        public string TagName { get; set; }
        public string SubTagName { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }

    // 更新博客标签请求
    public class UpdateBlogTagRequest
    {
        public string Id { get; set; }
        public string TagName { get; set; }
        public string SubTagName { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }

    // 博客标签响应
    public class BlogTagResponse
    {
        public string Id { get; set; }
        public string TagName { get; set; }
        public string SubTagName { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }

    // 博客标签列表响应
    public class BlogTagListResponse
    {
        public List<BlogTagResponse> Tags { get; set; }
        public int Total { get; set; }
    }
} 