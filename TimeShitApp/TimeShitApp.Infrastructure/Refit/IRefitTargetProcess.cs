using Refit;

namespace TimeShit.Services;

[Headers("X-Accept: application/json", "dataType:json")]
public interface IRefitTargetProcess
{
    [Headers("Content-Type: application/json; charset=utf-8")]
    [Get("/api/v1/users/LoggedUser")]
    Task<string> GetLoginUser([Authorize("Basic")] string token);
    
    [Headers("Content-Type: application/json; charset=utf-8")]
    [Get("/api/v1/Times")]
    Task<IApiResponse<string>> GetTime([Authorize("Basic")] string token, TimeQueryParams queryParams);
}
public class TimeQueryParams
{
    public string Where { get; set; }

    public string Include { get; set; }

    public int Take { get; set; } = 500;

    public string OrderBy { get; set; } = "Date";
}