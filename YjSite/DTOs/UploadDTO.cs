namespace YjSite.DTOs
{
    // 文件上传响应
    public class UploadFileResponse
    {
        // 完整的文件URL
        public string Url { get; set; }
        
        // OSS对象名称（文件名）
        public string FileName { get; set; }
    }
    
    // 多文件上传响应
    public class MultiUploadFileResponse
    {
        // 上传文件列表
        public List<UploadFileResponse> Files { get; set; }
    }
} 