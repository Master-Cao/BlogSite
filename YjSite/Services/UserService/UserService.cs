using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Site.Domain.Entities;
using SqlSugar;
using System.Security.Cryptography;
using System.Text;
using YjSite.DTOs;
using YjSite.Helpers.Mapping;

namespace YjSite.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<UserService> _logger;
        private readonly IMemoryCache _cache;

        public UserService(ISqlSugarClient db, ILogger<UserService> logger, IMemoryCache cache)
        {
            _db = db;
            _logger = logger;
            _cache = cache;
        }

        public async Task<(List<UserResponse>, int)> GetUsersAsync(int page, int pageSize)
        {
            try
            {
                var total = await _db.Queryable<User>()
                    .Where(u => !u.IsDeleted)
                    .CountAsync();

                var users = await _db.Queryable<User>()
                    .Where(u => !u.IsDeleted)
                    .OrderByDescending(u => u.CreateTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var userResponses = users.Select(u => u.ToUserResponse()).ToList();
                return (userResponses, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户列表失败");
                throw;
            }
        }

        public async Task<UserResponse> GetUserByIdAsync(string id)
        {
            var cacheKey = $"user_{id}";
            
            if (!_cache.TryGetValue(cacheKey, out UserResponse userResponse))
            {
                try
                {
                    var user = await _db.Queryable<User>()
                        .Where(u => u.Id == id && !u.IsDeleted)
                        .FirstAsync();

                    if (user != null)
                    {
                        userResponse = user.ToUserResponse();
                        // 缓存10分钟
                        _cache.Set(cacheKey, userResponse, TimeSpan.FromMinutes(10));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"获取用户失败，ID: {id}");
                    return null;
                }
            }

            return userResponse;
        }

        public async Task<UserResponse> GetUserByAccountAsync(string account)
        {
            try
            {
                var user = await _db.Queryable<User>()
                    .Where(u => u.Account == account && !u.IsDeleted)
                    .FirstAsync();

                return user?.ToUserResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"根据账号获取用户失败，账号: {account}");
                return null;
            }
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                // 检查账号是否已存在
                var existingUser = await _db.Queryable<User>()
                    .Where(u => u.Account == request.Account && !u.IsDeleted)
                    .FirstAsync();

                if (existingUser != null)
                {
                    throw new Exception("账号已存在");
                }

                // 创建用户
                var user = request.ToUserEntity();
                
                // 加密密码
                string salt = GenerateSalt();
                user.Password = HashPassword(request.Password, salt) + "~" + salt;

                await _db.Insertable(user).ExecuteCommandAsync();
                _logger.LogInformation($"创建用户成功: {user.Id}");

                return user.ToUserResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建用户失败: {ex.Message}");
                throw;
            }
        }

        public async Task<UserResponse> UpdateUserAsync(string id, UpdateUserRequest request)
        {
            try
            {
                var user = await _db.Queryable<User>()
                    .Where(u => u.Id == id && !u.IsDeleted)
                    .FirstAsync();

                if (user == null)
                {
                    return null;
                }

                // 更新用户
                request.UpdateEntity(user);
                
                var result = await _db.Updateable(user)
                    .UpdateColumns(u => new User
                    {
                        UserName = u.UserName,
                        Avatar = u.Avatar,
                        Pk = u.Pk
                    })
                    .Where(u => u.Id == id)
                    .ExecuteCommandAsync();

                if (result > 0)
                {
                    // 更新缓存
                    var response = user.ToUserResponse();
                    _cache.Set($"user_{id}", response, TimeSpan.FromMinutes(10));
                    
                    _logger.LogInformation($"更新用户成功: {id}");
                    return response;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新用户失败: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdatePasswordAsync(string id, UpdatePasswordRequest request)
        {
            try
            {
                var user = await _db.Queryable<User>()
                    .Where(u => u.Id == id && !u.IsDeleted)
                    .FirstAsync();

                if (user == null)
                {
                    return false;
                }

                // 验证旧密码
                var pwdParts = user.Password.Split("~");
                var oldPasswordHash = pwdParts[0];
                var salt = pwdParts[1];

                if (HashPassword(request.OldPassword, salt) != oldPasswordHash)
                {
                    throw new Exception("旧密码不正确");
                }

                // 更新密码
                string newSalt = GenerateSalt();
                string newPasswordHash = HashPassword(request.NewPassword, newSalt);
                user.Password = newPasswordHash + "~" + newSalt;

                var result = await _db.Updateable(user)
                    .UpdateColumns(u => new User { Password = u.Password })
                    .Where(u => u.Id == id)
                    .ExecuteCommandAsync();

                _logger.LogInformation($"更新密码成功: {id}");
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新密码失败: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var user = await _db.Queryable<User>()
                    .Where(u => u.Id == id && !u.IsDeleted)
                    .FirstAsync();

                if (user == null)
                {
                    return false;
                }

                // 软删除
                var result = await _db.Updateable<User>()
                    .SetColumns(u => new User
                    {
                        IsDeleted = true,
                        DeleteTime = DateTime.Now
                    })
                    .Where(u => u.Id == id)
                    .ExecuteCommandAsync();

                // 清除缓存
                _cache.Remove($"user_{id}");
                
                _logger.LogInformation($"删除用户成功: {id}");
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除用户失败: {ex.Message}");
                throw;
            }
        }

        #region 辅助方法

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Convert.FromBase64String(salt);
            
            using var derivedBytes = new Rfc2898DeriveBytes(
                passwordBytes, 
                saltBytes, 
                100, 
                HashAlgorithmName.SHA256);
            
            byte[] hash = derivedBytes.GetBytes(32);
            return Convert.ToBase64String(hash);
        }

        #endregion
    }
} 