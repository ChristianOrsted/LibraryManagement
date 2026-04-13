using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "分类名称不能为空")]
    [StringLength(50)]
    [Display(Name = "分类名称")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    [Display(Name = "分类描述")]
    public string? Description { get; set; }

    // Navigation
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
