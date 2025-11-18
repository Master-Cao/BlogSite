using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YjSite.DTOs;
using YjSite.OssHelper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace YjSite.Controllers
{
    public class UploadController : BaseController
    {
        private readonly OssHelper.OssHelper _oss;
        public UploadController(OssHelper.OssHelper ossHelper)
        {
            _oss = ossHelper;
        }
        // POST api/<UploadController>
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Post(IFormFile file, [FromQuery] string module = "default")
        {
            try
            {
                var uploadResult = await _oss.OssUploadFile(file, module);
                
                // 返回包含URL和文件名的响应
                return Ok(JsonView(uploadResult));
            }
            catch (Exception)
            {
                return BadRequest(JsonView(false, "阿里云上传失败"));
            }
        }
        
        // POST api/<UploadController>/multiple
        [HttpPost("uploads")]
        [Authorize]
        public async Task<IActionResult> PostMultiple(List<IFormFile> files, [FromQuery] string module = "default")
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(JsonView("未提供文件"));
                }
                
                var uploadResults = await _oss.OssUpLoadFiles(files, module);
                
                // 返回多文件上传响应
                var response = new MultiUploadFileResponse
                {
                    Files = uploadResults
                };
                
                return Ok(JsonView(response));
            }
            catch (Exception ex)
            {
                return BadRequest(JsonView(false, $"阿里云批量上传失败: {ex.Message}"));
            }
        }
    }
}
