using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度为3-50个字符")]
    [Display(Name = "用户名")]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [Display(Name = "邮箱")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "角色")]
    public string Role { get; set; } = "Reader"; // "Admin" / "Reader"

    [Display(Name = "注册时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
