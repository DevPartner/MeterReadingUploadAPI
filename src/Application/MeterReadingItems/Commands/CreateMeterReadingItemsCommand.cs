using System.Text.RegularExpressions;
using MeterReadingUploadAPI.Application.Common.Interfaces;
using MeterReadingUploadAPI.Application.MeterReadingItems.Services;
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
    private readonly IMapper _mapper;
    private readonly IMeterReadingValidatorService _validatorService;
    private readonly IMeterReadingDuplicateChecker _duplicateChecker;


    public CreateMeterReadingItemsCommandHandler(IApplicationDbContext context, IMapper mapper,
        IMeterReadingValidatorService validatorService,
        IMeterReadingDuplicateChecker duplicateChecker)
    {
        _context = context;
        _mapper = mapper;
        _validatorService = validatorService;
        _duplicateChecker = duplicateChecker;
    }


    public async Task<UploadResult> Handle(CreateMeterReadingItemsCommand request, CancellationToken cancellationToken)
    {
        var result = new UploadResult();

        var rawItems = await _validatorService.ParseAndValidateAsync(request.Stream, result, cancellationToken);
        if (!rawItems.Any()) return result;

        var filteredItems = await _duplicateChecker.FilterOutOlderReadsAsync(rawItems, result, cancellationToken);

        var entitiesToAdd = _mapper.Map<List<MeterReadingItem>>(filteredItems);
        _context.MeterRedingItems.AddRange(entitiesToAdd);
        await _context.SaveChangesAsync(cancellationToken);

        result.TotalCount = rawItems.Count;
        result.SuccessCount = filteredItems.Count;
        return result;
    }
}
