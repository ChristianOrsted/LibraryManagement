using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

/// <summary>
/// 用户实体类 —— 对应数据库中的 Users 表
/// 系统中有两种角色：Admin（管理员）和 Reader（普通读者）
/// </summary>
public class User
{
    // 主键，EF Core 会自动识别名为 "Id" 的属性作为主键，数据库中自增
    public int Id { get; set; }

    // [Required] = 必填字段，表单提交时如果为空会触发验证错误
    // [StringLength] = 限制字符串长度，同时影响数据库列定义和前端验证
    // [Display] = 在视图中显示的中文标签名
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度为3-50个字符")]
    [Display(Name = "用户名")]
    public string Username { get; set; } = string.Empty;

    // 存储的是经过 BCrypt 哈希后的密码，而不是明文密码（安全要求）
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [Display(Name = "邮箱")]
    public string Email { get; set; } = string.Empty;

    // 用户角色：默认为 "Reader"（普通读者），管理员为 "Admin"
    [Display(Name = "角色")]
    public string Role { get; set; } = "Reader";

    [Display(Name = "注册时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // ========== 导航属性 ==========
    // 导航属性是 EF Core 的概念，表示实体之间的关联关系
    // 一个用户可以有多条借阅记录（一对多关系）
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    // 一个用户可以有多条预约记录（一对多关系）
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
