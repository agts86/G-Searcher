using System.Text.Json.Serialization;
using LineWebHookAPI.Models.Dto.Line.API.Templates;
using LineWebHookAPI.Models.Dto.Line.API.Actions;

namespace LineWebHookAPI.Models.Dto.HotPeppers;

/// <summary>
/// グルメサーチAPIレスポンスのDTO
/// </summary>
public class HotPepperGourmetResponseDto
{
    /// <summary>
    /// 結果
    /// </summary>
    public Result Results { get; set; }

    /// <summary>
    /// 結果
    /// </summary>
    public class Result : HotPepperErrorResponseDto
    {
        /// <summary>
        /// APIのバージョン情報
        /// </summary>
        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// 検索結果の全件数
        /// </summary>
        [JsonPropertyName("results_available")]
        public int ResultsAvailable { get; set; }

        /// <summary>
        /// このレスポンスに含まれる検索結果の件数
        /// </summary>
        [JsonPropertyName("results_returned")]
        public string ResultsReturned { get; set; }

        /// <summary>
        /// 検索結果の開始位置
        /// </summary>
        [JsonPropertyName("results_start")]
        public int ResultsStart { get; set; }

        /// <summary>
        /// 店舗情報の配列
        /// </summary>
        [JsonPropertyName("shop")]
        public Shop[] Shops { get; set; }

        /// <summary>
        /// 店舗情報クラス
        /// </summary>
        public class Shop
        {
            /// <summary>
            /// 店舗ID
            /// </summary>
            [JsonPropertyName("id")]
            public string Id { get; set; }

            /// <summary>
            /// 掲載店名
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; }

            /// <summary>
            /// ロゴ画像URL
            /// </summary>
            [JsonPropertyName("logo_image")]
            public string LogoImage { get; set; }

            /// <summary>
            /// 住所
            /// </summary>
            [JsonPropertyName("address")]
            public string Address { get; set; }

            /// <summary>
            /// 最寄駅名
            /// </summary>
            [JsonPropertyName("station_name")]
            public string StationName { get; set; }

            /// <summary>
            /// 緯度
            /// </summary>
            [JsonPropertyName("lat")]
            public double Latitude { get; set; }

            /// <summary>
            /// 経度
            /// </summary>
            [JsonPropertyName("lng")]
            public double Longitude { get; set; }

            /// <summary>
            /// お店のジャンル情報
            /// </summary>
            [JsonPropertyName("genre")]
            public GenreInfo Genre { get; set; }

            /// <summary>
            /// キャッチコピー
            /// </summary>
            [JsonPropertyName("catch")]
            public string Catch { get; set; }

            /// <summary>
            /// 交通アクセス
            /// </summary>
            [JsonPropertyName("access")]
            public string Access { get; set; }

            /// <summary>
            /// 店舗のURL情報
            /// </summary>
            [JsonPropertyName("urls")]
            public UrlInfo Urls { get; set; }

            /// <summary>
            /// 写真情報
            /// </summary>
            [JsonPropertyName("photo")]
            public PhotoInfo Photo { get; set; }

            /// <summary>
            /// お店のジャンル情報クラス
            /// </summary>
            public class GenreInfo
            {
                /// <summary>
                /// お店のジャンルコード
                /// </summary>
                [JsonPropertyName("code")]
                public string Code { get; set; }

                /// <summary>
                /// お店のジャンル名
                /// </summary>
                [JsonPropertyName("name")]
                public string Name { get; set; }
            }

            /// <summary>
            /// 店舗のURL情報クラス
            /// </summary>
            public class UrlInfo
            {
                /// <summary>
                /// PC向けURL
                /// </summary>
                [JsonPropertyName("pc")]
                public string Pc { get; set; }
            }

            /// <summary>
            /// 写真情報クラス
            /// </summary>
            public class PhotoInfo
            {
                /// <summary>
                /// PC向けの写真情報
                /// </summary>
                [JsonPropertyName("pc")]
                public PhotoPc Pc { get; set; }
            }

            /// <summary>
            /// PC向け写真情報クラス
            /// </summary>
            public class PhotoPc
            {
                /// <summary>
                /// 店舗トップ写真（大）
                /// </summary>
                [JsonPropertyName("l")]
                public string Large { get; set; }

                /// <summary>
                /// 店舗トップ写真（中）
                /// </summary>
                [JsonPropertyName("m")]
                public string Medium { get; set; }

                /// <summary>
                /// 店舗トップ写真（小）
                /// </summary>
                [JsonPropertyName("s")]
                public string Small { get; set; }
            }
        }

        /// <summary>
        /// カルーセルテンプレートの配列に変換
        /// </summary>
        /// <returns></returns>
        public CarouselTemplate.Column[] ToCarouselTemplateColumns()
        {
            return Shops.Select(shop => new CarouselTemplate.Column
            {
                ThumbnailImageUrl = shop.Photo?.Pc?.Large,
                Title = shop.Name,
                Text = shop.Catch,
                Actions = new UriAction[]
                {
                    new ()
                    {
                        Label = "詳細を見る",
                        Uri = shop.Urls?.Pc
                    }
                }
            }).ToArray();
        }
    }
}