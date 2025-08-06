using MeterReadingUploadAPI.Domain.Entities;

namespace MeterReadingUploadAPI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }

    DbSet<MeterReadingItem> MeterRedingItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
