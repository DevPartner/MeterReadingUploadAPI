using MeterReadingUploadAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeterReadingUploadAPI.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.Property(t => t.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.LastName)
            .HasMaxLength(100)
            .IsRequired();
    }
}
