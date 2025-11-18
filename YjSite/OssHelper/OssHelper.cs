using Aliyun.OSS;
using Aliyun.OSS.Common;
using Microsoft.Extensions.Options;
using YjSite.Config;
using YjSite.DTOs;

namespace YjSite.OssHelper
{
    public class OssHelper
    {
        private readonly OssConfiguration _config;
        private ILogger<OssHelper> _logger;
        public OssHelper(IOptions<OssConfiguration> config, ILogger<OssHelper> logger)
        {
            _config = config.Value;
            _logger = logger;
        }


        public Task<UploadFileResponse> OssUploadFile(IFormFile file, string module = "default")
        {
            try
            {
                //阿里云oss相关参数,请自行补齐 
                var endpoint = _config.EndPoint.ToString();
                var accessKeyId = _config.AccessKeyID.ToString();
                var accessKeySecret = _config.AccessKeySecret.ToString();
                var bucket = _config.Bucket.ToString();
                var domain = _config.Domain.ToString();

                var write_client = new OssClient(endpoint, accessKeyId, accessKeySecret);
                var read_client = new OssClient(endpoint, accessKeyId, accessKeySecret);

                // 获取原始文件名
                string originalFileName = Path.GetFileName(file.FileName);
                // 使用模块名称和原始文件名组合，添加一个随机数避免重名
                string randomSuffix = new Random().Next(1000, 9999).ToString();
                var fname = $"{module}/{randomSuffix}_{originalFileName}";

                using (var stream = file.OpenReadStream())
                {
                    write_client.PutObject(bucket, fname, stream);
                }
                DateTime expiration = DateTime.Now.AddYears(20);
                var url = read_client.GeneratePresignedUri(bucket, fname, expiration);
                string urlstring = domain + url.AbsolutePath;
                
                var response = new UploadFileResponse
                {
                    Url = urlstring,
                    FileName = fname
                };
                
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "阿里云图片上传失败,ex=" + ex.Message);
                throw;
            }
        }

        public Task<List<UploadFileResponse>> OssUpLoadFiles(List<IFormFile> files, string module = "default")
        {
            List<UploadFileResponse> responses = new List<UploadFileResponse>();
            try
            {
                //阿里云oss相关参数,请自行补齐 
                var endpoint = _config.EndPoint.ToString();
                var accessKeyId = _config.AccessKeyID.ToString();
                var accessKeySecret = _config.AccessKeySecret.ToString();
                var bucket = _config.Bucket.ToString();
                var domain = _config.Domain.ToString();

                var write_client = new OssClient(endpoint, accessKeyId, accessKeySecret);
                var read_client = new OssClient(endpoint, accessKeyId, accessKeySecret);

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    // 获取原始文件名
                    string originalFileName = Path.GetFileName(file.FileName);
                    // 使用模块名称和原始文件名组合，添加一个随机数和索引避免重名
                    string randomSuffix = new Random().Next(1000, 9999).ToString();
                    var fname = $"{module}/{randomSuffix}_{i}_{originalFileName}";
                    
                    using (var stream = file.OpenReadStream())
                    {
                        write_client.PutObject(bucket, fname, stream);
                    }
                    DateTime expiration = DateTime.Now.AddYears(20);
                    var url = read_client.GeneratePresignedUri(bucket, fname, expiration);
                    string urlstring = domain + url.AbsolutePath;
                    
                    responses.Add(new UploadFileResponse
                    {
                        Url = urlstring,
                        FileName = fname
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "阿里云图片上传失败,ex=" + ex.Message);
                throw;
            }

            return Task.FromResult(responses);
        }

    }
}
