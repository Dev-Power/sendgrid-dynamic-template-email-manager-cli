using DynamicTemplateManager.Cli.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SendGrid;

namespace DynamicTemplateManager.Cli.Services.Impl;

public class DynamicTemplateService : IDynamicTemplateService
{
    private readonly ISendGridClient _sendGridClient;
    
    public DynamicTemplateService(ISendGridClient sendGridClient)
    {
        _sendGridClient = sendGridClient;
    }
    
    public async Task<List<(string, string)>> ListTemplates()
    {
        var queryParams = @"{
            'generations': 'dynamic',
            'page_size': 100
        }";
        
        var response = await _sendGridClient.RequestAsync(
            method: SendGridClient.Method.GET,
            urlPath: "templates",
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
    
    public async Task<string> CreateTemplate(string templateName)
    {
        var data = new
        {
            name = templateName, 
            generation = "dynamic"
        };
        
        var response = await _sendGridClient.RequestAsync(
            method: SendGridClient.Method.POST,
            urlPath: "templates",
            requestBody: JsonConvert.SerializeObject(data)
        );
        
        if (!response.IsSuccessStatusCode)
        {
            HandleFailedResponse(response);
        }
        
        var result = response.Body.ReadAsStringAsync().Result;
        return JObject.Parse(result)["id"].ToString();
    }

    public async Task UpdateTemplate(string templateName, string templateId)
    {
        var data = new
        {
            name = templateName,
        };
        
        var response = await _sendGridClient.RequestAsync(
            method: SendGridClient.Method.PATCH,
            urlPath: $"templates/{templateId}",
            requestBody: JsonConvert.SerializeObject(data)
        );
        
        if (!response.IsSuccessStatusCode)
        {
            HandleFailedResponse(response);
        }
    }
    
    public async Task DeleteTemplate(string templateId)
    {
        var response = await _sendGridClient.RequestAsync(
            method: SendGridClient.Method.DELETE,
            urlPath: $"templates/{templateId}"
        );
       
        if (!response.IsSuccessStatusCode)
        {
            HandleFailedResponse(response);
        }
    }
    
    public async Task<string> DuplicateTemplate(string templateName, string templateId)
    {
        var data = new
        {
            name = templateName
        };
        
        var response = await _sendGridClient.RequestAsync(
            method: SendGridClient.Method.POST,
            urlPath: $"templates/{templateId}",
            requestBody: JsonConvert.SerializeObject(data)
        );
        
        if (!response.IsSuccessStatusCode)
        {
            HandleFailedResponse(response);
        }
        
        var result = response.Body.ReadAsStringAsync().Result;
        return JObject.Parse(result)["id"].ToString();
    }
    
    public async Task<List<(string, string)>> ListVersions(string templateId)
    {
        var response = await _sendGridClient.RequestAsync(
            method: SendGridClient.Method.GET,
            urlPath: $"templates/{templateId}"
        );
        
        if (!response.IsSuccessStatusCode)
        {
            HandleFailedResponse(response);
        }
        
        var result = response.Body.ReadAsStringAsync().Result;
        var resultJson = JObject.Parse(result);
        
        var versionIdNameTuples = new List<(string, string)>();
        var versions = JArray.Parse(resultJson["versions"].ToString());
        foreach (var version in versions)
        {
            versionIdNameTuples.Add((version["name"].ToString(), version["id"].ToString()));
        }
        
        return versionIdNameTuples;
    }
    
    public async Task<string> CreateVersion(string templateId, string versionName, string htmltemplateData)
    {
        var data = new
        {
            template_id = templateId,
            active = 1,
            name = versionName,
            html_content = htmltemplateData,
            generate_plain_content = false,
            subject = "{{subject}}",
            editor = "code"
        };
        
        var response = await _sendGridClient.RequestAsync(
            method: SendGridClient.Method.POST,
            urlPath: $"templates/{templateId}/versions",
            requestBody: JsonConvert.SerializeObject(data)
        );
        
        if (!response.IsSuccessStatusCode)
        {
            HandleFailedResponse(response);
        }
        
        string result = response.Body.ReadAsStringAsync().Result;
        string versionId = JObject.Parse(result)["id"].ToString();
        return versionId;
    }

    public async Task UpdateVersion(string templateId, string versionId, string versionName, string htmltemplateData)
    {
        var data = new
        {
            template_id = templateId,
            active = 1,
            name = versionName,
            html_content = htmltemplateData,
            generate_plain_content = false,
            subject = "{{subject}}"
        };
        
        var response = await _sendGridClient.RequestAsync(
            method: SendGridClient.Method.PATCH,
            urlPath: $"templates/{templateId}/versions/{versionId}",
            requestBody: JsonConvert.SerializeObject(data)
        );
        
        if (!response.IsSuccessStatusCode)
        {
            HandleFailedResponse(response);
        }
    }
    
    public async Task DeleteVersion(string templateId, string versionId)
    {
        var response = await _sendGridClient.RequestAsync(
            method: SendGridClient.Method.DELETE,
            urlPath: $"templates/{templateId}/versions/{versionId}"
        );
        
        if (!response.IsSuccessStatusCode)
        {
            HandleFailedResponse(response);
        }
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