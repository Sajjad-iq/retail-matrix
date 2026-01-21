using FluentValidation;

namespace Application.Organizations.Commands.ChangeOrganizationStatus;

public class ChangeOrganizationStatusCommandValidator : AbstractValidator<ChangeOrganizationStatusCommand>
{
    public ChangeOrganizationStatusCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("معرف المؤسسة مطلوب");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("حالة المؤسسة غير صحيحة");
    }
}
