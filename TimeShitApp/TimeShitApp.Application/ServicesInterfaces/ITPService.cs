using TimeShitApp.Share;

namespace TimeShitApp.Application.ServicesInterfaces;

public interface ITPService
{
    public Task<User?> GetUserData(string name, string password);

    public Task<IList<Time>> GetTimes(DateTime? dateStart, DateTime? dateEnd, string userId, string authentication);
}