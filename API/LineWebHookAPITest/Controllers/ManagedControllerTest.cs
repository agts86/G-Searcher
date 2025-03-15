
using LineWebHookAPI.Controllers;
using LineWebHookAPI.Models.DB.Tables;
using Microsoft.AspNetCore.Mvc;

namespace LineWebHookAPITest.Controllers;

public class ManagedControllerTest : TestBase
{
    [Fact]
    public async Task GetGourmetLogAsyncTest()
    {
        var gourmetLogs = new GourmetLog[]
        {
            new()
            {
                Lat = 0,
                Lng = 1
            },
            new()
            {
                Lat = 1,
                Lng = 0
            }
        };
        DbContext.GourmetLogs.AddRange(gourmetLogs);
        await DbContext.SaveChangesAsync();
        var controller = new ManagedController(DbContext);
        var result = await controller.GetGourmetLogAsync();
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<GourmetLog[]>((result as OkObjectResult).Value);
        var content = (result as OkObjectResult).Value as GourmetLog[];
        Assert.Equal(2, content.Length);
        Assert.Equal(0, content[0].Lat);
        Assert.Equal(1, content[0].Lng);
        Assert.Equal(DateTime.Today, content[0].CreatedAt.Date);
        Assert.Equal(DateTime.Today, content[0].UpdatedAt.Date);
        Assert.Equal(1, content[1].Lat);
        Assert.Equal(0, content[1].Lng);
        Assert.Equal(DateTime.Today, content[1].CreatedAt.Date);
        Assert.Equal(DateTime.Today, content[1].UpdatedAt.Date);
    }

    [Fact]
    public async Task GetGErrorLogAsyncTest()
    {
        var errorLogs = new ErrorLog[]
        {
            new()
            {
                Contents = "Error1"
            },
            new()
            {
                Contents = "Error2"
            }
        };
        DbContext.ErrorLogs.AddRange(errorLogs);
        await DbContext.SaveChangesAsync();
        var controller = new ManagedController(DbContext);
        var result = await controller.GetErrorLogAsync();
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ErrorLog[]>((result as OkObjectResult).Value);
        var content = (result as OkObjectResult).Value as ErrorLog[];
        Assert.Equal(2, content.Length);
        Assert.Equal("Error1", content[0].Contents);
        Assert.Equal(DateTime.Today, content[0].CreatedAt.Date);
        Assert.Equal(DateTime.Today, content[0].UpdatedAt.Date);
        Assert.Equal("Error2", content[1].Contents);
        Assert.Equal(DateTime.Today, content[1].CreatedAt.Date);
        Assert.Equal(DateTime.Today, content[1].UpdatedAt.Date);
    }
}
