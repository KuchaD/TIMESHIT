using Abstraction.Shared;
using IdentityManager.Shared.Shared;
using Mediator;
using TimeShitApp.Application.ServicesInterfaces;
using TimeShitApp.Share;

namespace TimeShitApp.Application;

public sealed record UserDataRequest(string email, string password) : IRequest<Result<UserDataResponse>>;

public sealed record UserDataResponse(User User);


public sealed class UserHandler : IRequestHandler<UserDataRequest, Result<UserDataResponse>>
{
    private readonly ITPService _tpService;

    public UserHandler(ITPService tpService)
    {
        _tpService = tpService;
    }

    public async ValueTask<Result<UserDataResponse>> Handle(UserDataRequest request, CancellationToken cancellationToken)
    {
        var user = await _tpService.GetUserData(request.email, request.password);
        if (user is null)
        {
            return Result.Failure<UserDataResponse>(Error.NullValue);
        }

        return new UserDataResponse(user);
    }
}