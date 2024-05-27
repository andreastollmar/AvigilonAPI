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
}
