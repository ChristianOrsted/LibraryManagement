using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject1.Data;
using WebProject1.Models;

namespace WebProject1.Controllers;

public class BookController : Controller
{
    private readonly LibraryDbContext _context;
    private const int PageSize = 8;

    public BookController(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? keyword, int? categoryId, int page = 1)
    {
        var query = _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookCopies)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(b =>
                b.Title.Contains(keyword) ||
                b.Author.Contains(keyword) ||
                (b.ISBN != null && b.ISBN.Contains(keyword)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(b => b.CategoryId == categoryId.Value);
        }

        var totalItems = await query.CountAsync();
        var books = await query
            .OrderByDescending(b => b.Id)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

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

    public async Task<IActionResult> Details(int id)
    {
        var book = await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookCopies)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound();

        var availableCopies = book.BookCopies.Count(c => c.Status == "Available");
        ViewBag.AvailableCopies = availableCopies;
        ViewBag.TotalCopies = book.BookCopies.Count;

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId != null)
        {
            ViewBag.HasActiveReservation = await _context.Reservations
                .AnyAsync(r => r.UserId == userId && r.BookId == id && r.Status == "Pending");
        }

        return View(book);
    }
}
