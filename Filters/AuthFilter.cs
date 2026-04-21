using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebProject1.Filters;

/// <summary>
/// 登录验证过滤器 —— 以特性（Attribute）的形式标记在 Controller 或 Action 上
/// 被标记的 Controller/Action 在执行前会先检查用户是否已登录
/// 用法：在 Controller 类上加 [AuthFilter] 即可
/// </summary>
public class AuthFilter : ActionFilterAttribute
{
    // OnActionExecuting 在 Controller 的 Action 方法执行之前被调用
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // 从 Session 中读取 UserId，如果为 null 说明用户未登录
        var userId = context.HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            // 未登录则重定向到登录页面，不再执行原本的 Action
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
        base.OnActionExecuting(context);
    }
}

/// <summary>
/// 管理员权限过滤器 —— 检查当前用户是否是管理员
/// 通常与 AuthFilter 配合使用：先检查登录，再检查管理员权限
/// 用法：[AuthFilter] [AdminFilter]
/// </summary>
public class AdminFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // 从 Session 中读取用户角色
        var role = context.HttpContext.Session.GetString("UserRole");
        if (role != "Admin")
        {
            // 非管理员则重定向到首页
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
        base.OnActionExecuting(context);
    }
}
