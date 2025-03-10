using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LineWebHookAPI.Models.Dto.Line.API.Actions;

public class UriAction : IAction
{
    public string Type { get; } = "uri";

    public string Label { get; set; }

    [Required]
    public string Uri { get; set; }
}
