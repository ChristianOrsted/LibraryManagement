using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "预约用户")]
    public int UserId { get; set; }

    [Required]
    [Display(Name = "预约图书")]
    public int BookId { get; set; }

    [Display(Name = "预约时间")]
    public DateTime ReservedAt { get; set; } = DateTime.Now;

    [Display(Name = "状态")]
    public string Status { get; set; } = "Pending"; // "Pending" / "Fulfilled" / "Cancelled"

    // Navigation
    public User? User { get; set; }
    public Book? Book { get; set; }
}
