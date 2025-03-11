using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineWebHookAPI.Models.Dto.Line.API.Messages.Actions
{
    public interface IAction
    {
        string Type { get;}
    }
}
