using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Serilog;

using SqlSugar;

using YjSite.ViewModel;


// 控制器基类
namespace YjSite.Controllers
{
    [ApiController]
    [Route("api")]
    public class BaseController : ControllerBase
    {
        protected virtual void Logs(string str)
        {
            Log.Error(str);
        }
        protected virtual JsonView JsonView(object obj)
        {
            return new JsonView { Code = StatusCodes.Status200OK, Msg = "操作成功", Data = obj };
        }
        protected virtual JsonView JsonView(object obj, string msg)
        {
            return new JsonView { Code = StatusCodes.Status200OK, Msg = msg, Data = obj };
        }
        protected virtual JsonView JsonView(object obj, int count)
        {
            return new JsonView { Code = StatusCodes.Status200OK, Msg = "操作成功", Data = obj, Count = count };
        }
        protected virtual JsonView JsonView(string msg)
        {
            return new JsonView { Code = StatusCodes.Status400BadRequest, Msg = msg };
        }
        protected virtual JsonView JsonView(bool s)
        {
            if (s)
            {
                return new JsonView { Code = StatusCodes.Status200OK, Msg = "操作成功" };
            }
            else
            {
                return new JsonView { Code = StatusCodes.Status400BadRequest, Msg = "操作失败" };
            }
        }
        protected virtual JsonView JsonView(bool s, string msg)
        {
            if (s)
            {
                return new JsonView { Code = StatusCodes.Status200OK, Msg = msg };
            }
            else
            {
                return new JsonView { Code = StatusCodes.Status400BadRequest, Msg = msg };
            }
        }
    }
}
