using MeterReadingUploadAPI.Domain.Entities;

namespace MeterReadingUploadAPI.Application.MeterReadingItems.Commands;
public class MeterReadingItemDto
{
    public int AccountId { get; set; }
    public DateTime ReadingDateTime { get; set; }
    public string MeterReadValue { get; set; } = string.Empty;
    public int LineNumber { get; internal set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<MeterReadingItemDto, MeterReadingItem>();
        }
    }
}
