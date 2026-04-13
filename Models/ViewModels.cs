using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "用户名不能为空")]
    [Display(Name = "用户名")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度为3-50个字符")]
    [Display(Name = "用户名")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度至少6个字符")]
    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "请确认密码")]
    [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
    [DataType(DataType.Password)]
    [Display(Name = "确认密码")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [Display(Name = "邮箱")]
    public string Email { get; set; } = string.Empty;
}

public class BookSearchViewModel
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public List<Book> Books { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
}

public class DashboardViewModel
{
    public int TotalBooks { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueCount { get; set; }
    public List<BorrowRecord> RecentBorrows { get; set; } = new();
}
