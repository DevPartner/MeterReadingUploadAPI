using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.MeterReading.Queries.UploadMeterReading;

public record UploadMeterReadingQuery : IRequest<MeterReadingVm>
{
}

public class UploadMeterReadingQueryValidator : AbstractValidator<UploadMeterReadingQuery>
{
    public UploadMeterReadingQueryValidator()
    {
    }
}

public class UploadMeterReadingQueryHandler : IRequestHandler<UploadMeterReadingQuery, MeterReadingVm>
{
    private readonly IApplicationDbContext _context;

    public UploadMeterReadingQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MeterReadingVm> Handle(UploadMeterReadingQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
