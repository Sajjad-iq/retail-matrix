using Application.Auth.DTOs;
using Application.Common.Exceptions;
using Application.Common.Services;
using AutoMapper;
using Domains.Organizations.Repositories;
using Domains.Users.Enums;
using Domains.Users.Repositories;
using Domains.Users.Services;
using MediatR;

namespace Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<TokenDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Retrieve user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
            throw new UnauthorizedException("البريد الإلكتروني أو كلمة المرور غير صحيحة");

        // 2. Check if account is locked
        if (user.IsAccountLocked())
        {
            throw new UnauthorizedException("الحساب مقفل مؤقتاً بسبب محاولات تسجيل دخول فاشلة متعددة");
        }

        // 3. Verify password
        if (!user.VerifyPassword(request.Password, _passwordHasher))
        {
            // Record failed login attempt
            user.RecordFailedLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            throw new UnauthorizedException("البريد الإلكتروني أو كلمة المرور غير صحيحة");
        }

        // 4. Check if account is active
        if (!user.IsActive)
        {
            throw new UnauthorizedException("الحساب غير نشط");
        }

        // 5. Reset failed login attempts on successful login
        user.ResetFailedLoginAttempts();
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // 6. Fetch owned organizations for business owners
        IEnumerable<Guid>? ownedOrganizationIds = null;
        if (user.AccountType == AccountType.BusinessOwner)
        {
            var ownedOrganizations = await _organizationRepository.GetByCreatorAsync(user.Id, cancellationToken);
            ownedOrganizationIds = ownedOrganizations.Select(o => o.Id);
        }

        // 7. Generate JWT tokens with organization info
        var accessToken = _jwtTokenService.GenerateAccessToken(user, ownedOrganizationIds);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = _jwtTokenService.GetTokenExpiration();

        // 8. Map user to DTO
        var userDto = _mapper.Map<UserDto>(user);

        // 9. Return token response
        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = userDto
        };
    }
}
