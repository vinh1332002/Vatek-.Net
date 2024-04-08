using BookProject.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookProject.Data.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<DocumentInformation>
    {
        public void Configure(EntityTypeBuilder<DocumentInformation> builder)
        {
            builder.ToTable("DocumentInformation");

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Category);
        }
    }
}
