using YjSite.DTOs;

namespace YjSite.Services.UserService
{
    public interface IUserService
    {
        /// <summary>
        /// 获取用户列表
        /// </summary>
        Task<(List<UserResponse>, int)> GetUsersAsync(int page, int pageSize);
        
        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        Task<UserResponse> GetUserByIdAsync(string id);
        
        /// <summary>
        /// 根据账号获取用户
        /// </summary>
        Task<UserResponse> GetUserByAccountAsync(string account);
        
        /// <summary>
        /// 创建用户
        /// </summary>
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        
        /// <summary>
        /// 更新用户信息
        /// </summary>
        Task<UserResponse> UpdateUserAsync(string id, UpdateUserRequest request);
        
        /// <summary>
        /// 更新用户密码
        /// </summary>
        Task<bool> UpdatePasswordAsync(string id, UpdatePasswordRequest request);
        
        /// <summary>
        /// 删除用户
        /// </summary>
        Task<bool> DeleteUserAsync(string id);
    }
} 