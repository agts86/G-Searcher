using LineWebHookAPI.Models.Dto.Line.Hook.Messages;
using LineWebHookAPITest.Mocks.Models.Http;
using Microsoft.AspNetCore.Mvc;
using LineWebHookAPI.Models.Dto.Line.API.Requests;
using LineWebHookAPI.Models.Dto.Line.API.Messages;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Actions;
using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Constants.Line.API;
using LineWebHookAPI.Models.Dto.Line.Hook;
using LineWebHookAPI.Models.Dto.Yahoo;
using LineWebHookAPI.Controllers;
using LineWebHookAPI.Models.DB.Repositories;

namespace LineWebHookAPITest.Controllers;

public class YahooControllerTest : TestBase
{
    /// <summary>
    /// デバッグモード
    /// 結果あり
    /// </summary>
    [Fact]
    public async Task PostLocalAsyncTestDevelopmentExistResult()
    {
        var local = new LocalDto()
        {
            Feature = 
            [
                new LocalDto.FeatureInfo()
                {
                    Gid = "gid1",
                    Name = "店舗1",
                    Property = new LocalDto.FeatureInfo.PropertyInfo()
                    {
                        Address = "住所1",
                        Detail = new LocalDto.FeatureInfo.PropertyInfo.DetailInfo()
                        {
                            Image1 = "写真1",
                            YUrl = "URL1"
                        }
                    }
                },
                new LocalDto.FeatureInfo()
                {
                    Gid = "gid1",
                    Name = "店舗1-1",
                    Property = new LocalDto.FeatureInfo.PropertyInfo()
                    {
                        Address = "住所1-1",
                        Detail = new LocalDto.FeatureInfo.PropertyInfo.DetailInfo()
                        {
                            Image1 = "写真1-1",
                            YUrl = "URL1-1"
                        }
                    }
                },
                new LocalDto.FeatureInfo()
                {
                    Gid = "gid2",
                    Name = "店舗2",
                    Property = new LocalDto.FeatureInfo.PropertyInfo()
                    {
                        Address = "住所2",
                        Detail = new LocalDto.FeatureInfo.PropertyInfo.DetailInfo()
                        {
                            Image1 = "写真2",
                            YUrl = "URL2"
                        }
                    }
                }
            ]
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
        var yahooHttp = new YahooHttpMock(local);
        var lineHttpMock = new LineHttpMock();
        var yahooRepository = new YahooRepository(DbContext);
        var yahooController = new YahooController(yahooRepository, Env, yahooHttp, lineHttpMock);
        var res = await yahooController.PostLocalAsync(dto,"0106");

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

        Assert.Null(carousel.Columns[0].ThumbnailImageUrl);
        Assert.Equal("店舗1", carousel.Columns[0].Title);
        Assert.Equal("住所1", carousel.Columns[0].Text);
        Assert.IsType<UriAction[]>(carousel.Columns[0].Actions);
        var action1 = carousel.Columns[0].Actions as UriAction[];
        Assert.Single(action1);
        Assert.Equal("URL1", action1[0].Uri);
        Assert.Equal("詳細を見る", action1[0].Label);

        Assert.Null(carousel.Columns[0].ThumbnailImageUrl);
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
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, logs[0].CreatedAt.Date);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, logs[0].UpdatedAt.Date);
    }
    
    /// <summary>
    /// デバッグモード
    /// 結果なし
    /// </summary>
    [Fact]
    public async Task PostLocalAsyncTestDevelopmentNotResult()
    {
        var local = new LocalDto();

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
        var yahooHttp = new YahooHttpMock(local);
        var lineHttpMock = new LineHttpMock();
        var yahooRepository = new YahooRepository(DbContext);
        var yahooController = new YahooController(yahooRepository, Env, yahooHttp, lineHttpMock);
        var res = await yahooController.PostLocalAsync(dto,"0106");

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
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, logs[0].CreatedAt.Date);
        Assert.Equal(DateTime.Now.ToUniversalTime().AddHours(JapanKind).Date, logs[0].UpdatedAt.Date);
    }
}
