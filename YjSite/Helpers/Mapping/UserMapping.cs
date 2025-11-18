using Site.Domain.Entities;
using YjSite.DTOs;

namespace YjSite.Helpers.Mapping
{
    public static class UserMapping
    {
        // 将用户实体转换为响应DTO
        public static UserResponse ToUserResponse(this User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Account = user.Account,
                UserName = user.UserName,
                Avatar = user.Avatar,
                Pk = user.Pk,
                CreateTime = user.CreateTime
            };
        }

        // 将请求DTO转换为用户实体
        public static User ToUserEntity(this CreateUserRequest request)
        {
            return new User
            {
                Account = request.Account,
                Password = request.Password, // 注意：实际使用时应对密码进行加密
                UserName = request.UserName,
                Avatar = request.Avatar,
                Pk = request.Pk
            };
        }

        // 将请求DTO更新用户实体
        public static void UpdateEntity(this UpdateUserRequest request, User user)
        {
            user.UserName = request.UserName;
            user.Avatar = request.Avatar;
            user.Pk = request.Pk;
        }
    }
} 