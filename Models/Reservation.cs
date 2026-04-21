using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

/// <summary>
/// 预约实体类 —— 当所有副本都被借出时，读者可以预约某本书
/// 预约的是 Book（书目），而不是具体的 BookCopy（副本）
/// </summary>
public class Reservation
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "预约用户")]
    public int UserId { get; set; }

    // 注意：预约是针对 Book（书目）的，不是针对具体副本
    [Required]
    [Display(Name = "预约图书")]
    public int BookId { get; set; }

    [Display(Name = "预约时间")]
    public DateTime ReservedAt { get; set; } = DateTime.Now;

    // 预约状态：
    //   "Pending"   = 等待中（还没有可借副本）
    //   "Fulfilled" = 已完成（已借到书）
    //   "Cancelled" = 已取消
    [Display(Name = "状态")]
    public string Status { get; set; } = "Pending";

    // ========== 导航属性 ==========
    public User? User { get; set; }
    public Book? Book { get; set; }
}
