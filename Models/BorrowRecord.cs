using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

/// <summary>
/// 借阅记录实体类 —— 记录"谁在什么时候借了哪本书的哪个副本"
/// 这是连接 User 和 BookCopy 的核心业务表
/// </summary>
public class BorrowRecord
{
    public int Id { get; set; }

    // 外键：借阅用户
    [Required]
    [Display(Name = "借阅用户")]
    public int UserId { get; set; }

    // 外键：借阅的具体副本（注意是副本 BookCopy，不是 Book）
    [Required]
    [Display(Name = "图书副本")]
    public int BookCopyId { get; set; }

    [Display(Name = "借出时间")]
    public DateTime BorrowDate { get; set; } = DateTime.Now;

    // 应还日期，借阅时设为 30 天后（见 BorrowController.Borrow）
    [Display(Name = "应还时间")]
    public DateTime DueDate { get; set; }

    // 实际归还时间，为 null 表示尚未归还
    [Display(Name = "归还时间")]
    public DateTime? ReturnDate { get; set; }

    // 借阅状态：
    //   "Borrowing" = 借阅中（尚未归还）
    //   "Returned"  = 已归还
    //   "Overdue"   = 逾期未还
    [Display(Name = "状态")]
    public string Status { get; set; } = "Borrowing";

    // ========== 导航属性 ==========
    public User? User { get; set; }
    public BookCopy? BookCopy { get; set; }
}
