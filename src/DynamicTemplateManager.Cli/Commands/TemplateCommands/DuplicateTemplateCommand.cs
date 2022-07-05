using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ConsoleTables;
using DynamicTemplateManager.Cli.Services.Interfaces;
using Sharprompt;

namespace DynamicTemplateManager.Cli.Commands.TemplateCommands;

[Command("duplicate-template")]
public class DuplicateTemplateCommand : ICommand
{
    private readonly IDynamicTemplateService _dynamicTemplateService;

    public DuplicateTemplateCommand(IDynamicTemplateService dynamicTemplateService)
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
        
        var newTemplateId = _dynamicTemplateService.DuplicateTemplate(newTemplateName, templateId).Result;
        
        console.Output.WriteLine($"Template {templateName} has been duplicated successfully with new template id {newTemplateId}");
        
        return default;
    }
}