using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Shared.Validations;

[ExcludeFromCodeCoverage]
public partial class HalfNumericAttribute : ValidationAttribute
{
    private Regex Regex { get; } = HalfNumeric();

    public override bool IsValid(object value)
    {
        if(value is string str)
            return Regex.IsMatch(str);
        return true;
    }

    [GeneratedRegex(@"^[0-9]+$")]
    private static partial Regex HalfNumeric();
}
