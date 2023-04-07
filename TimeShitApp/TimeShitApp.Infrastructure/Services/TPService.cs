using System.Globalization;
using System.Text;
using System.Xml;
using TimeShitApp.Application;
using TimeShitApp.Application.ServicesInterfaces;
using TimeShitApp.Share;

namespace TimeShit.Services;

public class TPService : ITPService
{
    private readonly IRefitTargetProcess httpClient;
    
    public TPService(IRefitTargetProcess httpClient)
    {
        this.httpClient = httpClient;
    }
    
    public async Task<User?> GetUserData(string name, string password)
    {
        try
        {
            string authentication = Utils.CreateBasicAuth(name, password);
            var response = await httpClient.GetLoginUser(authentication);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);
           
            return new User(
                xml.GetElementsByTagName("FirstName")[0].InnerText,
                xml.GetElementsByTagName("LastName")[0].InnerText,
                xml.GetElementsByTagName("Email")[0].InnerText,
                xml.GetElementsByTagName("User")[0].Attributes.GetNamedItem("Id").Value
            );
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }

    public async Task<IList<Time>> GetTimes(DateTime? dateStart, DateTime? dateEnd, string userId, string authentication)
    {
        try
        {
            var dateStartString = dateStart?.ToString("dd-MMM-yyyy",CultureInfo.GetCultureInfo("en-US"));
            var dateEndString = dateEnd?.ToString("dd-MMM-yyyy", CultureInfo.GetCultureInfo("en-US"));
            var where = $"(User.Id eq "+userId+")and(Date gte \'"+dateStartString+"\')and(Date lte \'"+dateEndString+"\')";
            var response = await httpClient.GetTime(authentication, new TimeQueryParams(){ Where = where, Include = "[Spent,Description,Date,User[Id,Login],Assignable[Name],Project[Name]]"});
            
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response.Content);
            
            var timeEntries = xml.GetElementsByTagName("Time");
            var times = new List<Time>();
            foreach (XmlElement timeNode in timeEntries)
            {
                var time = new Time
                {
                    Hours = double.Parse(timeNode.GetElementsByTagName("Spent")[0].InnerText, CultureInfo.InvariantCulture),
                    Date = DateTime.Parse(timeNode.GetElementsByTagName("Date")[0].InnerText),
                    Project = timeNode.GetElementsByTagName("Project")[0].Attributes.GetNamedItem("Name").Value,
                    Task = timeNode.GetElementsByTagName("Assignable")[0].Attributes.GetNamedItem("Name").Value
                };
                times.Add(time);
            }
            
            return times;
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        return Array.Empty<Time>();
    }
}