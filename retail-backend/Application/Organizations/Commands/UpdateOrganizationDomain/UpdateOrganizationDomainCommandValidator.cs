using FluentValidation;

namespace Application.Organizations.Commands.UpdateOrganizationDomain;

public class UpdateOrganizationDomainCommandValidator : AbstractValidator<UpdateOrganizationDomainCommand>
{
    public UpdateOrganizationDomainCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("معرف المؤسسة مطلوب");

        RuleFor(x => x.Domain)
            .NotEmpty().WithMessage("نطاق المؤسسة مطلوب");
    }
}
