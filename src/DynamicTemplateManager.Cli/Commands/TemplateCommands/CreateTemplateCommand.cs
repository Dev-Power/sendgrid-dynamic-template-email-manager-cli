using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ConsoleTables;
using DynamicTemplateManager.Cli.Services.Interfaces;
using Sharprompt;

namespace DynamicTemplateManager.Cli.Commands.TemplateCommands;

[Command("create-template")]
public class CreateTemplateCommand : ICommand
{
    private readonly IDynamicTemplateService _dynamicTemplateService;

    [CommandParameter(0, Description = "The name of the new template", IsRequired = false)]
    public string TemplateName { get; set; } = string.Empty;
    
    public CreateTemplateCommand(IDynamicTemplateService dynamicTemplateService)
    {
        _dynamicTemplateService = dynamicTemplateService;
    }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        if (string.IsNullOrEmpty(TemplateName))
        {
            TemplateName = Prompt.Input<string>("Please enter the template name");    
        }
        
        var templateId = _dynamicTemplateService.CreateTemplate(TemplateName).Result;
        
        console.Output.WriteLine($"Template has been created successfully: {templateId}");
        
        return default;
    }
}