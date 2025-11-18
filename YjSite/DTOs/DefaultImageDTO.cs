namespace YjSite.DTOs
{
    // 创建默认图片请求
    public class CreateDefaultImageRequest
    {
        // 图片URL
        public string Url { get; set; }
    }

    // 更新默认图片请求
    public class UpdateDefaultImageRequest
    {
        // 图片ID
        public string Id { get; set; }
        
        // 图片URL
        public string Url { get; set; }
    }

    // 默认图片响应
    public class DefaultImageResponse
    {
        // 图片ID
        public string Id { get; set; }
        
        // 图片URL
        public string Url { get; set; }
        
        // 创建者ID
        public string CreateUserId { get; set; }
        
        // 创建时间
        public DateTime CreateTime { get; set; }
    }

    // 默认图片列表响应
    public class DefaultImageListResponse
    {
        // 默认图片列表
        public List<DefaultImageResponse> Images { get; set; }
        
        // 总数
        public int Total { get; set; }
    }
} 