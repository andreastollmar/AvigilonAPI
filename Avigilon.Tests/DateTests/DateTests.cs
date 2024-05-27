namespace Avigilon.Tests.DateTests;

public class DateTests
{
    [Theory]
    [InlineData("2024-01-03")]
    [InlineData("2022-05-26")]
    [InlineData("1999-10-11")]
    [InlineData("2015-12-31")]
    [InlineData("2020-06-17")]
    public void ValidInputWillReturnTrue(string date)
    {
        var validation = new InputValidations();
        
        var output = validation.ValidateDateInputFromUser(date);
        Assert.True(output);

    }
    [Theory]
    [InlineData("2024-00-03")]
    [InlineData("2022-05-32")]
    [InlineData("1999-10-1")]
    [InlineData("2015-1-31")]
    [InlineData("20-06-17")]
    public void InvalidInputWillThrowException(string date)
    {
        var validation = new InputValidations();

        //var output = validation.ValidateDateInputFromUser(input);
        Assert.Throws<Exception>(() => validation.ValidateDateInputFromUser(date));

    }
}
