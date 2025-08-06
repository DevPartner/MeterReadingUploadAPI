using MeterReadingUploadAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterReadingUploadAPI.Infrastructure.Data.Configurations;

public class MeterReadingsConfiguration : IEntityTypeConfiguration<MeterReadingItem>
{
    public void Configure(EntityTypeBuilder<MeterReadingItem> builder)
    {
        builder.Property(t => t.MeterReadValue)
            .HasMaxLength(200)
            .IsRequired();
    }
}
