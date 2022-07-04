using DynamicTemplateManager.Cli.Services.Interfaces;
using Newtonsoft.Json.Linq;
using SendGrid;

namespace DynamicTemplateManager.Cli.Services.Impl;

public class DynamicTemplateService : IDynamicTemplateService
{
    public async Task<List<(string, string)>> ListTemplates(string apiKey)
    {
        var sendGridClient = new SendGridClient(apiKey);
        var queryParams = @"{
            'generations': 'dynamic',
            'page_size': 100
        }";
        
        var response = await sendGridClient.RequestAsync(
            method: SendGridClient.Method.GET,
            urlPath: $"templates",
            queryParams: queryParams
        );
        
        if (!response.IsSuccessStatusCode)
        {
            HandleFailedResponse(response);
        }
        
        var result = response.Body.ReadAsStringAsync().Result;
        var resultJson = JObject.Parse(result);
        
        var templateIdNameTuples = new List<(string, string)>();
        var templates = JArray.Parse(resultJson["result"].ToString());
        foreach (var template in templates)
        {
            templateIdNameTuples.Add((template["name"].ToString(), template["id"].ToString()));
        }

        return templateIdNameTuples;
    }
    
    private void HandleFailedResponse(Response response)
    {
        var result = response.Body.ReadAsStringAsync().Result;
        
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(result);
        Console.WriteLine(response.Headers.ToString());

        throw new Exception($"API call failed with code {response.StatusCode}");
    } 
}