using FluentValidation;

namespace Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("الاسم مطلوب");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("البريد الإلكتروني مطلوب");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("كلمة المرور مطلوبة");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("رقم الهاتف مطلوب");
    }
}
