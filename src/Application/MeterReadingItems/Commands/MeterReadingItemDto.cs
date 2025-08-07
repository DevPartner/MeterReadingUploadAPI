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
            CreateMap<MeterReadingItemDto, MeterReadingItem>().ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Account, opt => opt.Ignore()) 
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModified, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DomainEvents, opt => opt.Ignore());
        }
    }
}
