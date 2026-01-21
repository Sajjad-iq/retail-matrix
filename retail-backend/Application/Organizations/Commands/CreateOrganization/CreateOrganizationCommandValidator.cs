using FluentValidation;

namespace Application.Organizations.Commands.CreateOrganization;

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم المؤسسة مطلوب");

        RuleFor(x => x.Domain)
            .NotEmpty().WithMessage("نطاق المؤسسة مطلوب");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب");
    }
}
