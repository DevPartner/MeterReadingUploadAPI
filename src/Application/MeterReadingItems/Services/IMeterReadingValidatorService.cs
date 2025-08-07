using MeterReadingUploadAPI.Application.MeterReadingItems.Commands;

namespace MeterReadingUploadAPI.Application.MeterReadingItems.Services;

public interface IMeterReadingValidatorService
{
    Task<List<MeterReadingItemDto>> ParseAndValidateAsync(
        Stream stream,
        UploadResult result,
        CancellationToken cancellationToken);
}
