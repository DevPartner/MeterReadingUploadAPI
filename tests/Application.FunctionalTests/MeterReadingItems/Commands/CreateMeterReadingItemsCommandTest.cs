
using System.Text;
using MeterReadingUploadAPI.Application.MeterReadingItems.Commands;
using MeterReadingUploadAPI.Domain.Entities;

namespace MeterReadingUploadAPI.Application.FunctionalTests;

using static Testing;

public class CreateMeterReadingItemsCommandTest : BaseTestFixture
{
    
    [Test]
    public async Task Should_Process_Valid_Input()
    {
        
        await AddAsync(new Account() { Id = 1, LastName = "Test", FirstName = "Test"});


        var csv = "AccountId,ReadingDateTime,MeterReadValue\n1,2022-01-01 10:00,12345";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var command = new CreateMeterReadingItemsCommand(stream);
        var result = await SendAsync(command);

        result.TotalCount.ShouldBe(1);
        result.SuccessCount.ShouldBe(1);
        result.Errors.ShouldBeEmpty();

        var count = await CountAsync<MeterReadingItem>();

        count.ShouldBe(1);
    }
    
    [Test]
    public async Task Should_Return_Error_For_Invalid_Format()
    {
        var csv = "AccountId,ReadingDateTime,MeterReadValue\n1,2022-01-01";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var command = new CreateMeterReadingItemsCommand(stream);
        var result = await SendAsync(command);

        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe("Invalid format");
    }

    [Test]
    public async Task Should_Return_Error_For_Validation_Failure()
    {
        var csv = "AccountId,ReadingDateTime,MeterReadValue\nabc,2022-01-01 10:00,99999";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var command = new CreateMeterReadingItemsCommand(stream);
        var result = await SendAsync(command);

        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe("Validation failed");
    }

    [Test]
    public async Task Should_Return_Error_For_Unknown_AccountId()
    {
        var csv = "AccountId,ReadingDateTime,MeterReadValue\n99,2022-01-01 10:00,12345";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var command = new CreateMeterReadingItemsCommand(stream);
        var result = await SendAsync(command);

        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe("AccountId not found");
    }

    [Test]
    public async Task Should_Return_Error_For_Duplicate_Readings()
    {
        var csv = @"AccountId,ReadingDateTime,MeterReadValue
            1,2022-01-01 10:00,12345
            1,2022-01-01 10:00,54321";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var command = new CreateMeterReadingItemsCommand(stream);
        var result = await SendAsync(command);

        result.TotalCount.ShouldBe(2);
        result.SuccessCount.ShouldBe(1);
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe("Duplicate reading");
    }
}
