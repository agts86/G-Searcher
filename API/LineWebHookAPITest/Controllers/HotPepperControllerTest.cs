using LineWebHookAPI.Models.Dto.Line.Hook.Messages;
using LineWebHookAPI.Models.Dto.HotPeppers;
using LineWebHookAPITest.Mocks.Models.Http;
using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Models.Dto.Line.API.Requests;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Actions;
using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Constants.Line.API;
using LineWebHookAPI.Constants.HotPepper;
using LineWebHookAPI.Models.Dto.Line.Hook;
using LineWebHookAPI.Controllers;
using LineWebHookAPI.Models.Services.HotPeppers;
using LineWebHookAPI.Models.DB.Repositories;

namespace LineWebHookAPITest.Controllers;

public class HotPepperControllerTest : TestBase
{
    /// <summary>
    /// デバッグモード
    /// 結果あり
    /// </summary>
    [Fact]
    public async Task PostGourmetLocationAsyncTestDevelopmentExistResult()
    {
        var hotPepperGourmetResponseDto = new HotPepperGourmetResponseDto()
        {
            Results = new HotPepperGourmetResponseDto.Result()
            {
                Shops =
                [
                    new HotPepperGourmetResponseDto.Result.Shop()
                    {
                        Name = "店舗1",
                        Address = "住所1",
                        Photo = new HotPepperGourmetResponseDto.Result.Shop.PhotoInfo()
                        {
                            Pc = new HotPepperGourmetResponseDto.Result.Shop.PhotoInfo.PhotoPc()
                            {
                                Large = "写真1"
                            }
                        },
                        Urls = new HotPepperGourmetResponseDto.Result.Shop.UrlInfo()
                        {
                            Pc = "URL1"
                        }
                    },
                    new HotPepperGourmetResponseDto.Result.Shop()
                    {
                        Name = "店舗2",
                        Address = "住所2",
                        Photo = new HotPepperGourmetResponseDto.Result.Shop.PhotoInfo()
                        {
                            Pc = new HotPepperGourmetResponseDto.Result.Shop.PhotoInfo.PhotoPc()
                            {
                                Large = "写真2"
                            }
                        },
                        Urls = new HotPepperGourmetResponseDto.Result.Shop.UrlInfo()
                        {
                            Pc = "URL2"
                        }
                    }
                ]
            }
        };

        var dto = new GourmetGettingDto()
        {
            Events =
            [
                new ()
                {
                    ReplyToken = "replyToken",
                    Message = new LocationMessage()
                    {
                        Latitude = 35.681236,
                        Longitude = 139.767125
                    }
                }
            ]
        };

        Env.EnvironmentName = "Development";
        var http = new HttpAdapterMock(hotPepperGourmetResponseDto);
        var hotPepperRepository = new HotPepperRepository(DbContext);
        var hotPepperService = new HotPepperService(Configuration, hotPepperRepository, Env, http);
        var baseControllerRepository = new BaseControllerRepository(DbContext);
        var hotPepperController = new HotPepperController(hotPepperService, Env,baseControllerRepository);
        var res = await hotPepperController.PostGourmetLocationAsync(dto,GenreCode.G013);

        Assert.IsType<OkObjectResult>(res);
        Assert.IsType<Reply[]>((res as OkObjectResult).Value);

        var contents = (res as OkObjectResult).Value as Reply[];
        Assert.Single(contents);
        Assert.Equal("replyToken", contents[0].ReplyToken);
        Assert.Single(contents[0].Messages);
        Assert.IsType<TemplateMessage>(contents[0].Messages[0]);
        
        var message = contents[0].Messages[0] as TemplateMessage;
        Assert.Equal("template", message.Type);
        Assert.Equal("検索結果", message.AltText);
        Assert.IsType<CarouselTemplate>(message.Template);

        var carousel = message.Template as CarouselTemplate;
        Assert.Equal(2, carousel.Columns.Length);

        Assert.Equal("写真1", carousel.Columns[0].ThumbnailImageUrl);
        Assert.Equal("店舗1", carousel.Columns[0].Title);
        Assert.Equal("住所1", carousel.Columns[0].Text);
        Assert.IsType<UriAction[]>(carousel.Columns[0].Actions);
        var action1 = carousel.Columns[0].Actions as UriAction[];
        Assert.Single(action1);
        Assert.Equal("URL1", action1[0].Uri);
        Assert.Equal("詳細を見る", action1[0].Label);

        Assert.Equal("写真2", carousel.Columns[1].ThumbnailImageUrl);
        Assert.Equal("店舗2", carousel.Columns[1].Title);
        Assert.Equal("住所2", carousel.Columns[1].Text);
        Assert.IsType<UriAction[]>(carousel.Columns[1].Actions);
        var action2 = carousel.Columns[1].Actions as UriAction[];
        Assert.Single(action2);
        Assert.Equal("URL2", action2[0].Uri);
        Assert.Equal("詳細を見る", action2[0].Label);

        var logs = await DbContext.GourmetLocationLogs.ToArrayAsync();
        Assert.Single(logs);
        Assert.Equal(35.681236, logs[0].Lat);
        Assert.Equal(139.767125, logs[0].Lng);
        Assert.Equal(DateTime.Today, logs[0].CreatedAt.Date);
        Assert.Equal(DateTime.Today, logs[0].UpdatedAt.Date);
    }
    
