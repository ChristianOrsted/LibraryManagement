using System.ComponentModel.DataAnnotations;

namespace WebProject1.Models;

/// <summary>
/// 图书分类实体类 —— 如"文学"、"科技"、"历史"等
/// 每本书都必须属于一个分类（通过 Book.CategoryId 关联）
/// </summary>
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

    // 导航属性：该分类下的所有图书（一对多）
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
