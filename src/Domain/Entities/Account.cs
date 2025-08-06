namespace MeterReadingUploadAPI.Domain.Entities;
public class Account : BaseAuditableEntity
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
}
