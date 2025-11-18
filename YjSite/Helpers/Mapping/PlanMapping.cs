using Site.Domain.Entities;
using YjSite.DTOs;
using static Site.Domain.Entities.Plans;

namespace YjSite.Helpers.Mapping
{
    public static class PlanMapping
    {
        // 将计划实体转换为响应DTO
        public static PlanResponse ToPlanResponse(this PlansModel plan)
        {
            return new PlanResponse
            {
                Id = plan.Id,
                Title = plan.Title,
                Content = plan.Content,
                Description = plan.Description,
                CoverImage = plan.CoverImage,
                Dealine = plan.Dealine,
                IsComplete = plan.IsComplete,
                CreateUserId = plan.CreateUserId,
                CreateTime = plan.CreateTime
            };
        }

        // 将请求DTO转换为计划实体
        public static PlansModel ToPlanEntity(this CreatePlanRequest request, string userId)
        {
            return new PlansModel
            {
                Title = request.Title,
                Content = request.Content,
                Description = request.Description,
                CoverImage = request.CoverImage,
                Dealine = request.Dealine,
                IsComplete = request.IsComplete,
                CreateUserId = userId
            };
        }

        // 将请求DTO更新计划实体
        public static void UpdateEntity(this UpdatePlanRequest request, PlansModel plan)
        {
            plan.Title = request.Title;
            plan.Content = request.Content;
            plan.Description = request.Description;
            plan.CoverImage = request.CoverImage;
            plan.Dealine = request.Dealine;
            plan.IsComplete = request.IsComplete;
        }
    }
} 