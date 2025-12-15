using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Site.Domain.Entities;
using SqlSugar;
using YjSite.DTOs;
using YjSite.Helpers.Mapping;

namespace YjSite.Services.LifeShareService
{
    /// <summary>
    /// 生活分享服务实现
    /// </summary>
    public class LifeShareService : ILifeShareService
    {
        private readonly ISqlSugarClient _sql;
        private readonly ILogger<LifeShareService> _logger;
        private readonly IMemoryCache _cache;

        public LifeShareService(ISqlSugarClient sql, ILogger<LifeShareService> logger, IMemoryCache cache)
        {
            _sql = sql;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// 获取随机默认图片URL
        /// </summary>
        private async Task<string> GetRandomDefaultImageAsync()
        {
            try
            {
                var images = await _sql.Queryable<DefaultImage>()
                    .Where(i => !i.IsDeleted)
                    .ToListAsync();

                if (images.Count > 0)
                {
                    var random = new Random();
                    return images[random.Next(images.Count)].Url;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get default image, using placeholder");
            }

            // 返回备用图片
            return "https://picsum.photos/800/600?random=" + new Random().Next(1000);
        }

        public async Task<(List<LifeShareResponse> Shares, int Total)> GetSharesAsync(int page, int pageSize, string? userId = null)
        {
            try
            {
                var query = _sql.Queryable<LifeShares>().Where(s => !s.IsDeleted);

                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(s => s.CreateUserId == userId);
                }

                var total = await query.CountAsync();

                var shares = await query
                    .OrderByDescending(s => s.CreateTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var responses = shares.Select(s => s.ToLifeShareResponse()).ToList();

                _logger.LogInformation($"Retrieved {shares.Count} life shares, page: {page}");
                return (responses, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving life shares: {ex.Message}");
                throw;
            }
        }

        public async Task<(List<LifeShareResponse> Shares, int Total)> GetSharesByCategoryAsync(string category, int page, int pageSize)
        {
            try
            {
                var query = _sql.Queryable<LifeShares>()
                    .Where(s => !s.IsDeleted && s.Category == category);

                var total = await query.CountAsync();

                var shares = await query
                    .OrderByDescending(s => s.CreateTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var responses = shares.Select(s => s.ToLifeShareResponse()).ToList();

                _logger.LogInformation($"Retrieved {shares.Count} life shares by category: {category}");
                return (responses, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving life shares by category: {ex.Message}");
                throw;
            }
        }

        public async Task<(List<LifeShareResponse> Shares, int Total)> GetSharesByViewCountAsync(int page, int pageSize, int minViewCount = 0)
        {
            try
            {
                var query = _sql.Queryable<LifeShares>()
                    .Where(s => !s.IsDeleted && s.ViewCount >= minViewCount);

                var total = await query.CountAsync();

                var shares = await query
                    .OrderByDescending(s => s.ViewCount)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var responses = shares.Select(s => s.ToLifeShareResponse()).ToList();

                _logger.LogInformation($"Retrieved {shares.Count} popular life shares (min views: {minViewCount})");
                return (responses, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving popular life shares: {ex.Message}");
                throw;
            }
        }

        public async Task<LifeShareResponse?> GetShareByIdAsync(string id)
        {
            try
            {
                var cacheKey = $"lifeshare_{id}";

                if (!_cache.TryGetValue(cacheKey, out LifeShareResponse? response))
                {
                    var share = await _sql.Queryable<LifeShares>()
                        .Where(s => s.Id == id && !s.IsDeleted)
                        .FirstAsync();

                    if (share != null)
                    {
                        response = share.ToLifeShareResponse();
                        _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving life share by id: {id}");
                throw;
            }
        }

        public async Task<LifeShareResponse?> CreateShareAsync(CreateLifeShareRequest request, string? userId)
        {
            try
            {
                string? authorName = null;
                string? authorAvatar = null;

                // 如果有用户ID，获取用户信息用于冗余存储
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _sql.Queryable<User>()
                        .Where(u => u.Id == userId)
                        .FirstAsync();
                    
                    authorName = user?.UserName;
                    authorAvatar = user?.Avatar;
                }
                else
                {
                    // 匿名用户默认显示名称
                    authorName = "匿名用户";
                }

                // 如果没有封面图片，获取随机默认图片
                if (string.IsNullOrEmpty(request.CoverImage))
                {
                    request.CoverImage = await GetRandomDefaultImageAsync();
                }

                var entity = request.ToLifeShareEntity(
                    userId,
                    authorName,
                    authorAvatar
                );

                await _sql.Insertable(entity).ExecuteCommandAsync();

                var response = entity.ToLifeShareResponse();
                _cache.Set($"lifeshare_{entity.Id}", response, TimeSpan.FromMinutes(10));

                _logger.LogInformation($"Created life share: {entity.Id}, Anonymous: {string.IsNullOrEmpty(userId)}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating life share: {ex.Message}");
                throw;
            }
        }

        public async Task<LifeShareResponse?> UpdateShareAsync(string id, UpdateLifeShareRequest request, string userId)
        {
            try
            {
                var share = await _sql.Queryable<LifeShares>()
                    .Where(s => s.Id == id && !s.IsDeleted)
                    .FirstAsync();

                if (share == null || share.CreateUserId != userId)
                {
                    return null;
                }

                request.UpdateEntity(share);

                var result = await _sql.Updateable(share)
                    .UpdateColumns(s => new LifeShares
                    {
                        Title = s.Title,
                        Content = s.Content,
                        CoverImage = s.CoverImage,
                        Images = s.Images,
                        Category = s.Category,
                        Tags = s.Tags
                    })
                    .Where(s => s.Id == id)
                    .ExecuteCommandAsync();

                if (result > 0)
                {
                    var response = share.ToLifeShareResponse();
                    _cache.Set($"lifeshare_{share.Id}", response, TimeSpan.FromMinutes(10));

                    _logger.LogInformation($"Updated life share: {share.Id}");
                    return response;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating life share: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteShareAsync(string id, string userId)
        {
            try
            {
                var share = await _sql.Queryable<LifeShares>()
                    .Where(s => s.Id == id && !s.IsDeleted)
                    .FirstAsync();

                if (share == null || share.CreateUserId != userId)
                {
                    return false;
                }

                // 软删除
                var result = await _sql.Updateable<LifeShares>()
                    .SetColumns(s => new LifeShares
                    {
                        IsDeleted = true,
                        DeleteTime = DateTime.Now,
                        DeleteUserId = userId
                    })
                    .Where(s => s.Id == id)
                    .ExecuteCommandAsync();

                // 清除缓存
                _cache.Remove($"lifeshare_{id}");

                _logger.LogInformation($"Deleted life share: {id}");
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting life share: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IncrementViewCountAsync(string id)
        {
            try
            {
                var result = await _sql.Updateable<LifeShares>()
                    .SetColumns(s => new LifeShares { ViewCount = s.ViewCount + 1 })
                    .Where(s => s.Id == id && !s.IsDeleted)
                    .ExecuteCommandAsync();

                // 清除缓存以便下次获取最新数据
                _cache.Remove($"lifeshare_{id}");

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error incrementing view count for life share: {id}");
                throw;
            }
        }

        public async Task<bool> ToggleLikeAsync(string id, bool isLike)
        {
            try
            {
                var increment = isLike ? 1 : -1;
                
                var result = await _sql.Updateable<LifeShares>()
                    .SetColumns(s => new LifeShares { LikeCount = s.LikeCount + increment })
                    .Where(s => s.Id == id && !s.IsDeleted && (isLike || s.LikeCount > 0))
                    .ExecuteCommandAsync();

                // 清除缓存
                _cache.Remove($"lifeshare_{id}");

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling like for life share: {id}");
                throw;
            }
        }
    }
}

