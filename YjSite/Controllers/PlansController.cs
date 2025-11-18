using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Site.Domain.Entities;
using YjSite.DTOs;
using YjSite.Helpers;
using YjSite.Services.PlansService;
using YjSite.ViewModel;
using static Site.Domain.Entities.Plans;

namespace YjSite.Controllers;

public class PlansController : BaseController
{
    private readonly IPlanService _planService;

    public PlansController(IPlanService planService)
    {
        _planService = planService;
    }

    /// <summary>
    /// 获取计划列表
    /// </summary>
    [HttpGet("plans")]
    public async Task<IActionResult> GetPlans([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string userId = null)
    {
        var (plans, total) = await _planService.GetPlansAsync(page, pageSize, userId);
        
        // 将实体转换为响应DTO
        var response = new PlanListResponse
        {
            Plans = plans,
            Total = total
        };
        
        return Ok(JsonView(response));
    }

    /// <summary>
    /// 根据年份获取计划列表
    /// </summary>
    [HttpGet("plans/year/{year}")]
    public async Task<IActionResult> GetPlansByYear(int year, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string userId = null)
    {
        var (plans, total) = await _planService.GetPlansByYearAsync(year, page, pageSize, userId);
        
        // 将实体转换为响应DTO
        var response = new PlanListResponse
        {
            Plans = plans,
            Total = total
        };
        
        return Ok(JsonView(response));
    }

    /// <summary>
    /// 获取单个计划详情
    /// </summary>
    [HttpGet("plan/{id}")]
    public async Task<IActionResult> GetPlanById(string id)
    {
        var plan = await _planService.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return NotFound(JsonView("计划不存在"));
        }
        
        return Ok(JsonView(plan));
    }

    /// <summary>
    /// 创建新计划
    /// </summary>
    [HttpPost("plan")]
    [Authorize]
    public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequest request)
    {
        try
        {
            var userId = UserHelper.GetCurrentUserId(User);
            
            // 直接传递CoverImage字段，不需要上传图片
            var planResponse = await _planService.CreatePlanAsync(request, userId);
            
            if (planResponse != null)
            {
                return Created($"/api/plan/{planResponse.Id}", JsonView(planResponse));
            }
            
            return BadRequest(JsonView("创建计划失败"));
        }
        catch (Exception ex)
        {
            return BadRequest(JsonView($"创建计划失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 更新已有计划
    /// </summary>
    [HttpPut("plan/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePlan(string id, [FromBody] UpdatePlanRequest request)
    {
        var plan = await _planService.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return NotFound(JsonView("计划不存在"));
        }

        var userId = UserHelper.GetCurrentUserId(User);
        if (plan.CreateUserId != userId)
        {
            return Forbid();
        }

        var updatedPlan = await _planService.UpdatePlanAsync(id, request, userId);
        
        if (updatedPlan != null)
        {
            return Ok(JsonView(updatedPlan));
        }
        
        return BadRequest(JsonView("更新计划失败"));
    }

    /// <summary>
    /// 更新计划完成状态
    /// </summary>
    [HttpPut("plan/{id}/complete")]
    [Authorize]
    public async Task<IActionResult> UpdatePlanCompleteStatus(string id, [FromBody] bool isComplete)
    {
        var plan = await _planService.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return NotFound(JsonView("计划不存在"));
        }

        var userId = UserHelper.GetCurrentUserId(User);
        if (plan.CreateUserId != userId)
        {
            return Forbid();
        }

        // 创建更新请求，只更新IsComplete属性
        var updateRequest = new UpdatePlanRequest
        {
            Id = id,
            Title = plan.Title,
            Content = plan.Content,
            Description = plan.Description,
            CoverImage = plan.CoverImage,
            Dealine = plan.Dealine,
            IsComplete = isComplete
        };

        var updatedPlan = await _planService.UpdatePlanAsync(id, updateRequest, userId);
        
        if (updatedPlan != null)
        {
            return Ok(JsonView(updatedPlan));
        }
        
        return BadRequest(JsonView("更新计划完成状态失败"));
    }

    /// <summary>
    /// 更新计划内容
    /// </summary>
    [HttpPut("plan/{id}/content")]
    [Authorize]
    public async Task<IActionResult> UpdatePlanContent(string id, [FromBody] string content)
    {
        var plan = await _planService.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return NotFound(JsonView("计划不存在"));
        }

        var userId = UserHelper.GetCurrentUserId(User);
        if (plan.CreateUserId != userId)
        {
            return Forbid();
        }

        // 创建更新请求，只更新Content属性
        var updateRequest = new UpdatePlanRequest
        {
            Id = id,
            Title = plan.Title,
            Content = content,
            Description = plan.Description,
            CoverImage = plan.CoverImage,
            Dealine = plan.Dealine,
            IsComplete = plan.IsComplete
        };

        var updatedPlan = await _planService.UpdatePlanAsync(id, updateRequest, userId);
        
        if (updatedPlan != null)
        {
            return Ok(JsonView(updatedPlan));
        }
        
        return BadRequest(JsonView("更新计划内容失败"));
    }

    /// <summary>
    /// 删除计划
    /// </summary>
    [HttpDelete("plan/{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePlan(string id)
    {
        var plan = await _planService.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return NotFound(JsonView("计划不存在"));
        }

        var userId = UserHelper.GetCurrentUserId(User);
        if (plan.CreateUserId != userId)
        {
            return Forbid();
        }

        var success = await _planService.DeletePlanAsync(id, userId);
        
        if (success)
        {
            return Ok(JsonView(true, "删除成功"));
        }
        
        return BadRequest(JsonView("删除计划失败"));
    }
} 