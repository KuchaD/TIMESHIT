using Refit;

namespace TimeShit.Services;

public interface IRefitTargetProcess
{
    [Get("/GetUser")]
    Task<string> GetLoginUser([Authorize("Basic")] string token);
}