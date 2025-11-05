using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace LineWebHookAPI.Validations;

public partial class HalfNumericAttribute : ValidationAttribute
{
    private Regex Regex { get; } = HalfNumeric();

    public override bool IsValid(object value)
    {
        if(value is string str)
            return Regex.IsMatch(str);
        return true;
    }

    [GeneratedRegex(@"^\d+$")]
    [ExcludeFromCodeCoverage]
    private static partial Regex HalfNumeric();
}
