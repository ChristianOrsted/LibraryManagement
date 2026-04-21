using Microsoft.EntityFrameworkCore;
using WebProject1.Models;

namespace WebProject1.Data;

/// <summary>
/// EF Core 数据库上下文 —— 整个应用与数据库交互的核心类
/// DbContext 是 EF Core 的"工作单元"，负责：
///   1. 将 C# 实体类映射到数据库表
///   2. 跟踪实体的变更（增删改）
///   3. 将变更翻译为 SQL 并执行
/// </summary>
public class LibraryDbContext : DbContext
{
    // 构造函数接收配置选项（如数据库连接字符串），由依赖注入自动提供
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    // ========== DbSet 属性 ==========
    // 每个 DbSet<T> 代表数据库中的一张表，可以对其进行 LINQ 查询
    // 例如 _context.Books.Where(b => b.Title == "三体") 会生成 SELECT ... WHERE Title = '三体'
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCopy> BookCopies => Set<BookCopy>();
    public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    /// <summary>
    /// Fluent API 配置 —— 在这里定义数据库约束和表关系
    /// 这是对 Model 类上的 DataAnnotation（如 [Required]）的补充
    /// 有些配置只能在这里做（如唯一索引、复合外键等）
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- User 表配置 ----------
        modelBuilder.Entity<User>(entity =>
        {
            // 为 Username 和 Email 创建唯一索引，防止重复注册
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // ---------- 配置表之间的外键关系 ----------
        // EF Core 需要知道实体之间的关联关系，才能正确生成 JOIN 查询

        // Book -> Category：多对一（多本书属于一个分类）
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasOne(b => b.Category)        // Book 有一个 Category
                  .WithMany(c => c.Books)          // Category 有多本 Book
                  .HasForeignKey(b => b.CategoryId); // 通过 CategoryId 外键关联
        });

        // BookCopy -> Book：多对一（多个副本属于一本书）
        modelBuilder.Entity<BookCopy>(entity =>
        {
            entity.HasOne(bc => bc.Book)
                  .WithMany(b => b.BookCopies)
                  .HasForeignKey(bc => bc.BookId);
        });

        // BorrowRecord -> User 和 BookCopy：两个多对一关系
        modelBuilder.Entity<BorrowRecord>(entity =>
        {
            entity.HasOne(br => br.User)
                  .WithMany(u => u.BorrowRecords)
                  .HasForeignKey(br => br.UserId);

            entity.HasOne(br => br.BookCopy)
                  .WithMany(bc => bc.BorrowRecords)
                  .HasForeignKey(br => br.BookCopyId);
        });

        // Reservation -> User 和 Book：两个多对一关系
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasOne(r => r.User)
                  .WithMany(u => u.Reservations)
                  .HasForeignKey(r => r.UserId);

            entity.HasOne(r => r.Book)
                  .WithMany(b => b.Reservations)
                  .HasForeignKey(r => r.BookId);
        });
    }
}
