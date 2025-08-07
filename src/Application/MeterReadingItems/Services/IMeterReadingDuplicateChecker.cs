using MeterReadingUploadAPI.Application.MeterReadingItems.Commands;

namespace MeterReadingUploadAPI.Application.MeterReadingItems.Services;

public interface IMeterReadingDuplicateChecker
{
    Task<List<MeterReadingItemDto>> FilterOutOlderReadsAsync(
        List<MeterReadingItemDto> items,
        UploadResult result,
        CancellationToken cancellationToken);
}
