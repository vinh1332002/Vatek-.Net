using BookProject.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookProject.Data.Configurations
{
    public class BookmarkConfiguration : IEntityTypeConfiguration<BookmarkPage>
    {
        public void Configure(EntityTypeBuilder<BookmarkPage> builder)
        {
            builder.ToTable("BookmarkPage");

            builder.HasKey(e => e.Id);

            builder.HasOne(b => b.DocumentPage)
                .WithOne(p => p.Bookmark)
                .HasForeignKey<BookmarkPage>(b => b.DocumentPageId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
