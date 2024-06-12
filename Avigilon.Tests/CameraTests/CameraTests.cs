namespace Avigilon.Tests.CameraTests;

public class CameraTests
{
    [Theory]
    [InlineData("vd1")]
    [InlineData("vd2")]
    [InlineData("VD1")]
    [InlineData("VD2")]
    [InlineData("lrf")]
    [InlineData("LRF")]
    [InlineData("LrF")]
    [InlineData("Vd1")]
    public void ValidCameraNamesWillReturnTrue(string camera)
    {
        var validation = new InputValidations();

        var output = validation.ValidateCameraChoiceFromUser(camera);
        Assert.True(output);
    }
    [Theory]
    [InlineData("VDD1")]
    [InlineData("LRF1")]
    [InlineData("Vakum1")]
    [InlineData("vdd2")]
    [InlineData("lrf2")]
    public void InvalidCameraNamesWillThrowException(string camera)
    {
        var validation = new InputValidations();

        Assert.Throws<Exception>(() => validation.ValidateCameraChoiceFromUser(camera));
    }

}
