using Application.Auth.DTOs;
using Application.Common.Exceptions;
using AutoMapper;
using Domains.Users.Repositories;
using MediatR;

namespace Application.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException("User", request.UserId);

        return _mapper.Map<UserDto>(user);
    }
}
