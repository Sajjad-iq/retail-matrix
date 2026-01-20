using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException()
        : base("حدثت أخطاء في التحقق من الصحة")
    {
        Errors = new List<string>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .Select(f => f.ErrorMessage)
            .ToList();
    }

    public ValidationException(string error)
        : this()
    {
        Errors = new List<string> { error };
    }
}
