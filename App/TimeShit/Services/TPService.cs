using System.Net;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RestSharp;
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
            Console.WriteLine(response);
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return "";
    }
    
}