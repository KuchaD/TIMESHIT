using System.Text;

namespace TimeShitApp.Application;

public static class Utils
{
    public static string CreateBasicAuth(string email, string password) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"));
}