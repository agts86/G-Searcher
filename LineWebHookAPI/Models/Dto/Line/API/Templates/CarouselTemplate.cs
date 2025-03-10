using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LineWebHookAPI.Constants.Line.API.Templates.Carousels;
using LineWebHookAPI.Jsons;
using LineWebHookAPI.Models.Dto.Line.API.Actions;

namespace LineWebHookAPI.Models.Dto.Line.API.Templates;

public class CarouselTemplate : ITemplate
{
    public string Type { get; } = "carousel";

    [Required]
    [MaxLength(10)]
    public Column[] Columns { get; set; }

    public ImageAspectRatio ImageAspectRatio { get; set; } = ImageAspectRatio.Rectangle;

    public ImageSize ImageSize { get; set; } = ImageSize.Cover;

    public class Column
    {
        public string ThumbnailImageUrl { get; set; }

        public string ImageBackgroundColor { get; set; }

        public string Title { get; set; }

        [Required]
        public string Text { get; set; }
        [JsonConverter(typeof(RealMoldConverter<IAction>))]
        public IAction DefaultAction { get; set; }

        [MaxLength(3)]
        [JsonConverter(typeof(RealMoldConverter<IAction[]>))]
        public IAction[] Actions { get; set; }
    }
}
