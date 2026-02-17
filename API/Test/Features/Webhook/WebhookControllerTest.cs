using Microsoft.EntityFrameworkCore;
using Features.Webhook.Constants;
using YahooDeveloperApiClient.YOLP.Response;
using Features.Webhook;
using Infrastructure.Models.DB.Repositories;
using LineDevSdk.DTO.Commons.Messages;
using LineDevSdk.DTO.MessagingAPIs;
using LineDevSdk.DTO.Commons.Messages.Templates;
using LineDevSdk.DTO.Commons.Messages.Actions;
using LineDevSdk.DTO.WebHooks;
using LineDevSdk.DTO.WebHooks.Events;
using Features.Webhook.Services;
using System.Text.Json;
using Features.Webhook.Dto;
using Shared.Jobs;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace Test.Features.Webhook;

public class WebhookControllerTest : TestBase
{
    [Fact]
    public async Task AcceptLocalAsyncTest()
    {
        var dto = new WebHook()
        {
            Events =
            [
                new MessageEvent()
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
        LocalJobDto capturedJob = null;
        var queueMock = new Mock<IBackgroundJobQueue<LocalJobDto>>();
        queueMock.Setup(x => x.EnqueueAsync(It.IsAny<LocalJobDto>(), It.IsAny<CancellationToken>()))
            .Callback<LocalJobDto, CancellationToken>((job, _) => capturedJob = job)
            .Returns(ValueTask.CompletedTask);
        var webhookServiceMock = new Mock<IWebhookService>();
        webhookServiceMock.Setup(x => x.AcceptLocalAsync(It.IsAny<LocalJobDto>()))
            .Returns(Task.CompletedTask);
        var controller = new WebhookController(webhookServiceMock.Object);

        var result = await controller.AcceptLocalAsync(dto, "0106", queueMock.Object);

        var accepted = Assert.IsType<AcceptedResult>(result);
        Assert.Equal(202, accepted.StatusCode);
        Assert.NotNull(capturedJob);
        Assert.Equal("0106", capturedJob.GenreCode);
        Assert.Same(dto, capturedJob.WebHook);
        var payloadJson = JsonSerializer.Serialize(accepted.Value);
        Assert.Contains(capturedJob.Id.ToString(), payloadJson);

        queueMock.Verify(x => x.EnqueueAsync(It.IsAny<LocalJobDto>(), It.IsAny<CancellationToken>()), Times.Once);
        webhookServiceMock.Verify
        (
            x => x.AcceptLocalAsync(It.Is<LocalJobDto>(job => job.Id == capturedJob.Id)),
            Times.Once
        );
    }

    /// <summary>
    /// デバッグモード
    /// 結果あり
    /// </summary>
    [Fact]
    public async Task PostLocalAsyncTestDevelopmentExistResult()
    {
        var local = new LocalSearchResult()
        {
            Feature = 
            [
                new LocalSearchResult.FeatureInfo()
                {
                    Gid = "gid1",
                    Name = "店舗1",
                    Property = new LocalSearchResult.FeatureInfo.PropertyInfo()
                    {
                        Address = "住所1",
                        Detail = new LocalSearchResult.FeatureInfo.PropertyInfo.DetailInfo()
                        {
                            Image1 = "写真1",
                            Extra = new Dictionary<string, JsonElement>()
                            {
                                { "YUrl", JsonDocument.Parse("\"URL1\"").RootElement }
                            }
                        }
                    }
                },
                new LocalSearchResult.FeatureInfo()
                {
                    Gid = "gid1",
                    Name = "店舗1-1",
                    Property = new LocalSearchResult.FeatureInfo.PropertyInfo()
                    {
                        Address = "住所1-1",
                        Detail = new LocalSearchResult.FeatureInfo.PropertyInfo.DetailInfo()
                        {
                            Image1 = "写真1-1",
                            Extra = new Dictionary<string, JsonElement>()
                            {
                                { "YUrl", JsonDocument.Parse("\"URL1-1\"").RootElement }
                            }
                        }
                    }
                },
                new LocalSearchResult.FeatureInfo()
                {
                    Gid = "gid2",
                    Name = "店舗2",
                    Property = new LocalSearchResult.FeatureInfo.PropertyInfo()
                    {
                        Address = "住所2",
                        Detail = new LocalSearchResult.FeatureInfo.PropertyInfo.DetailInfo()
                        {
                            Image1 = "写真2",
                            Extra = new Dictionary<string, JsonElement>()
                            {
                                { "YUrl", JsonDocument.Parse("\"URL2\"").RootElement }
                            }
                        }
                    }
                }
            ]
        };

        var dto = new WebHook()
        {
            Events =
            [
                new MessageEvent()
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
        var YahooClient = CreateYahooClientMock(local);
        var LineMessagingClientMock = CreateLineMessagingClientMock();
        var yahooRepository = new YahooRepository(DbContext, YahooClient, LineMessagingClientMock);
        var webhookService = new WebhookService(yahooRepository, Env, Configuration);
        var webhookController = new WebhookController(webhookService);
        var res = await webhookController.PostLocalAsync(dto,"0106");

        var contents = Assert.IsType<Reply[]>(res.Value);

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
        Assert.Equal(DateTime.Now, logs[0].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, logs[0].UpdatedAt, TimeSpan.FromMinutes(1));
    }
    
    /// <summary>
    /// デバッグモード
    /// 結果なし
    /// </summary>
    [Fact]
    public async Task PostLocalAsyncTestDevelopmentNotResult()
    {
        var local = new LocalSearchResult();

        var dto = new WebHook()
        {
            Events =
            [
                new MessageEvent()
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
        var YahooClient = CreateYahooClientMock(local);
        var LineMessagingClientMock = CreateLineMessagingClientMock();
        var yahooRepository = new YahooRepository(DbContext, YahooClient, LineMessagingClientMock);
        var webhookService = new WebhookService(yahooRepository, Env, Configuration);
        var webhookController = new WebhookController(webhookService);
        var res = await webhookController.PostLocalAsync(dto,"0106");

        var contents = Assert.IsType<Reply[]>(res.Value);
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
        Assert.Equal(DateTime.Now, logs[0].CreatedAt, TimeSpan.FromMinutes(1));
        Assert.Equal(DateTime.Now, logs[0].UpdatedAt, TimeSpan.FromMinutes(1));
    }
}
