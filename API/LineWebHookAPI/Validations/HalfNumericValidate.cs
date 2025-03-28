using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LineWebHookAPI.Validations;

public class HalfNumericAttribute : ValidationAttribute
{
    public Regex regex = new(@"^\d+$");

    public override bool IsValid(object value)
    {
        if(value is string str)
            return regex.IsMatch(str);
        return true;
    }
}
