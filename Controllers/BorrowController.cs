using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject1.Data;
using WebProject1.Filters;
using WebProject1.Models;

namespace WebProject1.Controllers;

[AuthFilter]
public class BorrowController : Controller
{
    private readonly LibraryDbContext _context;

    public BorrowController(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> MyBorrows()
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var records = await _context.BorrowRecords
            .Include(br => br.BookCopy!)
                .ThenInclude(bc => bc.Book!)
                    .ThenInclude(b => b.Category)
            .Where(br => br.UserId == userId)
            .OrderByDescending(br => br.BorrowDate)
            .ToListAsync();
        return View(records);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(int bookId)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

        // Check if user has too many active borrows
        var activeBorrows = await _context.BorrowRecords
            .CountAsync(br => br.UserId == userId && br.Status == "Borrowing");
        if (activeBorrows >= 5)
        {
            TempData["Error"] = "您当前借阅数量已达上限（5本）";
            return RedirectToAction("Details", "Book", new { id = bookId });
        }

        var copy = await _context.BookCopies
            .FirstOrDefaultAsync(c => c.BookId == bookId && c.Status == "Available");

        if (copy == null)
        {
            TempData["Error"] = "暂无可借副本";
            return RedirectToAction("Details", "Book", new { id = bookId });
        }

        copy.Status = "Borrowed";

        var record = new BorrowRecord
        {
            UserId = userId,
            BookCopyId = copy.Id,
            BorrowDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Status = "Borrowing"
        };

        _context.BorrowRecords.Add(record);
        await _context.SaveChangesAsync();

        TempData["Success"] = "借阅成功，请在30天内归还";
        return RedirectToAction("MyBorrows");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var record = await _context.BorrowRecords
            .Include(br => br.BookCopy)
            .FirstOrDefaultAsync(br => br.Id == id && br.UserId == userId);

        if (record == null) return NotFound();

        record.ReturnDate = DateTime.Now;
        record.Status = "Returned";
        record.BookCopy!.Status = "Available";

        await _context.SaveChangesAsync();

        TempData["Success"] = "归还成功";
        return RedirectToAction("MyBorrows");
    }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(int bookId)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (reservation == null) return NotFound();

        reservation.Status = "Cancelled";
        await _context.SaveChangesAsync();

        TempData["Success"] = "预约已取消";
        return RedirectToAction("MyReservations");
    }
}
