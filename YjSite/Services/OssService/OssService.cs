using Aliyun.OSS;
using Microsoft.Extensions.Options;
using YjSite.Config;
using YjSite.DTOs;

namespace YjSite.Services.OssService
{
    /// <summary>
    /// OSS 服务实现
    /// </summary>
    public class OssService : IOssService
    {
        private readonly OssConfiguration _config;
        private readonly ILogger<OssService> _logger;

        public OssService(IOptions<OssConfiguration> config, ILogger<OssService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task<UploadFileResponse> UploadFileAsync(IFormFile file, string module = "lifeshare")
        {
            try
            {
                var endpoint = _config.EndPoint;
                var accessKeyId = _config.AccessKeyID;
                var accessKeySecret = _config.AccessKeySecret;
                var bucket = _config.Bucket;
                var domain = _config.Domain;

                var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

                // 生成唯一文件名
                string originalFileName = Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(originalFileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string randomSuffix = new Random().Next(1000, 9999).ToString();
                var objectKey = $"{module}/{timestamp}_{randomSuffix}{extension}";

                using (var stream = file.OpenReadStream())
                {
                    client.PutObject(bucket, objectKey, stream);
                }

                // 生成访问URL
                string url = $"{domain}/{objectKey}";

                _logger.LogInformation($"Uploaded file to OSS: {objectKey}");

                return await Task.FromResult(new UploadFileResponse
                {
                    Url = url,
                    FileName = objectKey
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OSS upload failed: {ex.Message}");
                throw;
            }
        }

        public async Task<List<UploadFileResponse>> UploadFilesAsync(List<IFormFile> files, string module = "lifeshare")
        {
            var responses = new List<UploadFileResponse>();

            try
            {
                var endpoint = _config.EndPoint;
                var accessKeyId = _config.AccessKeyID;
                var accessKeySecret = _config.AccessKeySecret;
                var bucket = _config.Bucket;
                var domain = _config.Domain;

                var client = new OssClient(endpoint, accessKeyId, accessKeySecret);

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    
                    // 生成唯一文件名
                    string originalFileName = Path.GetFileName(file.FileName);
                    string extension = Path.GetExtension(originalFileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string randomSuffix = new Random().Next(1000, 9999).ToString();
                    var objectKey = $"{module}/{timestamp}_{randomSuffix}_{i}{extension}";

                    using (var stream = file.OpenReadStream())
                    {
                        client.PutObject(bucket, objectKey, stream);
                    }

                    // 生成访问URL
                    string url = $"{domain}/{objectKey}";

                    responses.Add(new UploadFileResponse
                    {
                        Url = url,
                        FileName = objectKey
                    });
                }

                _logger.LogInformation($"Uploaded {files.Count} files to OSS");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OSS batch upload failed: {ex.Message}");
                throw;
            }

            return await Task.FromResult(responses);
        }

        public string GetDomain()
        {
            return _config.Domain;
        }
    }
}

