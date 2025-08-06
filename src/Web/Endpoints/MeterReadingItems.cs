using MeterReadingUploadAPI.Application.MeterReadingItems.Commands;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MeterReadingUploadAPI.Web.Endpoints;

public class MeterReadingItems : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateMeterReadingItems, "/meter-reading-uploads")/*.RequireAuthorization()*/;
    }

    public async Task<Results<BadRequest, Ok<UploadResult>>> CreateMeterReadingItems(ISender sender, IFormFile file)
    {
        if (file == null || file.Length == 0) return TypedResults.BadRequest();


        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var command = new CreateMeterReadingItemsCommand(memoryStream);
        var result = await sender.Send(command);

        return TypedResults.Ok(result);
    }

}
