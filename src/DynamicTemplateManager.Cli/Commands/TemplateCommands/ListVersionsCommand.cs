using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using ConsoleTables;
using DynamicTemplateManager.Cli.Services.Interfaces;
using Sharprompt;

namespace DynamicTemplateManager.Cli.Commands.TemplateCommands;

[Command("list-versions")]
public class ListVersionsCommand : ICommand
{
    private readonly IDynamicTemplateService _dynamicTemplateService;

    public ListVersionsCommand(IDynamicTemplateService dynamicTemplateService)
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

        var table = new ConsoleTable("Template Name", "Version Name", "Version Id");
        
        foreach (var versionIdNameTuple in templates)
        {
            table.AddRow(templateName, versionIdNameTuple.Item1, versionIdNameTuple.Item2);
        }
        
        console.Output.WriteLine(table.ToString());
        return default;
    }
}