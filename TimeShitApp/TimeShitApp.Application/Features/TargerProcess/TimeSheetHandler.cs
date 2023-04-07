using Abstraction.Shared;
using IdentityManager.Shared.Shared;
using Mediator;
using Microsoft.Extensions.Options;
using TimeShitApp.Application.ServicesInterfaces;
using TimeShitApp.Options;
using TimeShitApp.Share;

namespace TimeShitApp.Application;


public sealed record TimeDataRequest(DateTime? StartTime, DateTime? EndTime, string userId, string authorization) : IRequest<Result<TimeDataResponse>>;

public sealed record TimeDataResponse(IList<Time> Times);

public sealed class TimeSheetHandler : IRequestHandler<TimeDataRequest, Result<TimeDataResponse>>
{
    private readonly ITPService _tpService;
    private readonly IOptions<GeneralSetting> _generalSetting;

    public TimeSheetHandler(ITPService tpService, IOptions<GeneralSetting> generalSetting)
    {
        _tpService = tpService;
        _generalSetting = generalSetting;
    }

    public async ValueTask<Result<TimeDataResponse>> Handle(TimeDataRequest request, CancellationToken cancellationToken)
    {
        var auth = AesOperation.DecryptString(_generalSetting.Value.EncryptionKey, request.authorization);
        var times = await _tpService.GetTimes(request.StartTime, request.EndTime, request.userId, auth);

        return new TimeDataResponse(times);
    }
}