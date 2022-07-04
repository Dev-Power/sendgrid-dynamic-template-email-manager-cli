using CliFx.Attributes;

namespace DynamicTemplateManager.Cli.Commands;

public abstract class SendGridCommandBase
{
    [CommandOption("sendgridApiKey", IsRequired = true, EnvironmentVariable = "SENDGRID_API_KEY")]
    public string SendGridApiKey { get; init; }
}