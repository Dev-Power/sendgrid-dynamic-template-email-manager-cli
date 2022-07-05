using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ConsoleTables;
using DynamicTemplateManager.Cli.Services.Interfaces;

namespace DynamicTemplateManager.Cli.Commands.TemplateCommands;

[Command("list-templates")]
public class ListTemplatesCommand : ICommand
{
    private readonly IDynamicTemplateService _dynamicTemplateService;

    public ListTemplatesCommand(IDynamicTemplateService dynamicTemplateService)
    {
        _dynamicTemplateService = dynamicTemplateService;
    }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        var templates = _dynamicTemplateService.ListTemplates().Result;

        var table = new ConsoleTable("Template Name", "Template Id");
        
        foreach (var templateIdNameTuple in templates)
        {
            table.AddRow(templateIdNameTuple.Item1, templateIdNameTuple.Item2);
        }
        
        console.Output.WriteLine(table.ToString());

        return default;
    }
}