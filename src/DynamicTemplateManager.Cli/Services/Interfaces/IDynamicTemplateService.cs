namespace DynamicTemplateManager.Cli.Services.Interfaces;

public interface IDynamicTemplateService
{
    Task<List<(string, string)>> ListTemplates();
    Task<string> CreateTemplate(string templateName);
    Task UpdateTemplate(string templateName, string templateId);
    Task DeleteTemplate(string templateId);
    Task<List<(string, string)>> ListVersions(string templateId);
    Task<string> CreateVersion(string templateId, string versionName, string templateFileName);
    Task UpdateVersion(string templateId, string versionId, string versionName, string templateFileName);
    Task DeleteVersion(string templateId, string versionId);
}