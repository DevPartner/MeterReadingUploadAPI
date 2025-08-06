using System.Text.RegularExpressions;
using MeterReadingUploadAPI.Application.Common.Interfaces;
using MeterReadingUploadAPI.Domain.Entities;

namespace MeterReadingUploadAPI.Application.MeterReadingItems.Commands;
public class CreateMeterReadingItemsCommand : IRequest<UploadResult>
{
    public Stream Stream { get; }

    public CreateMeterReadingItemsCommand(Stream stream)
    {
        Stream = stream;
    }
}


public class CreateMeterReadingItemsCommandHandler : IRequestHandler<CreateMeterReadingItemsCommand, UploadResult>
{
    private readonly IApplicationDbContext _context;

    public CreateMeterReadingItemsCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<UploadResult> Handle(CreateMeterReadingItemsCommand request, CancellationToken cancellationToken)
    {
        var result = new UploadResult();
        var validAccountIds = new HashSet<int>(                                             //possible issue for large amount of accounts
            await _context.Accounts.Select(a => a.Id).ToListAsync(cancellationToken));

        var seen = new HashSet<(int, DateTime)>();
        var itemsToAdd = new List<MeterReadingItem>();
        

        using var reader = new StreamReader(request.Stream);
        int lineNumber = 0;
        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync();
            lineNumber++;

            // Skip header line
            if (line ==null || (lineNumber == 1 && line.StartsWith("AccountId"))) continue;

            var parts = line?.Split(',');
            if (parts?.Length < 3)
            {
                result.Errors.Add(new UploadError { LineNumber = lineNumber, LineContent = line!, ErrorMessage = "Invalid format" });
                continue;
            }

            if (!int.TryParse(parts![0], out int accountId) ||
                !DateTime.TryParse(parts[1], out var readingTime) ||
                string.IsNullOrWhiteSpace(parts[2]) ||
                !Regex.IsMatch(parts[2], @"^\d{5}$"))
            {
                result.Errors.Add(new UploadError { LineNumber = lineNumber, LineContent = line!, ErrorMessage = "Validation failed" });
                continue;
            }

            if (!validAccountIds.Contains(accountId))
            {
                result.Errors.Add(new UploadError { LineNumber = lineNumber, LineContent = line!, ErrorMessage = "AccountId not found" });
                continue;
            }

            if (!seen.Add((accountId, readingTime)))
            {
                result.Errors.Add(new UploadError { LineNumber = lineNumber, LineContent = line!, ErrorMessage = "Duplicate reading" });
                continue;
            }

            itemsToAdd.Add(new MeterReadingItem
            {
                AccountId = accountId,
                ReadingDateTime = readingTime,
                MeterReadValue = parts[2]
            });


        }

        _context.MeterRedingItems.AddRange(itemsToAdd);
        await _context.SaveChangesAsync(cancellationToken);

        result.TotalCount = lineNumber - 1;
        result.SuccessCount = itemsToAdd.Count;

        return result;
    }
}
