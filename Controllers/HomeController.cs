using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject1.Data;
using WebProject1.Models;

namespace WebProject1.Controllers;

/// <summary>
/// 首页控制器 —— 处理网站首页和通用页面
/// 不需要登录即可访问（没有 [AuthFilter]）
/// </summary>
public class HomeController : Controller
{
    private readonly LibraryDbContext _context;

    public HomeController(LibraryDbContext context)
    {
        _context = context;
    }

    // GET / 或 GET /Home/Index —— 网站首页
    public async Task<IActionResult> Index()
    {
        // ViewBag 是一种动态对象，用于从 Controller 向 View 传递少量临时数据
        // 适合传递不值得为其创建 ViewModel 的简单值
        ViewBag.TotalBooks = await _context.Books.CountAsync();
        ViewBag.TotalCopies = await _context.BookCopies.CountAsync();
        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.ActiveBorrows = await _context.BorrowRecords.CountAsync(br => br.Status == "Borrowing");

        // 查询最新添加的 6 本书，用于首页展示
        // Include() 是"预加载"（Eager Loading），一次性加载关联数据，避免 N+1 查询问题
        // 如果不 Include，访问 book.Category 时会是 null
        var latestBooks = await _context.Books
            .Include(b => b.Category)     // 加载图书的分类信息
            .Include(b => b.BookCopies)   // 加载图书的副本信息（用于显示可借数量）
            .OrderByDescending(b => b.Id) // 按 Id 降序 = 最新添加的排前面
            .Take(6)                      // 只取前 6 条
            .ToListAsync();               // 执行查询，转为 List

        return View(latestBooks);  // 将数据传给 Views/Home/Index.cshtml 渲染
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // 全局错误处理页面
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
