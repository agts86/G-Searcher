using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LineDevSdk.DTOs.WebHooks;

namespace LineWebHookAPI.Models.Dto.Yahoo;

public class YahooLocalJob(string id, WebHook webHook, string genreCode)
{
    public string Id { get; } = id;

    public WebHook WebHook { get; set; } = webHook;
    public string GenreCode { get; } = genreCode;
}
