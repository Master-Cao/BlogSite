using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Site.Domain.Entities;
using SqlSugar;
using YjSite.DTOs;
using YjSite.Helpers.Mapping;

namespace YjSite.Services.BlogsService
{
    public class BlogService : IBlogService
    {
        private readonly ISqlSugarClient _sql;
        private readonly ILogger<BlogService> _logger;
        private readonly IMemoryCache _cache;

        public BlogService(ISqlSugarClient sql, ILogger<BlogService> logger, IMemoryCache cache)
        {
            _sql = sql;
            _logger = logger;
            _cache = cache;
        }

        public async Task<(List<BlogResponse> Blogs, int Total)> GetBlogsAsync(int page, int pageSize, string userId = null)
        {
            var query = _sql.Queryable<Blogs>().Where(b => !b.IsDeleted);
            
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.CreateUserId == userId);
            }

            var total = await query.CountAsync();
            
            var blogs = await query.OrderByDescending(o => o.CreateTime)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            var blogResponses = new List<BlogResponse>();
            foreach (var blog in blogs)
            {
                blogResponses.Add(blog.ToBlogResponse());
            }
            return (blogResponses, total);
        }

        public async Task<(List<BlogResponse> Blogs, int Total)> GetBlogsByViewCountAsync(int page, int pageSize, int minViewCount = 0)
        {
            try
            {
                // 查询未删除且观看量大于等于指定值的博客
                var query = _sql.Queryable<Blogs>()
                    .Where(b => !b.IsDeleted && b.View >= minViewCount);

                var total = await query.CountAsync();
                
                // 按观看量降序排序，获取分页数据
                var blogs = await query
                    .OrderByDescending(b => b.View)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var blogResponses = new List<BlogResponse>();
                foreach (var blog in blogs)
                {
                    blogResponses.Add(blog.ToBlogResponse());
                }
                
                _logger.LogInformation($"Retrieved {blogs.Count} blogs by view count (min: {minViewCount})");
                return (blogResponses, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving blogs by view count: {ex.Message}");
                throw;
            }
        }

        public async Task<BlogResponse> GetBlogByIdAsync(string id)
        {
            var cacheKey = $"blog_{id}";
            
            if (!_cache.TryGetValue(cacheKey, out BlogResponse blogResponse))
            {
                var blog = await _sql.Queryable<Site.Domain.Entities.Blogs>()
                    .Where(b => b.Id == id && !b.IsDeleted)
                    .FirstAsync();
                
                if (blog != null)
                {

                    blogResponse = blog.ToBlogResponse();
                    // 缓存10分钟
                    _cache.Set(cacheKey, blogResponse, TimeSpan.FromMinutes(10));
                }
            }
            
            return blogResponse;
        }

        public async Task<BlogResponse> CreateBlogAsync(CreateBlogRequest request, string userId)
        {
            try
            {
                var blog = request.ToBlogEntity(userId);
                await _sql.Insertable(blog).ExecuteCommandAsync();
                
                // 将新博客添加到缓存
                var response = blog.ToBlogResponse();
                _cache.Set($"blog_{blog.Id}", response, TimeSpan.FromMinutes(10));
                
                _logger.LogInformation($"Created blog: {blog.Id}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating blog: {ex.Message}");
                throw;
            }
        }

        public async Task<BlogResponse> UpdateBlogAsync(string id, UpdateBlogRequest request, string userId)
        {
            try
            {
                var blog = await _sql.Queryable<Blogs>()
                    .Where(b => b.Id == id && !b.IsDeleted)
                    .FirstAsync();

                if (blog == null || blog.CreateUserId != userId)
                {
                    return null;
                }

                // 更新实体
                request.UpdateEntity(blog);
                
                // 确保状态更新
                blog.State = request.State;
                
                // 执行更新
                var result = await _sql.Updateable(blog)
                    .UpdateColumns(b => new Blogs 
                    { 
                        Title = b.Title,
                        Summary = b.Summary,
                        Content = b.Content,
                        ContentHtml = b.ContentHtml,
                        Tages = b.Tages,
                        CoverImage = b.CoverImage,
                        State = b.State
                    })
                    .Where(b => b.Id == id)
                    .ExecuteCommandAsync();

                if (result > 0)
                {
                    // 更新缓存
                    var response = blog.ToBlogResponse();
                    _cache.Set($"blog_{blog.Id}", response, TimeSpan.FromMinutes(10));
                    
                    _logger.LogInformation($"Updated blog: {blog.Id}, State: {blog.State}");
                    return response;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating blog: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteBlogAsync(string id, string userId)
        {
            try
            {
                var blog = await _sql.Queryable<Site.Domain.Entities.Blogs>()
                    .Where(b => b.Id == id && !b.IsDeleted)
                    .FirstAsync();

                if (blog == null || blog.CreateUserId != userId)
                {
                    return false;
                }

                // 软删除
                var result = await _sql.Updateable<Site.Domain.Entities.Blogs>()
                    .SetColumns(b => new Site.Domain.Entities.Blogs 
                    { 
                        IsDeleted = true, 
                        DeleteTime = DateTime.Now,
                        DeleteUserId = userId
                    })
                    .Where(b => b.Id == id)
                    .ExecuteCommandAsync();
                
                // 清除缓存
                _cache.Remove($"blog_{id}");
                
                _logger.LogInformation($"Deleted blog: {id}");
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting blog: {ex.Message}");
                throw;
            }
        }

        public async Task<(List<BlogResponse> Blogs, int Total)> GetBlogsByTagAsync(string tag, int page, int pageSize)
        {
            try
            {
                // 查询未删除且标签包含指定标签的博客
                var query = _sql.Queryable<Blogs>()
                    .Where(b => !b.IsDeleted && b.Tages.Contains(tag));

                var total = await query.CountAsync();
                
                // 按创建时间降序排序，获取分页数据
                var blogs = await query
                    .OrderByDescending(b => b.CreateTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var blogResponses = new List<BlogResponse>();
                foreach (var blog in blogs)
                {
                    blogResponses.Add(blog.ToBlogResponse());
                }
                
                _logger.LogInformation($"Retrieved {blogs.Count} blogs by tag: {tag}");
                return (blogResponses, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving blogs by tag: {ex.Message}");
                throw;
            }
        }

        public async Task<BlogResponse> PublishBlogAsync(CreateBlogRequest request, string userId)
        {
            try
            {
                var blog = request.ToBlogEntity(userId);
                await _sql.Insertable(blog).ExecuteCommandAsync();
                
                // 将新博客添加到缓存
                var response = blog.ToBlogResponse();
                _cache.Set($"blog_{blog.Id}", response, TimeSpan.FromMinutes(10));
                
                // 清除可能存在的博客列表缓存 (如果有任何列表缓存)
                // 这里可以按需清除特定的缓存键，比如首页博客列表等
                
                _logger.LogInformation($"Published blog: {blog.Id}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error publishing blog: {ex.Message}");
                throw;
            }
        }
    }
} 