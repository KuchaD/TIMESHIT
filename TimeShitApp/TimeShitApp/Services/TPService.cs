using System.Globalization;
using System.Text;
using TimeShit.Services.Interfaces;

namespace TimeShit.Services;

public class TPService : ITPService
{
    private readonly IRefitTargetProcess httpClient;
    
    
    public TPService(IRefitTargetProcess httpClient)
    {
        this.httpClient = httpClient;
    }
    
    public async Task<string> GetData(string name, string password)
    {
        try
        {
            string authentication = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{name}:{password}"));
            var response = await httpClient.GetLoginUser(authentication);
            return response;
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return "";
    }

    public async Task<string> GetTimes(string name, string password)
    {
        try
        {
        var data = await GetData(name, password);
        string authentication = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{name}:{password}"));
        
        var dateStart = DateTime.Now.AddDays(-30).ToString("dd-MMM-yyyy",CultureInfo.GetCultureInfo("en-US"));
        var dateEnd = DateTime.Now.ToString("dd-MMM-yyyy", CultureInfo.GetCultureInfo("en-US"));
        var UserId = "6911";
        var where = $"(User.Id eq "+UserId+")and(Date gte \'"+dateStart+"\')and(Date lte \'"+dateEnd+"\')";
        var response = await httpClient.GetTime(authentication, new MyQueryParams(){ where = where, include = "[Spent,Description,Date,User[Id,Login],Assignable[Name],Project[Name]]&take=500&orderBy=Date"});
        return response;
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return "";
    }
}