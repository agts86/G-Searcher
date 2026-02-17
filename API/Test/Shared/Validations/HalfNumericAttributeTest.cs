using Shared.Validations;


namespace Test.Shared.Validations;

public class HalfNumericAttributeTest
{
    [Theory]
    [InlineData("1", true)]
    [InlineData(null, true)]
    [InlineData("１", false)]
    [InlineData("a", false)]
    [InlineData("", false)]
    public void IsValidTestWhenInputReturnsExceptedResult(string input, bool expected)
    {
        // Arrange
        var attribute = new HalfNumericAttribute();

        // Act
        var result = attribute.IsValid(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
