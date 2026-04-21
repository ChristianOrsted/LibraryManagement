using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

/// <summary>
/// 图书副本实体类 —— 代表一本书的实际物理馆藏
/// 每一条 BookCopy 记录对应图书馆中一本具体的实体书
/// 用户借阅的是 BookCopy（副本），而不是 Book（书目信息）
/// </summary>
public class BookCopy
{
    public int Id { get; set; }

    // 外键：指向 Book 表，表示这个副本属于哪本书
    [Required]
    [Display(Name = "所属图书")]
    public int BookId { get; set; }

    // 副本状态：
    //   "Available" = 可借（在架上）
    //   "Borrowed"  = 已借出
    //   "Damaged"   = 损坏
    [Required]
    [Display(Name = "状态")]
    public string Status { get; set; } = "Available";

    // 馆内存放位置，如 "A区-1排"
    [StringLength(50)]
    [Display(Name = "馆内位置")]
    public string? Location { get; set; }

    // ========== 导航属性 ==========
    public Book? Book { get; set; }
    // 这个副本的借阅历史（一个副本可以被多次借出和归还）
    public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
}
