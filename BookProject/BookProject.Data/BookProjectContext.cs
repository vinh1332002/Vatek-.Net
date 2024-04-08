using BookProject.Data.Configurations;
using BookProject.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookProject.Data
{
    public class BookProjectContext : DbContext
    {
        public BookProjectContext(DbContextOptions<BookProjectContext> options) : base(options)
        {
        }

        public DbSet<DocumentInformation> Documents { get; set; }
        public DbSet<BookmarkPage> Bookmarks { get; set; }
        public DbSet<DocumentPage> DocumentPages { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DocumentConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentPageConfiguration());
            modelBuilder.ApplyConfiguration(new BookmarkConfiguration());
        }
    }
}