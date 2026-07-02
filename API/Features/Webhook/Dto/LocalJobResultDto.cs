using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Features.Webhook.Dto;

public class LocalJobResultDto(LocalJobDto job, LocalEventResultDto[] results, string errorMessage)
{
    public LocalJobDto Job { get; } = job;

    public LocalEventResultDto[] Results { get; } = results;

    public bool IsSuccess { get; } = errorMessage is null;

    public string ErrorMessage { get; } = errorMessage;
}
