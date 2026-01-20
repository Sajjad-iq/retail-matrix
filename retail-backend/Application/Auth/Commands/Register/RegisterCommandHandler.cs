using Application.Common.Exceptions;
using Domains.Users.Entities;
using Domains.Users.Repositories;
using Domains.Users.Services;
using MediatR;

namespace Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ValidationException("البريد الإلكتروني مستخدم بالفعل");
        }

        // 2. Check if phone number already exists
        if (await _userRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken))
        {
            throw new ValidationException("رقم الهاتف مستخدم بالفعل");
        }

        // 3. Create BusinessOwner user
        var user = User.CreateBusinessOwner(
            name: request.Name,
            email: request.Email,
            password: request.Password,
            phoneNumber: request.PhoneNumber,
            passwordHasher: _passwordHasher,
            address: request.Address
        );

        // 4. Persist user
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // 5. Return user ID
        return user.Id;
    }
}
