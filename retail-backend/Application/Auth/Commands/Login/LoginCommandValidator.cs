using FluentValidation;

namespace Application.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("البريد الإلكتروني مطلوب");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("كلمة المرور مطلوبة");
    }
}
