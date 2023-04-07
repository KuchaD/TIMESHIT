using IdentityManager.Shared.Shared;
using Mediator;
using TimeShitApp.Share;

namespace TimeShitApp.Application;

public sealed record TimeRecalculationRequest(IList<Time> Times, double AdditionHours, bool WholeHours) 
    : IRequest<Result<TimeRecalculationResponse>>;

public sealed record TimeRecalculationResponse(IList<Time> Times);

public sealed class TimeRecalculationHandler 
    : IRequestHandler<TimeRecalculationRequest, Result<TimeRecalculationResponse>>
{
    //algorithm for recalculation time is for powershell app
    public async ValueTask<Result<TimeRecalculationResponse>> Handle(TimeRecalculationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.AdditionHours is not 0)
        {
            var coefHours = request.AdditionHours * 4;
            var unitValue = 0.25;
            if (request.WholeHours)
            {
                coefHours = request.AdditionHours;
                unitValue = 1;
            }

            var absCoefHours = Math.Abs(coefHours);
            var isCoefPositive = coefHours > 0;
            var ix = 0;
            while (absCoefHours > 0)
            {
                if (ix > request.Times.Count)
                {
                    ix = 0;
                }

                if (isCoefPositive)
                {
                    request.Times[ix].Hours += unitValue;
                    absCoefHours--;
                }
                else
                {
                    var currentRecordSpent = request.Times[ix].Hours;
                    currentRecordSpent -= unitValue;
                    if (currentRecordSpent > 0)
                    {
                        request.Times[ix].Hours = currentRecordSpent;
                        absCoefHours--;
                    }
                }

                ix++;
            }
        }

        return new TimeRecalculationResponse(request.Times);
    }
}