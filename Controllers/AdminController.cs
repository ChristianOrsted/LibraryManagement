using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebProject1.Data;
using WebProject1.Filters;
using WebProject1.Models;

namespace WebProject1.Controllers;

[AuthFilter]
[AdminFilter]
public class AdminController : Controller
{
    private readonly LibraryDbContext _context;

    public AdminController(LibraryDbContext context)
    {
        _context = context;
    }

    // Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var model = new DashboardViewModel
        {
            TotalBooks = await _context.Books.CountAsync(),
            TotalUsers = await _context.Users.CountAsync(),
            ActiveBorrows = await _context.BorrowRecords.CountAsync(br => br.Status == "Borrowing"),
            OverdueCount = await _context.BorrowRecords.CountAsync(br => br.Status == "Borrowing" && br.DueDate < DateTime.Now),
            RecentBorrows = await _context.BorrowRecords
                .Include(br => br.User)
                .Include(br => br.BookCopy!).ThenInclude(bc => bc.Book)
                .OrderByDescending(br => br.BorrowDate)
                .Take(10)
                .ToListAsync()
        };
        return View(model);
    }

    // ===== Book CRUD =====
    public async Task<IActionResult> Books()
    {
        var books = await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookCopies)
            .OrderByDescending(b => b.Id)
            .ToListAsync();
        return View(books);
    }

    public async Task<IActionResult> CreateBook()
    {
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBook(Book book)
    {
        ModelState.Remove("Category");
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(book);
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        TempData["Success"] = "图书添加成功";
        return RedirectToAction("Books");
    }

    public async Task<IActionResult> EditBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", book.CategoryId);
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBook(Book book)
    {
        ModelState.Remove("Category");
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", book.CategoryId);
            return View(book);
        }

        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        TempData["Success"] = "图书更新成功";
        return RedirectToAction("Books");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books
            .Include(b => b.BookCopies)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (book == null) return NotFound();

        var hasBorrowed = await _context.BorrowRecords
            .AnyAsync(br => br.BookCopy!.BookId == id && br.Status == "Borrowing");
        if (hasBorrowed)
        {
            TempData["Error"] = "该图书有未归还的借阅记录，无法删除";
            return RedirectToAction("Books");
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        TempData["Success"] = "图书删除成功";
        return RedirectToAction("Books");
    }

    // ===== Category CRUD =====
    public async Task<IActionResult> Categories()
    {
        var categories = await _context.Categories
            .Include(c => c.Books)
            .ToListAsync();
        return View(categories);
    }

    public IActionResult CreateCategory()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        if (!ModelState.IsValid) return View(category);

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "分类添加成功";
        return RedirectToAction("Categories");
    }

    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(Category category)
    {
        if (!ModelState.IsValid) return View(category);

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "分类更新成功";
        return RedirectToAction("Categories");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();

        if (category.Books.Any())
        {
            TempData["Error"] = "该分类下还有图书，无法删除";
            return RedirectToAction("Categories");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "分类删除成功";
        return RedirectToAction("Categories");
    }

    // ===== BookCopy Management =====
    public async Task<IActionResult> BookCopies(int bookId)
    {
        var book = await _context.Books
            .Include(b => b.BookCopies)
            .FirstOrDefaultAsync(b => b.Id == bookId);
        if (book == null) return NotFound();

        ViewBag.Book = book;
        return View(book.BookCopies.ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCopy(int bookId, string? location)
    {
        var copy = new BookCopy
        {
            BookId = bookId,
            Status = "Available",
            Location = location
        };
        _context.BookCopies.Add(copy);
        await _context.SaveChangesAsync();
        TempData["Success"] = "副本添加成功";
        return RedirectToAction("BookCopies", new { bookId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCopyStatus(int id, string status, int bookId)
    {
        var copy = await _context.BookCopies.FindAsync(id);
        if (copy == null) return NotFound();

        copy.Status = status;
        await _context.SaveChangesAsync();
        TempData["Success"] = "状态更新成功";
        return RedirectToAction("BookCopies", new { bookId });
    }

    // ===== User Management =====
    public async Task<IActionResult> Users()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserRole(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Role = user.Role == "Admin" ? "Reader" : "Admin";
        await _context.SaveChangesAsync();
        TempData["Success"] = $"用户 {user.Username} 的角色已更新为 {user.Role}";
        return RedirectToAction("Users");
    }

    // ===== Borrow Management =====
    public async Task<IActionResult> BorrowRecords()
    {
        var records = await _context.BorrowRecords
            .Include(br => br.User)
            .Include(br => br.BookCopy!).ThenInclude(bc => bc.Book)
            .OrderByDescending(br => br.BorrowDate)
            .ToListAsync();
        return View(records);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForceReturn(int id)
    {
        var record = await _context.BorrowRecords
            .Include(br => br.BookCopy)
            .FirstOrDefaultAsync(br => br.Id == id);
        if (record == null) return NotFound();

        record.ReturnDate = DateTime.Now;
        record.Status = "Returned";
        record.BookCopy!.Status = "Available";
        await _context.SaveChangesAsync();

        TempData["Success"] = "强制归还成功";
        return RedirectToAction("BorrowRecords");
    }
}
