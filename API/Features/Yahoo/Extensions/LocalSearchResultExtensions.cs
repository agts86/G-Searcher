using LineDevSdk.DTO.Commons.Messages.Actions;
using LineDevSdk.DTO.Commons.Messages.Templates;
using YahooDeveloperApiClient.YOLP.Response;

namespace Features.Yahoo.Extensions;

/// <summary>
/// LocalSearchResult拡張クラス
/// </summary>
internal static class LocalSearchResultExtensions
{
    /// <summary>
    /// カルーセルテンプレートの配列に変換
    /// </summary>
    /// <returns></returns>
    public static CarouselTemplate.Column[] ToCarouselTemplateColumns(this LocalSearchResult dto)
    {
        return
        [
            .. dto.Feature?
                .GroupBy(x => x.Gid)
                .Select
                (
                    x => new CarouselTemplate.Column
                    {
                        Title = x.First().Name,
                        Text = x.First().Property.Address,
                        Actions = new UriAction[]
                        {
                            new ()
                            {
                                Label = "詳細を見る",
                                Uri = x.First().Property.Detail.GetExtraValue<string>("YUrl")
                            }
                        }
                    }
                )
                .Take(10) ?? []
        ];
    }
}
