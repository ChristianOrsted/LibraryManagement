using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject1.Data;
using WebProject1.Models;

namespace WebProject1.Controllers;

/// <summary>
/// 账户控制器 —— 处理用户登录、注册、登出
/// 这是系统中唯一不需要登录就能访问的 Controller（没有 [AuthFilter]）
/// </summary>
public class AccountController : Controller
{
    // 通过构造函数注入数据库上下文（依赖注入模式）
    private readonly LibraryDbContext _context;

    public AccountController(LibraryDbContext context)
    {
        _context = context;
    }

    // GET /Account/Login —— 显示登录页面
    public IActionResult Login()
    {
        // 如果用户已登录，直接跳转到首页，不显示登录表单
        if (HttpContext.Session.GetInt32("UserId") != null)
            return RedirectToAction("Index", "Home");
        return View();
    }

    // POST /Account/Login —— 处理登录表单提交
    [HttpPost]
    [ValidateAntiForgeryToken]  // 防止 CSRF 攻击：验证表单中的防伪令牌
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // ModelState.IsValid 会根据 ViewModel 上的验证特性（[Required] 等）自动校验
        if (!ModelState.IsValid) return View(model);

        // 根据用户名查找用户
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
        // 验证密码：用 BCrypt 对比用户输入的明文密码和数据库中的哈希值
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError("", "用户名或密码错误");
            return View(model);
        }

        // 登录成功：将用户信息存入 Session（服务端会话）
        // 后续请求通过读取 Session 来判断用户身份
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("UserRole", user.Role);

        // 管理员跳转到后台仪表盘，普通读者跳转到首页
        return user.Role == "Admin"
            ? RedirectToAction("Dashboard", "Admin")
            : RedirectToAction("Index", "Home");
    }

    // GET /Account/Register —— 显示注册页面
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
            return RedirectToAction("Index", "Home");
        return View();
    }

    // POST /Account/Register —— 处理注册表单提交
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // 检查用户名是否已被占用
        if (await _context.Users.AnyAsync(u => u.Username == model.Username))
        {
            ModelState.AddModelError("Username", "用户名已存在");
            return View(model);
        }

        // 检查邮箱是否已被注册
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "邮箱已被注册");
            return View(model);
        }

        // 创建新用户，密码经过 BCrypt 哈希后再存储（安全要求：永远不存明文密码）
        var user = new User
        {
            Username = model.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Email = model.Email,
            Role = "Reader",  // 新注册用户默认为普通读者
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // TempData 用于在重定向后的下一个请求中显示一次性消息
        TempData["Success"] = "注册成功，请登录";
        return RedirectToAction("Login");
    }

    // GET /Account/Logout —— 登出
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();  // 清除 Session 中的所有数据
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
