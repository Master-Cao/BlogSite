using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YjSite.DTOs;
using YjSite.Services.OssService;

namespace YjSite.Controllers
{
    /// <summary>
    /// 文件上传控制器
    /// </summary>
    public class UploadController : BaseController
    {
        private readonly IOssService _ossService;
        
        public UploadController(IOssService ossService)
        {
            _ossService = ossService;
        }

        /// <summary>
        /// 上传单个文件（支持匿名上传）
        /// </summary>
        [HttpPost("upload")]
        [AllowAnonymous]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string module = "lifeshare")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(JsonView("未提供文件"));
                }

                // 验证文件类型
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(JsonView("不支持的文件类型，仅支持 jpg/png/gif/webp"));
                }

                // 验证文件大小（最大 10MB）
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(JsonView("文件大小超过限制（最大10MB）"));
                }

                var uploadResult = await _ossService.UploadFileAsync(file, module);
                return Ok(JsonView(uploadResult));
            }
            catch (Exception ex)
            {
                return BadRequest(JsonView(false, $"上传失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 上传多个文件（支持匿名上传）
        /// </summary>
        [HttpPost("uploads")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadMultiple(List<IFormFile> files, [FromQuery] string module = "lifeshare")
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(JsonView("未提供文件"));
                }

                // 限制最大文件数量
                if (files.Count > 20)
                {
                    return BadRequest(JsonView("单次最多上传20个文件"));
                }

                // 验证所有文件
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                foreach (var file in files)
                {
                    if (!allowedTypes.Contains(file.ContentType.ToLower()))
                    {
                        return BadRequest(JsonView($"文件 {file.FileName} 类型不支持"));
                    }
                    if (file.Length > 10 * 1024 * 1024)
                    {
                        return BadRequest(JsonView($"文件 {file.FileName} 超过大小限制"));
                    }
                }

                var uploadResults = await _ossService.UploadFilesAsync(files, module);

                var response = new MultiUploadFileResponse
                {
                    Files = uploadResults
                };

                return Ok(JsonView(response));
            }
            catch (Exception ex)
            {
                return BadRequest(JsonView(false, $"批量上传失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 获取 OSS 域名
        /// </summary>
        [HttpGet("oss-domain")]
        [AllowAnonymous]
        public IActionResult GetOssDomain()
        {
            var domain = _ossService.GetDomain();
            return Ok(JsonView(new { domain }));
        }
    }
}
