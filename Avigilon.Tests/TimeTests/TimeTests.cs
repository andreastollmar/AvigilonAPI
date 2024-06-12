using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Avigilon.Tests.TimeTests;

public class TimeTests
{
    [Theory]
    [InlineData("23:59:59")]
    [InlineData("00:01:00")]
    [InlineData("03:45:55")]
    [InlineData("00:59:00")]
    public void CorrectTimesWillBeValid(string time)
    {
        var validation = new InputValidations();

        var output = validation.ValidateTimeInputFromUser(time);
        Assert.True(output);
    }
    [Theory]
    [InlineData("24:00:00")]
    [InlineData("00:00:60")]
    [InlineData("23:60:00")]
    [InlineData("25:55:55")]
    public void InvalidTimesWillThrowException(string time)
    {
        var validation = new InputValidations();

        Assert.Throws<Exception>(() => validation.ValidateTimeInputFromUser(time));
    }

}
