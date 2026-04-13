using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProject1.Data;
using WebProject1.Models;

namespace WebProject1.Controllers;

public class HomeController : Controller
{
    private readonly LibraryDbContext _context;

    public HomeController(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalBooks = await _context.Books.CountAsync();
        ViewBag.TotalCopies = await _context.BookCopies.CountAsync();
        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.ActiveBorrows = await _context.BorrowRecords.CountAsync(br => br.Status == "Borrowing");

        var latestBooks = await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookCopies)
            .OrderByDescending(b => b.Id)
            .Take(6)
            .ToListAsync();

        return View(latestBooks);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
