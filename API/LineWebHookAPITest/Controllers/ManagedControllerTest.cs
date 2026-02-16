using LineWebHookAPI.Constants;
using LineWebHookAPI.Controllers;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.DB.Tables;
using LineWebHookAPI.Models.Services;
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
        var managedService = new ManagedService(managedRepository);
        var controller = new ManagedController(managedService);
        var result = await controller.GetGourmetLogsAsync(MessageTypes.location);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<GourmetLocationLog[]>((result as OkObjectResult).Value);
        var content = (result as OkObjectResult).Value as GourmetLocationLog[];
        Assert.Equal(2, content.Length);
        Assert.Equal(0, content[0].Lat);
        Assert.Equal(1, content[0].Lng);
        Assert.Equal(DateTime.Now, content[0].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, content[0].UpdatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(1, content[1].Lat);
        Assert.Equal(0, content[1].Lng);
        Assert.Equal(DateTime.Now, content[1].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, content[1].UpdatedAt, TimeSpan.FromMinutes(1));
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
        var managedService = new ManagedService(managedRepository);
        var controller = new ManagedController(managedService);
        var result = await controller.GetGourmetLogsAsync(MessageTypes.text);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<GourmetWordLog[]>((result as OkObjectResult).Value);
        var content = (result as OkObjectResult).Value as GourmetWordLog[];
        Assert.Equal(2, content.Length);
        Assert.Equal("二郎系", content[0].Text);
        Assert.Equal(DateTime.Now, content[0].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, content[0].UpdatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal("家系", content[1].Text);
        Assert.Equal(DateTime.Now, content[1].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, content[1].UpdatedAt, TimeSpan.FromMinutes(1));
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
        var managedService = new ManagedService(managedRepository);
        var controller = new ManagedController(managedService);
        var result = await controller.GetErrorLogAsync();

        var content = Assert.IsType<ErrorLog[]>(result.Value);
        Assert.Equal(2, content.Length);
        Assert.Equal("Error1", content[0].Contents);
        Assert.Equal(DateTime.Now, content[0].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, content[0].UpdatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal("Error2", content[1].Contents);
        Assert.Equal(DateTime.Now, content[1].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, content[1].UpdatedAt, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetJobLogAsyncTest()
    {
        var jobLogs = new JobLog[]
        {
            new()
            {
                IsSuccess = true,
                Contents = "job1",
                Info = null
            },
            new()
            {
                IsSuccess = false,
                Contents = "job2",
                Info = "error"
            }
        };
        DbContext.JobLogs.AddRange(jobLogs);
        await DbContext.SaveChangesAsync();
        var managedRepository = new ManagedRepository(DbContext);
        var managedService = new ManagedService(managedRepository);
        var controller = new ManagedController(managedService);
        var result = await controller.GetJobLogAsync();

        var content = Assert.IsType<JobLog[]>(result.Value);
        Assert.Equal(2, content.Length);
        Assert.True(content[0].IsSuccess);
        Assert.Equal("job1", content[0].Contents);
        Assert.Null(content[0].Info);
        Assert.Equal(DateTime.Now, content[0].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, content[0].UpdatedAt, TimeSpan.FromMinutes(1));
        Assert.False(content[1].IsSuccess);
        Assert.Equal("job2", content[1].Contents);
        Assert.Equal("error", content[1].Info);
        Assert.Equal(DateTime.Now, content[1].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, content[1].UpdatedAt, TimeSpan.FromMinutes(1));
    }
}
