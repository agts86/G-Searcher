using LineWebHookAPI.Models.Dto.Line.Hook.Messages;
using LineWebHookAPI.Models.Dto.HotPeppers;
using LineWebHookAPITest.Mocks.Controllers;
using LineWebHookAPITest.Mocks.Models.Http;
using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Models.Dto.Line.API.Requests;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Actions;
using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Constants.Line.API;
using LineWebHookAPI.Constants.HotPepper;

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
        var hotPepperController = new HotPepperControllerMock(Configuration, DbContext, Env, http);
        var res = await hotPepperController.PostGourmetLocationAsync(dto,GenreCode.G013);

        Assert.IsType<OkObjectResult>(res);
        Assert.IsType<Reply>((res as OkObjectResult).Value);

        var contents = (res as OkObjectResult).Value as Reply;
        Assert.Equal("replyToken", contents.ReplyToken);
        Assert.IsType<TemplateMessage[]>(contents.Messages);
        Assert.Single(contents.Messages);

        var message = contents.Messages as TemplateMessage[];
        Assert.Equal("template", message[0].Type);
        Assert.Equal("検索結果", message[0].AltText);
        Assert.IsType<CarouselTemplate>(message[0].Template);

        var carousel = message[0].Template as CarouselTemplate;
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

        var logs = await DbContext.GourmetLogs.ToArrayAsync();
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
        var hotPepperController = new HotPepperControllerMock(Configuration, DbContext, Env, http);
        var res = await hotPepperController.PostGourmetLocationAsync(dto,GenreCode.G013);

        Assert.IsType<OkObjectResult>(res);
        Assert.IsType<Reply>((res as OkObjectResult).Value);

        var contents = (res as OkObjectResult).Value as Reply;
        Assert.Equal("replyToken", contents.ReplyToken);
        Assert.IsType<TextMessage[]>(contents.Messages);
        Assert.Single(contents.Messages);

        var message = contents.Messages as TextMessage[];
        Assert.Equal("text", message[0].Type);
        Assert.Equal(MessageTexts.NotFound, message[0].Text);

        var logs = await DbContext.GourmetLogs.ToArrayAsync();
        Assert.Single(logs);
        Assert.Equal(35.681236, logs[0].Lat);
        Assert.Equal(139.767125, logs[0].Lng);
        Assert.Equal(DateTime.Today, logs[0].CreatedAt.Date);
        Assert.Equal(DateTime.Today, logs[0].UpdatedAt.Date);
    }

    /// <summary>
    /// プロダクトモード
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
        var hotPepperController = new HotPepperControllerMock(Configuration, DbContext, Env, http);
        var res = await hotPepperController.PostGourmetLocationAsync(dto,GenreCode.G013);

        Assert.IsType<OkResult>(res);
    }
}
