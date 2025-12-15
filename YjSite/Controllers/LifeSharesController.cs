using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YjSite.DTOs;
using YjSite.Helpers;
using YjSite.Services.LifeShareService;

namespace YjSite.Controllers
{
    /// <summary>
    /// 生活分享控制器
    /// </summary>
    public class LifeSharesController : BaseController
    {
        private readonly ILifeShareService _lifeShareService;

        public LifeSharesController(ILifeShareService lifeShareService)
        {
            _lifeShareService = lifeShareService;
        }

        /// <summary>
        /// 获取生活分享列表
        /// </summary>
        [HttpGet("life-shares")]
        public async Task<IActionResult> GetShares(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 12, 
            [FromQuery] string? userId = null)
        {
            var (shares, total) = await _lifeShareService.GetSharesAsync(page, pageSize, userId);

            var response = new LifeShareListResponse
            {
                Shares = shares,
                Total = total
            };

            return Ok(JsonView(response));
        }

        /// <summary>
        /// 按分类获取生活分享列表
        /// </summary>
        [HttpGet("life-shares/category/{category}")]
        public async Task<IActionResult> GetSharesByCategory(
            string category, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 12)
        {
            var (shares, total) = await _lifeShareService.GetSharesByCategoryAsync(category, page, pageSize);

            var response = new LifeShareListResponse
            {
                Shares = shares,
                Total = total
            };

            return Ok(JsonView(response));
        }

        /// <summary>
        /// 获取热门生活分享（按浏览量排序）
        /// </summary>
        [HttpGet("life-shares/popular")]
        public async Task<IActionResult> GetPopularShares(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 12, 
            [FromQuery] int minViewCount = 0)
        {
            var (shares, total) = await _lifeShareService.GetSharesByViewCountAsync(page, pageSize, minViewCount);

            var response = new LifeShareListResponse
            {
                Shares = shares,
                Total = total
            };

            return Ok(JsonView(response));
        }

        /// <summary>
        /// 获取单个分享详情
        /// </summary>
        [HttpGet("life-share/{id}")]
        public async Task<IActionResult> GetShareById(string id)
        {
            var share = await _lifeShareService.GetShareByIdAsync(id);
            
            if (share == null)
            {
                return NotFound(JsonView("分享不存在"));
            }

            // 增加浏览量
            await _lifeShareService.IncrementViewCountAsync(id);

            return Ok(JsonView(share));
        }

        /// <summary>
        /// 创建生活分享（支持匿名投递）
        /// </summary>
        [HttpPost("life-share")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateShare([FromBody] CreateLifeShareRequest request)
        {
            // 尝试获取用户ID，匿名用户为null
            string? userId = null;
            try
            {
                userId = UserHelper.GetCurrentUserId(User);
            }
            catch
            {
                // 匿名用户，userId保持为null
            }

            var shareResponse = await _lifeShareService.CreateShareAsync(request, userId);

            if (shareResponse != null)
            {
                return Created($"/api/life-share/{shareResponse.Id}", JsonView(shareResponse));
            }

            return BadRequest(JsonView("创建分享失败"));
        }

        /// <summary>
        /// 更新生活分享
        /// </summary>
        [HttpPut("life-share/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateShare(string id, [FromBody] UpdateLifeShareRequest request)
        {
            var share = await _lifeShareService.GetShareByIdAsync(id);
            
            if (share == null)
            {
                return NotFound(JsonView("分享不存在"));
            }

            var userId = UserHelper.GetCurrentUserId(User);
            
            if (share.CreateUserId != userId)
            {
                return Forbid();
            }

            var updatedShare = await _lifeShareService.UpdateShareAsync(id, request, userId);

            if (updatedShare != null)
            {
                return Ok(JsonView(updatedShare));
            }

            return BadRequest(JsonView("更新分享失败"));
        }

        /// <summary>
        /// 删除生活分享
        /// </summary>
        [HttpDelete("life-share/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteShare(string id)
        {
            var share = await _lifeShareService.GetShareByIdAsync(id);
            
            if (share == null)
            {
                return NotFound(JsonView("分享不存在"));
            }

            var userId = UserHelper.GetCurrentUserId(User);
            
            if (share.CreateUserId != userId)
            {
                return Forbid();
            }

            var success = await _lifeShareService.DeleteShareAsync(id, userId);

            if (success)
            {
                return Ok(JsonView(true, "删除成功"));
            }

            return BadRequest(JsonView("删除分享失败"));
        }

        /// <summary>
        /// 点赞分享
        /// </summary>
        [HttpPost("life-share/{id}/like")]
        public async Task<IActionResult> LikeShare(string id)
        {
            var success = await _lifeShareService.ToggleLikeAsync(id, true);

            if (success)
            {
                return Ok(JsonView(true, "点赞成功"));
            }

            return BadRequest(JsonView("点赞失败"));
        }

        /// <summary>
        /// 取消点赞
        /// </summary>
        [HttpDelete("life-share/{id}/like")]
        public async Task<IActionResult> UnlikeShare(string id)
        {
            var success = await _lifeShareService.ToggleLikeAsync(id, false);

            if (success)
            {
                return Ok(JsonView(true, "取消点赞成功"));
            }

            return BadRequest(JsonView("取消点赞失败"));
        }
    }
}

