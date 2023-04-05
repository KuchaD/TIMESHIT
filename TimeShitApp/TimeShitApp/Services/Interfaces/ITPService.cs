namespace TimeShit.Services.Interfaces;

public interface ITPService
{
    public Task<string> GetData(string name, string password);
}