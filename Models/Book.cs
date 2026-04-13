using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

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

    [StringLength(20)]
    [Display(Name = "ISBN")]
    public string? ISBN { get; set; }

    [StringLength(100)]
    [Display(Name = "出版社")]
    public string? Publisher { get; set; }

    [Display(Name = "出版年份")]
    [Range(1000, 2100)]
    public int? PublishYear { get; set; }

    [StringLength(500)]
    [Display(Name = "简介")]
    public string? Description { get; set; }

    [StringLength(200)]
    [Display(Name = "封面图片")]
    public string? CoverImage { get; set; }

    [Required(ErrorMessage = "请选择分类")]
    [Display(Name = "分类")]
    public int CategoryId { get; set; }

    // Navigation
    public Category? Category { get; set; }
    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
