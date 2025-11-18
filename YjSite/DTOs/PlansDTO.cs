using Site.Domain.Entities;
using static Site.Domain.Entities.Plans;

namespace YjSite.DTOs
{
    // 创建计划请求
    public class CreatePlanRequest
    {
        public string Title { get; set; }
        public string? Content { get; set; }
        public string Description { get; set; }
        public string? CoverImage { get; set; }
        public DateTime? Dealine { get; set; }
        public bool IsComplete { get; set; } = false;
    }

    // 更新计划请求
    public class UpdatePlanRequest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public string Description { get; set; }
        public string? CoverImage { get; set; }
        public DateTime? Dealine { get; set; }
        public bool IsComplete { get; set; }
    }

    // 计划响应
    public class PlanResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public string Description { get; set; }
        public string? CoverImage { get; set; }
        public DateTime? Dealine { get; set; }
        public bool IsComplete { get; set; }
        public string CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }

    // 计划列表响应
    public class PlanListResponse
    {
        public List<PlanResponse> Plans { get; set; }
        public int Total { get; set; }
    }
} 