    /// <summary>
    /// デバッグモード
    /// 結果なし
    /// </summary>
    [Fact]
    public async Task PostGourmetLocationAsyncTestDevelopmentNotResult()
    {
        var hotPepperGourmetResponseDto = new HotPepperGourmetResponseDto()
        {
            Results = new HotPepperGourmetResponseDto.Result()
            {
                Shops = []
            }
        };

        var dto = new GourmetGettingDto()
        {
            Events =
            [
                new ()
                {
                    ReplyToken = "replyToken",
                    Message = new LocationMessage()
                    {
                        Latitude = 35.681236,
                        Longitude = 139.767125
                    }
                }
            ]
        };

        Env.EnvironmentName = "Development";
        var http = new HttpAdapterMock(hotPepperGourmetResponseDto);
        var hotPepperRepository = new HotPepperRepository(DbContext);
        var hotPepperService = new HotPepperService(Configuration, hotPepperRepository, Env, http);
        var baseControllerRepository = new BaseControllerRepository(DbContext);
        var hotPepperController = new HotPepperController(hotPepperService, Env,baseControllerRepository);
        var res = await hotPepperController.PostGourmetLocationAsync(dto,GenreCode.G013);

        Assert.IsType<OkObjectResult>(res);
        Assert.IsType<Reply[]>((res as OkObjectResult).Value);

        var contents = (res as OkObjectResult).Value as Reply[];
        Assert.Single(contents);
        Assert.Equal("replyToken", contents[0].ReplyToken);
        Assert.Single(contents[0].Messages);
        Assert.IsType<TextV2Message>(contents[0].Messages[0]);

        var message = contents[0].Messages[0] as TextV2Message;
        Assert.Equal("textV2", message.Type);
        Assert.Equal(MessageTexts.NotFound, message.Text);

        var logs = await DbContext.GourmetLocationLogs.ToArrayAsync();
        Assert.Single(logs);
        Assert.Equal(35.681236, logs[0].Lat);
        Assert.Equal(139.767125, logs[0].Lng);
        Assert.Equal(DateTime.Today, logs[0].CreatedAt.Date);
        Assert.Equal(DateTime.Today, logs[0].UpdatedAt.Date);
    }

    /// <summary>
    /// プロダクトモード
    /// 正常終了 
    /// </summary>
    [Fact]
    public async Task PostGourmetLocationAsyncTestProduction()
    {
        var hotPepperGourmetResponseDto = new HotPepperGourmetResponseDto()
        {
            Results = new HotPepperGourmetResponseDto.Result()
            {
                Shops = []
            }
        };

        var dto = new GourmetGettingDto()
        {
            Events =
            [
                new ()
                {
                    ReplyToken = "replyToken",
                    Message = new LocationMessage()
                    {
                        Latitude = 35.681236,
                        Longitude = 139.767125
                    }
                }
            ]
        };

        Env.EnvironmentName = "Production";
        var http = new HttpAdapterMock(hotPepperGourmetResponseDto);
        var hotPepperRepository = new HotPepperRepository(DbContext);
        var hotPepperService = new HotPepperService(Configuration, hotPepperRepository, Env, http);
        var baseControllerRepository = new BaseControllerRepository(DbContext);
        var hotPepperController = new HotPepperController(hotPepperService, Env,baseControllerRepository);
        var res = await hotPepperController.PostGourmetLocationAsync(dto,GenreCode.G013);

        Assert.IsType<OkResult>(res);
    }

    /// <summary>
    /// プロダクトモード
    /// 200だけど実はエラー
    /// </summary>
    [Theory]
    [InlineData("1000")]
    [InlineData("2000")]
    [InlineData("3000")]
    public async Task PostGourmetLocationAsyncTestProductionStatusException(string code)
    {
        var hotPepperGourmetResponseDto = new HotPepperGourmetResponseDto()
        {
            Results = new HotPepperGourmetResponseDto.Result()
            {
                Error = new HotPepperErrorResponseDto.ErrorInfo()
                {
                    Code = code,
                    Message = "Bad Request"
                }
            }
        };

        var dto = new GourmetGettingDto()
        {
            Events =
            [
                new ()
                {
                    ReplyToken = "replyToken",
                    Message = new LocationMessage()
                    {
                        Latitude = 35.681236,
                        Longitude = 139.767125
                    }
                }
            ]
        };

        Env.EnvironmentName = "Production";
        var http = new HttpAdapterMock(hotPepperGourmetResponseDto);
        var hotPepperRepository = new HotPepperRepository(DbContext);
        var hotPepperService = new HotPepperService(Configuration, hotPepperRepository, Env, http);
        var baseControllerRepository = new BaseControllerRepository(DbContext);
        var hotPepperController = new HotPepperController(hotPepperService, Env,baseControllerRepository);
        var res = await hotPepperController.PostGourmetLocationAsync(dto,GenreCode.G013);

        Assert.IsType<OkResult>(res);
    }
}
