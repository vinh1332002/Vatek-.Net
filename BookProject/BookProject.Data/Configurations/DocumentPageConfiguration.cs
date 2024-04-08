using BookProject.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookProject.Data.Configurations
{
    public class DocumentPageConfiguration : IEntityTypeConfiguration<DocumentPage>
    {
        public void Configure(EntityTypeBuilder<DocumentPage> builder)
        {
            builder.ToTable("DocumentPage");

            builder.HasKey(e => e.Id);
        }
    }
}
