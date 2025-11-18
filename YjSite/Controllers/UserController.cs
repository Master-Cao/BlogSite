using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YjSite.DTOs;
using YjSite.Helpers;
using YjSite.Services.UserService;
using YjSite.ViewModel;

namespace YjSite.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (users, total) = await _userService.GetUsersAsync(page, pageSize);
            
            // 将实体转换为响应DTO
            var response = new UserListResponse
            {
                Users = users,
                Total = total
            };
            
            return Ok(JsonView(response));
        }

        /// <summary>
        /// 获取单个用户详情
        /// </summary>
        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string id)
        {
            // 只允许管理员和当前用户查看用户详情
            var currentUserId = UserHelper.GetCurrentUserId(User);
            if (currentUserId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(JsonView("用户不存在"));
            }
            
            return Ok(JsonView(user));
        }

        /// <summary>
        /// 创建新用户
        /// </summary>
        [HttpPost("user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var userResponse = await _userService.CreateUserAsync(request);
                
                if (userResponse != null)
                {
                    return Created($"/api/user/{userResponse.Id}", JsonView(userResponse));
                }
                
                return BadRequest(JsonView("创建用户失败"));
            }
            catch (Exception ex)
            {
                return BadRequest(JsonView($"创建用户失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        [HttpPut("user/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            // 只允许管理员和当前用户更新用户信息
            var currentUserId = UserHelper.GetCurrentUserId(User);
            if (currentUserId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(JsonView("用户不存在"));
            }

            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, request);
                
                if (updatedUser != null)
                {
                    return Ok(JsonView(updatedUser));
                }
                
                return BadRequest(JsonView("更新用户信息失败"));
            }
            catch (Exception ex)
            {
                return BadRequest(JsonView($"更新用户信息失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 更新用户密码
        /// </summary>
        [HttpPut("user/{id}/password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(string id, [FromBody] UpdatePasswordRequest request)
        {
            // 只允许当前用户更新自己的密码
            var currentUserId = UserHelper.GetCurrentUserId(User);
            if (currentUserId != id)
            {
                return Forbid();
            }
            
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(JsonView("用户不存在"));
            }

            try
            {
                var success = await _userService.UpdatePasswordAsync(id, request);
                
                if (success)
                {
                    return Ok(JsonView(true, "密码更新成功"));
                }
                
                return BadRequest(JsonView("更新密码失败"));
            }
            catch (Exception ex)
            {
                return BadRequest(JsonView($"更新密码失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete("user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(JsonView("用户不存在"));
            }

            try
            {
                var success = await _userService.DeleteUserAsync(id);
                
                if (success)
                {
                    return Ok(JsonView(true, "删除成功"));
                }
                
                return BadRequest(JsonView("删除用户失败"));
            }
            catch (Exception ex)
            {
                return BadRequest(JsonView($"删除用户失败: {ex.Message}"));
            }
        }
    }
} 