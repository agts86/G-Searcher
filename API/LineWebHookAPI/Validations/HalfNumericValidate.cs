using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LineWebHookAPI.Validations;

public class HalfNumericAttribute : ValidationAttribute
{
    private Regex Regex { get; } = new(@"^\d+$");

    public override bool IsValid(object value)
    {
        if(value is string str)
            return Regex.IsMatch(str);
        return true;
    }
}
