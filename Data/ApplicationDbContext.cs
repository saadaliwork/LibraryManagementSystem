using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }

        public List<Book> GetBooksByPublishedYear(int publishedYear)
        {
            return Books.FromSqlRaw("EXEC GetBooksByPublishedYear @PublishedYear", new[] { new Microsoft.Data.SqlClient.SqlParameter("@PublishedYear", publishedYear) }).ToList();
        }

        public int GetTotalBooks()
        {
            return Books.Count();
        }

        public int GetTotalMembers()
        {
            return Members.Count();
        }

        public Member? GetMostRecentMember() 
        {
            return Members.OrderByDescending(m => m.JoinDate).FirstOrDefault();
        }
    }
}