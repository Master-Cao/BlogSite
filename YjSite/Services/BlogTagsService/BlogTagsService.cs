using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Site.Domain.Entities;
using SqlSugar;
using YjSite.DTOs;
using YjSite.Helpers.Mapping;

namespace YjSite.Services.BlogTagsService
{
    public class BlogTagsService : IBlogTagsService
    {
        private readonly ISqlSugarClient _sql;
        private readonly ILogger<BlogTagsService> _logger;
        private readonly IMemoryCache _cache;

        public BlogTagsService(ISqlSugarClient sql, ILogger<BlogTagsService> logger, IMemoryCache cache)
        {
            _sql = sql;
            _logger = logger;
            _cache = cache;
        }

        public async Task<(List<BlogTagResponse> Tags, int Total)> GetBlogTagsAsync(int page, int pageSize)
        {
            var query = _sql.Queryable<BlogTags>().Where(t => !t.IsDeleted);
            
            var total = await query.CountAsync();
            
            var tags = await query.OrderByDescending(o => o.CreateTime)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            var tagResponses = new List<BlogTagResponse>();
            foreach (var tag in tags)
            {
                tagResponses.Add(tag.ToBlogTagResponse());
            }
            
            return (tagResponses, total);
        }

        public async Task<BlogTagResponse> GetBlogTagByIdAsync(string id)
        {
            var cacheKey = $"blogtag_{id}";
            
            if (!_cache.TryGetValue(cacheKey, out BlogTagResponse tagResponse))
            {
                var tag = await _sql.Queryable<BlogTags>()
                    .Where(t => t.Id == id && !t.IsDeleted)
                    .FirstAsync();
                
                if (tag != null)
                {
                    tagResponse = tag.ToBlogTagResponse();
                    // 缓存10分钟
                    _cache.Set(cacheKey, tagResponse, TimeSpan.FromMinutes(10));
                }
            }
            
            return tagResponse;
        }

        public async Task<BlogTagResponse> CreateBlogTagAsync(CreateBlogTagRequest request, string userId)
        {
            try
            {
                var tag = request.ToBlogTagEntity(userId);
                await _sql.Insertable(tag).ExecuteCommandAsync();
                _logger.LogInformation($"Created blog tag: {tag.Id}");
                
                return tag.ToBlogTagResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating blog tag: {ex.Message}");
                throw;
            }
        }

        public async Task<BlogTagResponse> UpdateBlogTagAsync(string id, UpdateBlogTagRequest request, string userId)
        {
            try
            {
                var tag = await _sql.Queryable<BlogTags>()
                    .Where(t => t.Id == id && !t.IsDeleted)
                    .FirstAsync();

                if (tag == null)
                {
                    return null;
                }

                // 更新实体
                request.UpdateEntity(tag);
                
                // 执行更新
                var result = await _sql.Updateable(tag)
                    .UpdateColumns(t => new BlogTags 
                    { 
                        TagName = t.TagName,
                        SubTagName = t.SubTagName,
                        Icon = t.Icon,
                        Color = t.Color
                    })
                    .Where(t => t.Id == id)
                    .ExecuteCommandAsync();

                if (result > 0)
                {
                    // 更新缓存
                    var response = tag.ToBlogTagResponse();
                    _cache.Set($"blogtag_{tag.Id}", response, TimeSpan.FromMinutes(10));
                    
                    _logger.LogInformation($"Updated blog tag: {tag.Id}");
                    return response;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating blog tag: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteBlogTagAsync(string id, string userId)
        {
            try
            {
                var tag = await _sql.Queryable<BlogTags>()
                    .Where(t => t.Id == id && !t.IsDeleted)
                    .FirstAsync();

                if (tag == null)
                {
                    return false;
                }

                // 软删除
                var result = await _sql.Updateable<BlogTags>()
                    .SetColumns(t => new BlogTags 
                    { 
                        IsDeleted = true, 
                        DeleteTime = DateTime.Now,
                        DeleteUserId = userId
                    })
                    .Where(t => t.Id == id)
                    .ExecuteCommandAsync();
                
                // 清除缓存
                _cache.Remove($"blogtag_{id}");
                
                _logger.LogInformation($"Deleted blog tag: {id}");
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting blog tag: {ex.Message}");
                throw;
            }
        }
    }
} 