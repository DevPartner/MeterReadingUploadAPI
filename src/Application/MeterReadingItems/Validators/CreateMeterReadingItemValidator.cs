using MeterReadingUploadAPI.Application.Common.Interfaces;
using MeterReadingUploadAPI.Application.MeterReadingItems.Commands;

namespace MeterReadingUploadAPI.Application.MeterReadingItems.Validators;

public class CreateMeterReadingItemValidator : AbstractValidator<MeterReadingItemDto>
{
    public CreateMeterReadingItemValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.ReadingDateTime)
            .NotEmpty().WithMessage("Reading time is required");

        RuleFor(x => x.MeterReadValue)
            .Matches(@"^\d{5}$", System.Text.RegularExpressions.RegexOptions.Compiled)
            .WithMessage("MeterReadValue must be a 5-digit number");

    }
}

