using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject1.Data;
using WebProject1.Filters;
using WebProject1.Models;

namespace WebProject1.Controllers;

/// <summary>
/// 借阅控制器 —— 处理读者的借书、还书、预约、取消预约等操作
/// [AuthFilter] 表示所有操作都需要登录后才能访问
/// </summary>
[AuthFilter]
public class BorrowController : Controller
{
    private readonly LibraryDbContext _context;

    public BorrowController(LibraryDbContext context)
    {
        _context = context;
    }

    // GET /Borrow/MyBorrows —— 查看"我的借阅"列表
    public async Task<IActionResult> MyBorrows()
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        // ThenInclude 用于加载多层嵌套关联：BorrowRecord -> BookCopy -> Book -> Category
        // 这样在视图中可以直接显示"书名"和"分类"，而不只是一个外键 ID
        var records = await _context.BorrowRecords
            .Include(br => br.BookCopy!)
                .ThenInclude(bc => bc.Book!)
                    .ThenInclude(b => b.Category)
            .Where(br => br.UserId == userId)
            .OrderByDescending(br => br.BorrowDate)
            .ToListAsync();
        return View(records);
    }

    // POST /Borrow/Borrow —— 借书操作
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(int bookId)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        // 业务规则：每人最多同时借阅 5 本书
        var activeBorrows = await _context.BorrowRecords
            .CountAsync(br => br.UserId == userId && br.Status == "Borrowing");
        if (activeBorrows >= 5)
        {
            TempData["Error"] = "您当前借阅数量已达上限（5本）";
            return RedirectToAction("Details", "Book", new { id = bookId });
        }

        // 查找该书的一个可用副本
        var copy = await _context.BookCopies
            .FirstOrDefaultAsync(c => c.BookId == bookId && c.Status == "Available");

        if (copy == null)
        {
            TempData["Error"] = "暂无可借副本";
            return RedirectToAction("Details", "Book", new { id = bookId });
        }

        // 将副本状态改为"已借出"
        copy.Status = "Borrowed";

        // 创建借阅记录，借期 30 天
        var record = new BorrowRecord
        {
            UserId = userId,
            BookCopyId = copy.Id,
            BorrowDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Status = "Borrowing"
        };

        _context.BorrowRecords.Add(record);
        // SaveChangesAsync 将上面所有的变更（更新副本状态 + 新增借阅记录）一起提交到数据库
        await _context.SaveChangesAsync();

        TempData["Success"] = "借阅成功，请在30天内归还";
        return RedirectToAction("MyBorrows");
    }

    // POST /Borrow/Return —— 还书操作
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        // 查找借阅记录，并确保是当前用户自己的记录（防止越权操作）
        var record = await _context.BorrowRecords
            .Include(br => br.BookCopy)
            .FirstOrDefaultAsync(br => br.Id == id && br.UserId == userId);

        if (record == null) return NotFound();

        // 更新借阅记录和副本状态
        record.ReturnDate = DateTime.Now;
        record.Status = "Returned";
        record.BookCopy!.Status = "Available";  // 副本重新变为可借

        await _context.SaveChangesAsync();

        TempData["Success"] = "归还成功";
        return RedirectToAction("MyBorrows");
    }

    // GET /Borrow/MyReservations —— 查看"我的预约"列表
    public async Task<IActionResult> MyReservations()
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var reservations = await _context.Reservations
            .Include(r => r.Book!)
                .ThenInclude(b => b.Category)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.ReservedAt)
            .ToListAsync();
        return View(reservations);
    }

    // POST /Borrow/Reserve —— 预约图书（当所有副本都被借出时使用）
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(int bookId)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        // 防止重复预约同一本书
        var existing = await _context.Reservations
            .AnyAsync(r => r.UserId == userId && r.BookId == bookId && r.Status == "Pending");
        if (existing)
        {
            TempData["Error"] = "您已预约过这本书";
            return RedirectToAction("Details", "Book", new { id = bookId });
        }

        var reservation = new Reservation
        {
            UserId = userId,
            BookId = bookId,
            ReservedAt = DateTime.Now,
            Status = "Pending"
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        TempData["Success"] = "预约成功，有可借副本时将通知您";
        return RedirectToAction("MyReservations");
    }

    // POST /Borrow/CancelReservation —— 取消预约
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        // 查找预约记录，确保是当前用户自己的（安全校验）
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (reservation == null) return NotFound();

        reservation.Status = "Cancelled";
        await _context.SaveChangesAsync();

        TempData["Success"] = "预约已取消";
        return RedirectToAction("MyReservations");
    }
}
