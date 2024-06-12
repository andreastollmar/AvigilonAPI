namespace Avigilon.Infrastructure.Validation;
public class InputValidations : IInputValidations
{
    public bool ValidateDateInputFromUser(string date)
    {
        var pattern = "^\\d{4}-(0[1-9]|1[012])-(0[1-9]|[1][0-9]|2[0-9]|3[01])$";
        var regex = new Regex(pattern);
        var match = regex.Match(date);
        if (match.Success) 
        {
            return true;
        }
        throw new Exception("Date does not match expected pattern. Use format yyyy-mm-dd.");

    }

    public bool ValidateTimeInputFromUser(string time)
    {
        var pattern = "^(?:[01]\\d|2[0-3]):[0-5]\\d:[0-5]\\d$";
        var regex = new Regex(pattern);
        var match = regex?.Match(time);
        if (match.Success)
        {
            return true;
        }
        throw new Exception($"{time} is not valid. Needs to be hh:mm:ss");
    }

    public bool ValidateCameraChoiceFromUser(string camera)
    {
        if (camera.ToLower().Equals("vd1") || camera.ToLower().Equals("vd2") || camera.ToLower().Equals("lrf"))
        {
            return true;
        }
        throw new Exception($"{camera} is not valid. Needs to be vd1, vd2 or lrf");

    }
}
