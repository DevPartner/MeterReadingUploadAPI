using MeterReadingUploadAPI.Application.Common.Interfaces;
using MeterReadingUploadAPI.Application.MeterReadingItems.Commands;

namespace MeterReadingUploadAPI.Application.MeterReadingItems.Services;
public class MeterReadingDuplicateChecker : IMeterReadingDuplicateChecker
{
    private readonly IApplicationDbContext _context;

    public MeterReadingDuplicateChecker(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MeterReadingItemDto>> FilterOutOlderReadsAsync(
        List<MeterReadingItemDto> items,
        UploadResult result,
        CancellationToken cancellationToken)
    {
        var accountIds = items.Select(x => x.AccountId).Distinct().ToList();

        var existingReads = await _context.MeterRedingItems
            .Where(x => accountIds.Contains(x.AccountId))
            .GroupBy(x => x.AccountId)
            .Select(g => new
            {
                g.Key,
                LatestReading = g.OrderByDescending(x => x.ReadingDateTime).FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.Key, x => x.LatestReading, cancellationToken);

        var filtered = new List<MeterReadingItemDto>();

        foreach (var item in items)
        {
            if (existingReads.TryGetValue(item.AccountId, out var existing) &&
                existing != null &&
                item.ReadingDateTime <= existing.ReadingDateTime)
            {
                result.Errors.Add(new UploadError
                {
                    LineNumber = item.LineNumber,
                    ErrorMessage = $"New read is older(or equal) than the existing read for account {item.AccountId} at {existing.ReadingDateTime}"
                });
                continue;
            }

            filtered.Add(item);
        }

        return filtered;
    }
}

