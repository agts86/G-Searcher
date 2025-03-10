using System.Text.Json.Serialization;
using LineWebHookAPI.Models.Dto.Line.API.Templates;
using LineWebHookAPI.Jsons;

namespace LineWebHookAPI.Models.Dto.Line.API;

public class TemplateMessage
{
    public string Type { get; } = "template";

    public string AltText { get; set; }

    [JsonConverter(typeof(RealMoldConverter<ITemplate>))]
    public ITemplate Template { get; set; }
}
