namespace MeterReadingUploadAPI.Domain.Entities;
public class MeterReadingItem : BaseAuditableEntity
{
    public int AccountId { get; set; }
    public DateTime ReadingDateTime { get; set; }
    public string MeterReadValue { get; set; } = "";

    public Account? Account { get; set; }
}
