using Site.Domain.Entities;

namespace YjSite.DTOs
{
    // 用户创建请求
    public class CreateUserRequest
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string? Avatar { get; set; }
        public string? Pk { get; set; }
    }

    // 用户更新请求
    public class UpdateUserRequest
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? Avatar { get; set; }
        public string? Pk { get; set; }
    }

    // 用户密码更新请求
    public class UpdatePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    // 用户响应
    public class UserResponse
    {
        public string Id { get; set; }
        public string Account { get; set; }
        public string UserName { get; set; }
        public string? Avatar { get; set; }
        public string? Pk { get; set; }
        public DateTime CreateTime { get; set; }
    }

    // 用户列表响应
    public class UserListResponse
    {
        public List<UserResponse> Users { get; set; }
        public int Total { get; set; }
    }
} 