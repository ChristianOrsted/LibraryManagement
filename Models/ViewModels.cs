using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

// ============================
// ViewModel（视图模型）
// ViewModel 不对应数据库表，而是专门为页面显示和表单提交设计的数据结构
// 它们是 Controller 和 View 之间传递数据的"信使"
// ============================

/// <summary>
/// 登录表单的 ViewModel —— 只需要用户名和密码
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "用户名不能为空")]
    [Display(Name = "用户名")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [DataType(DataType.Password)]  // 告诉视图引擎渲染为密码输入框（显示为 ****）
    [Display(Name = "密码")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 注册表单的 ViewModel —— 包含用户名、密码、确认密码和邮箱
/// </summary>
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

    // [Compare] 会自动校验此字段的值是否和 Password 一致
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

/// <summary>
/// 图书搜索页面的 ViewModel —— 包含搜索条件 + 搜索结果 + 分页信息
/// 一个 ViewModel 同时承载"用户输入的筛选条件"和"查询出来的结果"
/// </summary>
public class BookSearchViewModel
{
    public string? Keyword { get; set; }       // 搜索关键词（可按书名/作者/ISBN搜索）
    public int? CategoryId { get; set; }        // 按分类筛选
    public List<Book> Books { get; set; } = new();          // 当前页的搜索结果
    public List<Category> Categories { get; set; } = new(); // 所有分类（用于渲染下拉框）
    public int CurrentPage { get; set; } = 1;   // 当前页码
    public int TotalPages { get; set; }         // 总页数
}

/// <summary>
/// 管理员仪表盘的 ViewModel —— 汇总系统统计数据
/// </summary>
public class DashboardViewModel
{
    public int TotalBooks { get; set; }         // 图书总数
    public int TotalUsers { get; set; }         // 用户总数
    public int ActiveBorrows { get; set; }      // 当前正在借阅中的记录数
    public int OverdueCount { get; set; }       // 逾期未还的记录数
    public List<BorrowRecord> RecentBorrows { get; set; } = new(); // 最近的借阅记录
}
