using LineWebHookAPI.Constants;
using LineWebHookAPI.Controllers;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Services.Managed;
using Microsoft.AspNetCore.Mvc;

namespace LineWebHookAPITest.Controllers;

public class ManagedControllerTest : TestBase
{
    [Fact]
    public async Task GetGourmetLogAsyncTestLocation()
    {
        var gourmetLogs = new GourmetLocationLog[]
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
        DbContext.GourmetLocationLogs.AddRange(gourmetLogs);
        await DbContext.SaveChangesAsync();
        var managedRepository = new ManagedRepository(DbContext);
        var controller = new ManagedController(managedRepository);
        var result = await controller.GetGourmetLogsAsync(MessageTypes.location);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<GourmetLocationLog[]>((result as OkObjectResult).Value);
        var content = (result as OkObjectResult).Value as GourmetLocationLog[];
        Assert.Equal(2, content.Length);
        Assert.Equal(0, content[0].Lat);
        Assert.Equal(1, content[0].Lng);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[0].CreatedAt.Date);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[0].UpdatedAt.Date);
        Assert.Equal(1, content[1].Lat);
        Assert.Equal(0, content[1].Lng);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[1].CreatedAt.Date);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[1].UpdatedAt.Date);
    }

    [Fact]
    public async Task GetGourmetLogAsyncTestWord()
    {
        var gourmetLogs = new GourmetWordLog[]
        {
            new()
            {
                Text = "二郎系"
            },
            new()
            {
                Text = "家系"
            }
        };
        DbContext.GourmetWordLogs.AddRange(gourmetLogs);
        await DbContext.SaveChangesAsync();
        var managedRepository = new ManagedRepository(DbContext);
        var controller = new ManagedController(managedRepository);
        var result = await controller.GetGourmetLogsAsync(MessageTypes.text);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<GourmetWordLog[]>((result as OkObjectResult).Value);
        var content = (result as OkObjectResult).Value as GourmetWordLog[];
        Assert.Equal(2, content.Length);
        Assert.Equal("二郎系", content[0].Text);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[0].CreatedAt.Date);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[0].UpdatedAt.Date);
        Assert.Equal("家系", content[1].Text);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[1].CreatedAt.Date);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[1].UpdatedAt.Date);
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
        var managedRepository = new ManagedRepository(DbContext);
        var controller = new ManagedController(managedRepository);
        var result = await controller.GetErrorLogAsync();
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ErrorLog[]>((result as OkObjectResult).Value);
        var content = (result as OkObjectResult).Value as ErrorLog[];
        Assert.Equal(2, content.Length);
        Assert.Equal("Error1", content[0].Contents);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[0].CreatedAt.Date);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[0].UpdatedAt.Date);
        Assert.Equal("Error2", content[1].Contents);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[1].CreatedAt.Date);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, content[1].UpdatedAt.Date);
    }
}
