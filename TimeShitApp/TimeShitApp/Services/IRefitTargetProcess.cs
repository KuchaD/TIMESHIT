using Refit;

namespace TimeShit.Services;

[Headers("X-Accept: application/json", "dataType:json")]
public interface IRefitTargetProcess
{
    [Headers("Content-Type: application/json; charset=utf-8")]
    [Get("/api/v1/users/LoggedUser")]
    Task<string> GetLoginUser([Authorize("Basic")] string token);
}