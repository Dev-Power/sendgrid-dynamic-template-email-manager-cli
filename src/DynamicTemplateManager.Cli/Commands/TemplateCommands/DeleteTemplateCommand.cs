using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ConsoleTables;
using DynamicTemplateManager.Cli.Services.Interfaces;
using Sharprompt;

namespace DynamicTemplateManager.Cli.Commands.TemplateCommands;

[Command("delete-template")]
public class DeleteTemplateCommand : ICommand
{
    private readonly IDynamicTemplateService _dynamicTemplateService;

    public DeleteTemplateCommand(IDynamicTemplateService dynamicTemplateService)
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
        
        var shouldProceed = Prompt.Confirm($"Please confirm you want to delete template [{templateName}]", defaultValue: false);
        if (shouldProceed)
        {
            _dynamicTemplateService.DeleteTemplate(templateId).Wait();
        
            console.Output.WriteLine($"Template {templateName} has been deleted successfully.");            
        }
        return default;
    }
}