using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace G_Searcher.Models.Exceptions;

public class ResponseError(string message)
{
    public string Message { get;} = message;
}
