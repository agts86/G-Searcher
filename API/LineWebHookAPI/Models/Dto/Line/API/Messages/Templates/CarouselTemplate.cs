using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LineWebHookAPI.Constants.Line.API.Templates.Carousels;
using LineWebHookAPI.Jsons;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Actions;

namespace LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;

public class CarouselTemplate : ITemplate
{
    [JsonPropertyName("type")]
    public string Type { get; } = "carousel";

    [Required]
    [MaxLength(10)]
    [JsonPropertyName("columns")]
    public Column[] Columns { get; set; }

    [JsonPropertyName("imageAspectRatio")]
    public ImageAspectRatio ImageAspectRatio { get; set; } = ImageAspectRatio.Rectangle;

    [JsonPropertyName("imageSize")]
    public ImageSize ImageSize { get; set; } = ImageSize.Cover;

    public class Column
    {
        [JsonPropertyName("thumbnailImageUrl")]
        public string ThumbnailImageUrl { get; set; }

        [JsonPropertyName("imageBackgroundColor")]
        public string ImageBackgroundColor { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("text")]
        [Required]
        public string Text { get; set; }
        
        [JsonConverter(typeof(RealMoldConverter<IAction>))]
        [JsonPropertyName("defaultAction")]
        public IAction DefaultAction { get; set; }

        [MaxLength(3)]
        [JsonConverter(typeof(RealMoldConverter<IAction[]>))]
        [JsonPropertyName("actions")]
        public IAction[] Actions { get; set; }
    }
}
