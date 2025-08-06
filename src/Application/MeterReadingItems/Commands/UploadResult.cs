namespace MeterReadingUploadAPI.Application.MeterReadingItems.Commands;

public class UploadResult
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount => Errors.Count;
    public List<UploadError> Errors { get; set; } = new();
}

public class UploadError
{
    public int LineNumber { get; set; }
    public required string LineContent { get; set; }
    public required string ErrorMessage { get; set; }
}
