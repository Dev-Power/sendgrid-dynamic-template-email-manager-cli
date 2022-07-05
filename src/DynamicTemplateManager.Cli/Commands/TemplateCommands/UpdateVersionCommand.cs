using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ConsoleTables;
using DynamicTemplateManager.Cli.Services.Interfaces;
using Sharprompt;

namespace DynamicTemplateManager.Cli.Commands.TemplateCommands;

[Command("update-version")]
public class UpdateVersionCommand : ICommand
{
    [CommandOption("base", 'b', Description = "The root directory for the HTML templates", EnvironmentVariable = "HTML_BASE_DIR", IsRequired = false)]
    public string HtmlBaseDirectory { get; set; } = string.Empty;
    
    [CommandOption("path", 'p', Description = "The full path to the version HTML file", IsRequired = false)]
    public string HtmlFilePath { get; set; } = string.Empty;
    
    private readonly IDynamicTemplateService _dynamicTemplateService;

    public UpdateVersionCommand(IDynamicTemplateService dynamicTemplateService)
    {
        _dynamicTemplateService = dynamicTemplateService;
    }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        var templates = _dynamicTemplateService.ListTemplates().Result;
        if (templates.Count == 0)
        {
            throw new ArgumentException("No available templates. Please create a template first.");
        }
        
        var templateName = Prompt.Select("Please select a template", templates.Select(t => t.Item1).OrderBy(t => t).ToList());
        var templateId = templates.First(t => t.Item1 == templateName).Item2;
        
        var versions = _dynamicTemplateService.ListVersions(templateId).Result;
        if (templates.Count == 0)
        {
            throw new ArgumentException("No available version found. Please create a version first.");
        }
        var versionName = Prompt.Select("Please select a select", versions.Select(t => t.Item1).OrderBy(t => t).ToList());
        var versionId = versions.First(t => t.Item1 == versionName).Item2;
        
        string htmltemplateData = string.Empty;
        
        if (!string.IsNullOrEmpty(HtmlBaseDirectory))
        {
            var templateFileList = Directory.GetFiles(HtmlBaseDirectory, "*.html", SearchOption.AllDirectories);
            if (templateFileList.Length == 0)
            {
                throw new ArgumentException("No available HTML templates. Please create your HTML template under ./EmailTemplates folder with .html extension");
            }
            var templateFileNameList = templateFileList.Select(f => Path.GetFileNameWithoutExtension(f)).OrderBy(f => f);
            var templateFileName = Prompt.Select($"Please select a template (HTML files Loaded from {HtmlBaseDirectory} folder)", templateFileNameList);
            
            htmltemplateData = File.ReadAllTextAsync($"{HtmlBaseDirectory}/{templateFileName}.html").Result;
        }
        else
        {
            if (string.IsNullOrEmpty(HtmlFilePath))
            {
                HtmlFilePath = Prompt.Input<string>("Please enter the path to the HTML file");
            }
            
            htmltemplateData = File.ReadAllTextAsync(HtmlFilePath).Result;
        }
        
        _dynamicTemplateService.UpdateVersion(templateId, versionId, versionName, htmltemplateData).Wait();
        console.Output.WriteLine($"Version has been created successfully: {versionId}");
        
        return default;
    }
}