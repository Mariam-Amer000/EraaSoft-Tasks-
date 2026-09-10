using System.ComponentModel.DataAnnotations;

namespace CinemaDashboard.Validations;

public class LetteroOnlyAttribute : ValidationAttribute
{
    private readonly int _minLenght;
    private readonly int _maxLenght;
    public LetteroOnlyAttribute(int minLenght = 0, int maxLenght = 8000)
    {
        _minLenght = minLenght;
        _maxLenght = maxLenght;
    }

    public override bool IsValid(object? value)
    {
        if (value is not string text)
            return false;

        return text.Length >= _minLenght && text.Length <= _maxLenght && text.All(c => char.IsLetter(c));
    }

    public override string FormatErrorMessage(string name)
    {
        return $"the field {name} must have chars only, and the lenght must be in {_minLenght},{_maxLenght}";
    }
}
