using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Site.Domain.Entities;
using SqlSugar;
using YjSite.DTOs;
using YjSite.Helpers.Mapping;
using static Site.Domain.Entities.Plans;

namespace YjSite.Services.PlansService
{
    public class PlanService : IPlanService
    {
        private readonly ISqlSugarClient _sql;
        private readonly ILogger<PlanService> _logger;
        private readonly IMemoryCache _cache;

        public PlanService(ISqlSugarClient sql, ILogger<PlanService> logger, IMemoryCache cache)
        {
            _sql = sql;
            _logger = logger;
            _cache = cache;
        }

        public async Task<(List<PlanResponse> Plans, int Total)> GetPlansAsync(int page, int pageSize, string userId = null)
        {
            var query = _sql.Queryable<PlansModel>().Where(p => !p.IsDeleted);
            
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(p => p.CreateUserId == userId);
            }

            var total = await query.CountAsync();
            
            var plans = await query.OrderByDescending(p => p.CreateTime)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            var planResponses = new List<PlanResponse>();
            foreach (var plan in plans)
            {
                planResponses.Add(plan.ToPlanResponse());
            }
            return (planResponses, total);
        }

        public async Task<(List<PlanResponse> Plans, int Total)> GetPlansByYearAsync(int year, int page, int pageSize, string userId = null)
        {
            // 计算指定年份的开始和结束日期
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31, 23, 59, 59);
            
            var query = _sql.Queryable<PlansModel>()
                .Where(p => !p.IsDeleted)
                .Where(p => p.CreateTime >= startDate && p.CreateTime <= endDate);
            
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(p => p.CreateUserId == userId);
            }

            var total = await query.CountAsync();
            
            var plans = await query.OrderByDescending(p => p.CreateTime)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            var planResponses = new List<PlanResponse>();
            foreach (var plan in plans)
            {
                planResponses.Add(plan.ToPlanResponse());
            }
            
            return (planResponses, total);
        }

        public async Task<PlanResponse> GetPlanByIdAsync(string id)
        {
            var cacheKey = $"plan_{id}";
            
            if (!_cache.TryGetValue(cacheKey, out PlanResponse planResponse))
            {
                var plan = await _sql.Queryable<PlansModel>()
                    .Where(p => p.Id == id && !p.IsDeleted)
                    .FirstAsync();
                
                if (plan != null)
                {
                    planResponse = plan.ToPlanResponse();
                    // 缓存10分钟
                    _cache.Set(cacheKey, planResponse, TimeSpan.FromMinutes(10));
                }
            }
            
            return planResponse;
        }

        public async Task<PlanResponse> CreatePlanAsync(CreatePlanRequest request, string userId)
        {
            try
            {
                var plan = request.ToPlanEntity(userId);
                
                // 使用请求中的CoverImage字段
                plan.CoverImage = request.CoverImage;
                
                await _sql.Insertable(plan).ExecuteCommandAsync();
                _logger.LogInformation($"Created plan: {plan.Id}");
                return plan.ToPlanResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating plan: {ex.Message}");
                throw;
            }
        }

        public async Task<PlanResponse> UpdatePlanAsync(string id, UpdatePlanRequest request, string userId)
        {
            try
            {
                var plan = await _sql.Queryable<PlansModel>()
                    .Where(p => p.Id == id && !p.IsDeleted)
                    .FirstAsync();

                if (plan == null || plan.CreateUserId != userId)
                {
                    return null;
                }

                // 更新实体
                request.UpdateEntity(plan);
                
                // 执行更新
                var result = await _sql.Updateable(plan)
                    .UpdateColumns(p => new PlansModel 
                    { 
                        Title = p.Title,
                        Content = p.Content,
                        Description = p.Description,
                        CoverImage = p.CoverImage,
                        Dealine = p.Dealine,
                        IsComplete = p.IsComplete
                    })
                    .Where(p => p.Id == id)
                    .ExecuteCommandAsync();

                if (result > 0)
                {
                    // 更新缓存
                    var response = plan.ToPlanResponse();
                    _cache.Set($"plan_{plan.Id}", response, TimeSpan.FromMinutes(10));
                    
                    _logger.LogInformation($"Updated plan: {plan.Id}");
                    return response;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating plan: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeletePlanAsync(string id, string userId)
        {
            try
            {
                var plan = await _sql.Queryable<PlansModel>()
                    .Where(p => p.Id == id && !p.IsDeleted)
                    .FirstAsync();

                if (plan == null || plan.CreateUserId != userId)
                {
                    return false;
                }

                // 软删除
                var result = await _sql.Updateable<PlansModel>()
                    .SetColumns(p => new PlansModel 
                    { 
                        IsDeleted = true, 
                        DeleteTime = DateTime.Now,
                        DeleteUserId = userId
                    })
                    .Where(p => p.Id == id)
                    .ExecuteCommandAsync();
                
                // 清除缓存
                _cache.Remove($"plan_{id}");
                
                _logger.LogInformation($"Deleted plan: {id}");
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting plan: {ex.Message}");
                throw;
            }
        }
    }
} 