using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

public class BookCopy
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "所属图书")]
    public int BookId { get; set; }

    [Required]
    [Display(Name = "状态")]
    public string Status { get; set; } = "Available"; // "Available" / "Borrowed" / "Damaged"

    [StringLength(50)]
    [Display(Name = "馆内位置")]
    public string? Location { get; set; }

    // Navigation
    public Book? Book { get; set; }
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
