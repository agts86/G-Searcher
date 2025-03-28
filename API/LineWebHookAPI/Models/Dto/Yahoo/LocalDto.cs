using LineWebHookAPI.Models.Dto.Line.API.Messages.Actions;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;

namespace LineWebHookAPI.Models.Dto.Yahoo;

public class LocalDto
{
    public FeatureInfo[] Feature { get; set; }

    public class FeatureInfo
    {
        public string Name { get; set; }

        public PropertyInfo Property { get; set; }


        public class PropertyInfo
        {
            public string Address { get; set; }

            public DetailInfo Detail { get; set; }

            public class DetailInfo
            {
                public string Image1 { get; set; }

                public string CassetteOwnerLogoImage { get; set; }

                public string YUrl { get; set; }
            }
        }

        
    }

    /// <summary>
    /// カルーセルテンプレートの配列に変換
    /// </summary>
    /// <returns></returns>
    public CarouselTemplate.Column[] ToCarouselTemplateColumns()
    {
        return [.. Feature?.Select(feature => new CarouselTemplate.Column
        {
            Title = feature.Name,
            Text = feature.Property.Address,
            Actions = new UriAction[]
            {
                new ()
                {
                    Label = "詳細を見る",
                    Uri = feature.Property.Detail.YUrl
                }
            }
        })
        .Take(10) ?? []];
    }
}
