using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

/// <summary>
/// 图书实体类 —— 代表一本书的"元信息"（书名、作者等）
/// 注意：Book 是抽象的"书目信息"，实际的物理馆藏由 BookCopy 表示
/// 例如《三体》这本书（Book）可能有 3 本实体副本（BookCopy）
/// </summary>
public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "书名不能为空")]
    [StringLength(200)]
    [Display(Name = "书名")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "作者不能为空")]
    [StringLength(100)]
    [Display(Name = "作者")]
    public string Author { get; set; } = string.Empty;

    // string? 表示可空类型，即这些字段在数据库中允许为 NULL
    [StringLength(20)]
    [Display(Name = "ISBN")]
    public string? ISBN { get; set; }

    [StringLength(100)]
    [Display(Name = "出版社")]
    public string? Publisher { get; set; }

    [Display(Name = "出版年份")]
    [Range(1000, 2100)]  // 限制输入范围，防止输入不合理的年份
    public int? PublishYear { get; set; }

    [StringLength(500)]
    [Display(Name = "简介")]
    public string? Description { get; set; }

    [StringLength(200)]
    [Display(Name = "封面图片")]
    public string? CoverImage { get; set; }

    // 外键：指向 Category 表的 Id，表示这本书属于哪个分类
    [Required(ErrorMessage = "请选择分类")]
    [Display(Name = "分类")]
    public int CategoryId { get; set; }

    // ========== 导航属性 ==========
    // 所属分类（多对一：多本书属于同一个分类）
    public Category? Category { get; set; }
    // 这本书的所有物理副本（一对多：一本书有多个副本）
    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
    // 这本书的所有预约记录
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
