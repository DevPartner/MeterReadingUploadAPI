using MeterReadingUploadAPI.Application.Common.Interfaces;
using MeterReadingUploadAPI.Application.MeterReadingItems.Commands;

namespace MeterReadingUploadAPI.Application.MeterReadingItems.Services;
public class MeterReadingValidatorService : IMeterReadingValidatorService
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<MeterReadingItemDto> _validator;

    public MeterReadingValidatorService(IApplicationDbContext context, IValidator<MeterReadingItemDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<List<MeterReadingItemDto>> ParseAndValidateAsync(
        Stream stream,
        UploadResult result,
        CancellationToken cancellationToken)
    {
        var validAccountIds = new HashSet<int>(
            await _context.Accounts.AsNoTracking().Select(x => x.Id).ToListAsync(cancellationToken));

        var seen = new HashSet<(int, DateTime)>();
        var items = new List<MeterReadingItemDto>();
        using var reader = new StreamReader(stream);

        int lineNumber = 0;
        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync();
            lineNumber++;

            if (line == null || lineNumber == 1 && line.StartsWith("AccountId")) continue;

            var parts = line.Split(',');
            if (parts.Length < 3)
            {
                result.Errors.Add(new UploadError { LineNumber = lineNumber, ErrorMessage = "Malformed line" });
                continue;
            }

            if (!int.TryParse(parts[0], out int accountId) ||
                !DateTime.TryParse(parts[1], out DateTime readingTime) ||
                string.IsNullOrWhiteSpace(parts[2]))
            {
                result.Errors.Add(new UploadError { LineNumber = lineNumber, ErrorMessage = "Invalid format" });
                continue;
            }

            var dto = new MeterReadingItemDto
            {
                AccountId = accountId,
                ReadingDateTime = readingTime,
                MeterReadValue = parts[2],
                LineNumber = lineNumber
            };

            var validation = await _validator.ValidateAsync(dto, cancellationToken);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    result.Errors.Add(new UploadError { LineNumber = lineNumber, ErrorMessage = error.ErrorMessage });
                }
                continue;
            }

            if (!validAccountIds.Contains(accountId))
            {
                result.Errors.Add(new UploadError { LineNumber = lineNumber, ErrorMessage = "AccountId not found" });
                continue;
            }

            if (!seen.Add((accountId, readingTime)))
            {
                result.Errors.Add(new UploadError { LineNumber = lineNumber, ErrorMessage = "Duplicate reading in the file" });
                continue;
            }

            items.Add(dto);
        }

        return items;
    }
}
