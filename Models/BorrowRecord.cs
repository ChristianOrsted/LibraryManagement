using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

public class BorrowRecord
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "借阅用户")]
    public int UserId { get; set; }

    [Required]
    [Display(Name = "图书副本")]
    public int BookCopyId { get; set; }

    [Display(Name = "借出时间")]
    public DateTime BorrowDate { get; set; } = DateTime.Now;

    [Display(Name = "应还时间")]
    public DateTime DueDate { get; set; }

    [Display(Name = "归还时间")]
    public DateTime? ReturnDate { get; set; }

    [Display(Name = "状态")]
    public string Status { get; set; } = "Borrowing"; // "Borrowing" / "Returned" / "Overdue"

    // Navigation
    public User? User { get; set; }
    public BookCopy? BookCopy { get; set; }
}
