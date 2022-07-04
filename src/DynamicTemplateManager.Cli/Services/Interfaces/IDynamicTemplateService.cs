namespace DynamicTemplateManager.Cli.Services.Interfaces;

public interface IDynamicTemplateService
{
    Task<List<(string, string)>> ListTemplates(string apiKey);
}