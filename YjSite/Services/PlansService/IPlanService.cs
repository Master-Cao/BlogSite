using YjSite.DTOs;
using static Site.Domain.Entities.Plans;

namespace YjSite.Services.PlansService
{
    public interface IPlanService
    {
        Task<(List<PlanResponse> Plans, int Total)> GetPlansAsync(int page, int pageSize, string userId = null);
        Task<(List<PlanResponse> Plans, int Total)> GetPlansByYearAsync(int year, int page, int pageSize, string userId = null);
        Task<PlanResponse> GetPlanByIdAsync(string id);
        Task<PlanResponse> CreatePlanAsync(CreatePlanRequest request, string userId);
        Task<PlanResponse> UpdatePlanAsync(string id, UpdatePlanRequest request, string userId);
        Task<bool> DeletePlanAsync(string id, string userId);
    }
} 