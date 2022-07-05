using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ConsoleTables;
using DynamicTemplateManager.Cli.Services.Interfaces;
using Sharprompt;

namespace DynamicTemplateManager.Cli.Commands.TemplateCommands;

[Command("update-template")]
public class UpdateTemplateCommand : ICommand
{
    private readonly IDynamicTemplateService _dynamicTemplateService;

    public UpdateTemplateCommand(IDynamicTemplateService dynamicTemplateService)
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
        
        var newTemplateName = Prompt.Input<string>("Please enter the new template name");
        
        _dynamicTemplateService.UpdateTemplate(newTemplateName, templateId).Wait();
        
        console.Output.WriteLine($"Template has been updated successfully");
        
        return default;
    }
}