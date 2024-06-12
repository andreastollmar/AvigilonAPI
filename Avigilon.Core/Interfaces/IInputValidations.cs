namespace Avigilon.Core.Interfaces;

public interface IInputValidations
{
    bool ValidateDateInputFromUser(string date);
    public bool ValidateTimeInputFromUser(string time);
    public bool ValidateCameraChoiceFromUser(string camera);
}
