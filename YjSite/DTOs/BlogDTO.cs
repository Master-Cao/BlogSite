namespace YjSite.DTOs
{
    // 创建博客请求
    public class CreateBlogRequest
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string ContentHtml { get; set; }
        public string Tages { get; set; }
        public string CoverImage { get; set; }
        public int State { get; set; }
    }

    // 更新博客请求
    public class UpdateBlogRequest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string ContentHtml { get; set; }
        public string Tages { get; set; }
        public string CoverImage { get; set; }
        public int State { get; set; }
    }

    // 博客响应
    public class BlogResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string ContentHtml { get; set; }
        public string Tages { get; set; }
        public string CoverImage { get; set; }
        public string CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
        public int State { get; set; }
        public int Common { get; set; }
        public int View { get; set; }
    }

    // 博客列表响应
    public class BlogListResponse
    {
        public List<BlogResponse> Blogs { get; set; }
        public int Total { get; set; }
    }
} 