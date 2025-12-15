using YjSite.DTOs;

namespace YjSite.Services.OssService
{
    /// <summary>
    /// OSS 服务接口
    /// </summary>
    public interface IOssService
    {
        /// <summary>
        /// 上传单个文件到 OSS
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="module">模块名称（用于分类存储）</param>
        /// <returns>上传结果</returns>
        Task<UploadFileResponse> UploadFileAsync(IFormFile file, string module = "lifeshare");

        /// <summary>
        /// 上传多个文件到 OSS
        /// </summary>
        /// <param name="files">文件列表</param>
        /// <param name="module">模块名称</param>
        /// <returns>上传结果列表</returns>
        Task<List<UploadFileResponse>> UploadFilesAsync(List<IFormFile> files, string module = "lifeshare");

        /// <summary>
        /// 获取 OSS 域名（用于前端拼接URL）
        /// </summary>
        /// <returns>域名</returns>
        string GetDomain();
    }
}

