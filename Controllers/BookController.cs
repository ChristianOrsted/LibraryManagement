using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject1.Data;
using WebProject1.Models;

namespace WebProject1.Controllers;

/// <summary>
/// 图书浏览控制器 —— 面向所有用户（无需登录）的图书搜索和详情查看
/// </summary>
public class BookController : Controller
{
    private readonly LibraryDbContext _context;
    private const int PageSize = 8;  // 每页显示 8 本书

    public BookController(LibraryDbContext context)
    {
        _context = context;
    }

    // GET /Book/Index?keyword=三体&categoryId=1&page=2 —— 图书搜索列表页
    // 支持按关键词搜索 + 按分类筛选 + 分页
    public async Task<IActionResult> Index(string? keyword, int? categoryId, int page = 1)
    {
        // 构建查询：EF Core 的 LINQ 查询是"延迟执行"的
        // 调用 Where/OrderBy 等方法时不会立即查数据库，而是构建一个查询表达式
        // 直到调用 ToListAsync/CountAsync 时才真正执行 SQL
        var query = _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookCopies)
            .AsQueryable();

        // 根据关键词筛选（支持搜索书名、作者、ISBN）
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(b =>
                b.Title.Contains(keyword) ||
                b.Author.Contains(keyword) ||
                (b.ISBN != null && b.ISBN.Contains(keyword)));
        }

        // 根据分类筛选
        if (categoryId.HasValue)
        {
            query = query.Where(b => b.CategoryId == categoryId.Value);
        }

        // 分页逻辑：
        // 1. 先计算符合条件的总记录数（用于计算总页数）
        // 2. Skip + Take 实现数据库级别的分页（翻译为 SQL 的 LIMIT/OFFSET）
        var totalItems = await query.CountAsync();
        var books = await query
            .OrderByDescending(b => b.Id)
            .Skip((page - 1) * PageSize)  // 跳过前面的页
            .Take(PageSize)                // 只取当前页的数据
            .ToListAsync();

        // 将搜索条件和结果一起打包到 ViewModel 中传给视图
        var model = new BookSearchViewModel
        {
            Keyword = keyword,
            CategoryId = categoryId,
            Books = books,
            Categories = await _context.Categories.ToListAsync(),
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
        };

        return View(model);
    }

    // GET /Book/Details/5 —— 图书详情页
    public async Task<IActionResult> Details(int id)
    {
        var book = await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookCopies)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound();  // 返回 404 页面

        // 统计可借副本数和总副本数，供页面显示"可借 2/5"
        var availableCopies = book.BookCopies.Count(c => c.Status == "Available");
        ViewBag.AvailableCopies = availableCopies;
        ViewBag.TotalCopies = book.BookCopies.Count;

        // 如果用户已登录，检查是否已经预约了这本书（用于控制页面上"预约"按钮的显示）
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId != null)
        {
            ViewBag.HasActiveReservation = await _context.Reservations
                .AnyAsync(r => r.UserId == userId && r.BookId == id && r.Status == "Pending");
        }

        return View(book);
    }
}
