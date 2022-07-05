using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ConsoleTables;
using DynamicTemplateManager.Cli.Services.Interfaces;
using Sharprompt;

namespace DynamicTemplateManager.Cli.Commands.TemplateCommands;

[Command("delete-version")]
public class DeleteVersionCommand : ICommand
{
    private readonly IDynamicTemplateService _dynamicTemplateService;

    public DeleteVersionCommand(IDynamicTemplateService dynamicTemplateService)
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
        
        var templateName = Prompt.Select("Please select a template to delete", templates.Select(t => t.Item1).OrderBy(t => t).ToList());
        var templateId = templates.First(t => t.Item1 == templateName).Item2;
        
        var versions = _dynamicTemplateService.ListVersions(templateId).Result;
        if (templates.Count == 0)
        {
            throw new ArgumentException("No available version found. Please create a version first.");
        }
        var versionName = Prompt.Select("Please select a select", versions.Select(t => t.Item1).OrderBy(t => t).ToList());
        var versionId = versions.First(t => t.Item1 == versionName).Item2;
        
        var shouldProceed = Prompt.Confirm($"Please confirm you want to delete version [{versionName}] from template [{templateName}]", defaultValue: false);
        if (shouldProceed)
        {
            _dynamicTemplateService.DeleteVersion(templateId, versionId).Wait();
        
            console.Output.WriteLine($"Version {versionName} has been deleted successfully from template {templateName}");            
        }
        
        return default;
    }
}