using LibraryManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Data;


public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Book configuration
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ISBN).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Publisher).HasMaxLength(200);
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasIndex(e => e.ISBN).IsUnique();
        });

        // BookCopy configuration
        modelBuilder.Entity<BookCopy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CopyNumber).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Book)
                .WithMany(b => b.BookCopies)
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Loan configuration
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(l => l.Id);

            entity.Property(l => l.UserId)
                .IsRequired();

            entity.Property(l => l.BookCopyId)
                .IsRequired();

            entity.Property(l => l.LoanDate)
                .IsRequired();

            entity.Property(l => l.Status)
                .IsRequired();

            entity.Property(l => l.FineAmount)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);  

            entity.HasOne(l => l.ApprovedBy)
                .WithMany() 
                .HasForeignKey(l => l.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict) 
                .IsRequired(false); 

            entity.HasOne(l => l.ReturnConfirmedBy)
                .WithMany()  
                .HasForeignKey(l => l.ReturnConfirmedByUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(l => l.BookCopy)
                .WithMany(bc => bc.Loans)
                .HasForeignKey(l => l.BookCopyId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Book)
                .WithMany(b => b.Reservations)
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => new { e.BookId, e.QueuePosition });
        });

        // ApplicationUser configuration
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.TotalFines).HasColumnType("decimal(18,2)");
        });
    }
}
