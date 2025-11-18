using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YjSite.ViewModel;

namespace YjSite.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                var result = new JsonView
                {
                    Code = StatusCodes.Status400BadRequest,
                    Msg = context.Exception.Message
                };

                context.Result = new BadRequestObjectResult(result);
                context.ExceptionHandled = true;
            }
        }
    }
} 