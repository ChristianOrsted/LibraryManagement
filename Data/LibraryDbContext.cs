using Microsoft.EntityFrameworkCore;
using WebProject1.Models;

namespace WebProject1.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCopy> BookCopies => Set<BookCopy>();
    public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasOne(b => b.Category)
                  .WithMany(c => c.Books)
                  .HasForeignKey(b => b.CategoryId);
        });

        modelBuilder.Entity<BookCopy>(entity =>
        {
            entity.HasOne(bc => bc.Book)
                  .WithMany(b => b.BookCopies)
                  .HasForeignKey(bc => bc.BookId);
        });

        modelBuilder.Entity<BorrowRecord>(entity =>
        {
            entity.HasOne(br => br.User)
                  .WithMany(u => u.BorrowRecords)
                  .HasForeignKey(br => br.UserId);

            entity.HasOne(br => br.BookCopy)
                  .WithMany(bc => bc.BorrowRecords)
                  .HasForeignKey(br => br.BookCopyId);
        });

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
