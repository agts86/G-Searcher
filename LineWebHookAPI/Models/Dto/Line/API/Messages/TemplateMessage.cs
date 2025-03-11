using System.Text.Json.Serialization;
using LineWebHookAPI.Models.Dto.Line.API.Messages.Templates;
using LineWebHookAPI.Jsons;

namespace LineWebHookAPI.Models.Dto.Line.API.Messages;

public class TemplateMessage : IMessage
{
    public string Type { get; } = "template";

    public string AltText { get; set; }

    [JsonConverter(typeof(RealMoldConverter<ITemplate>))]
    public ITemplate Template { get; set; }
}
