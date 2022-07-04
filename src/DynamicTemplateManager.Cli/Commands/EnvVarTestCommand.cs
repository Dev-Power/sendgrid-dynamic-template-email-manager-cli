using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace DynamicTemplateManager.Cli.Commands;

[Command]
public class EnvVarTestCommand : SendGridCommandBase, ICommand
{
    public ValueTask ExecuteAsync(IConsole console)
    {
        Console.WriteLine(SendGridApiKey);
        return default;
    }
}