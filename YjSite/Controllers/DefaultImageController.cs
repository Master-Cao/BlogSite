using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YjSite.DTOs;
using YjSite.Helpers;
using YjSite.Services.DefaultImageService;

namespace YjSite.Controllers
{
    public class DefaultImageController : BaseController
    {
        private readonly IDefaultImageService _defaultImageService;

        public DefaultImageController(IDefaultImageService defaultImageService)
        {
            _defaultImageService = defaultImageService;
        }

        /// <summary>
        /// 获取默认图片列表
        /// </summary>
        [HttpGet("default-images")]
        public async Task<IActionResult> GetDefaultImages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (images, total) = await _defaultImageService.GetDefaultImagesAsync(page, pageSize);
            
            // 将实体转换为响应DTO
            var response = new DefaultImageListResponse
            {
                Images = images,
                Total = total
            };
            
            return Ok(JsonView(response));
        }

        /// <summary>
        /// 获取单个默认图片详情
        /// </summary>
        [HttpGet("default-image/{id}")]
        public async Task<IActionResult> GetDefaultImageById(string id)
        {
            var image = await _defaultImageService.GetDefaultImageByIdAsync(id);
            if (image == null)
            {
                return NotFound(JsonView("默认图片不存在"));
            }
            
            return Ok(JsonView(image));
        }

        
        /// <summary>
        /// 创建新默认图片
        /// </summary>
        [HttpPost("default-image")]
        [Authorize]
        public async Task<IActionResult> CreateDefaultImage([FromBody] CreateDefaultImageRequest request)
        {
            var userId = UserHelper.GetCurrentUserId(User);
            var imageResponse = await _defaultImageService.CreateDefaultImageAsync(request, userId);
            
            if (imageResponse != null)
            {
                return Created($"/api/default-image/{imageResponse.Id}", JsonView(imageResponse));
            }
            
            return BadRequest(JsonView("创建默认图片失败"));
        }

        /// <summary>
        /// 更新已有默认图片
        /// </summary>
        [HttpPut("default-image/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateDefaultImage(string id, [FromBody] UpdateDefaultImageRequest request)
        {
            var image = await _defaultImageService.GetDefaultImageByIdAsync(id);
            if (image == null)
            {
                return NotFound(JsonView("默认图片不存在"));
            }

            var userId = UserHelper.GetCurrentUserId(User);
            var updatedImage = await _defaultImageService.UpdateDefaultImageAsync(id, request, userId);
            
            if (updatedImage != null)
            {
                return Ok(JsonView(updatedImage));
            }
            
            return BadRequest(JsonView("更新默认图片失败"));
        }

        /// <summary>
        /// 删除默认图片
        /// </summary>
        [HttpDelete("default-image/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDefaultImage(string id)
        {
            var image = await _defaultImageService.GetDefaultImageByIdAsync(id);
            if (image == null)
            {
                return NotFound(JsonView("默认图片不存在"));
            }

            var userId = UserHelper.GetCurrentUserId(User);
            var success = await _defaultImageService.DeleteDefaultImageAsync(id, userId);
            
            if (success)
            {
                return Ok(JsonView(true, "删除成功"));
            }
            
            return BadRequest(JsonView("删除默认图片失败"));
        }
    }
} 