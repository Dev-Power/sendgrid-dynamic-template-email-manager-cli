using CliFx;
using DynamicTemplateManager.Cli.Commands.TemplateCommands;
using DynamicTemplateManager.Cli.Services.Impl;
using DynamicTemplateManager.Cli.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilderContext, services) => services
        // Register services
        .AddTransient<IDynamicTemplateService, DynamicTemplateService>()
        
        // Register commands
        .AddTransient<ListTemplatesCommand>()
    )
    .Build();

await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .SetExecutableName("dtm")
    .SetDescription("CLI to manage SendGrid Dynamic Email Templates")
    .SetTitle("SendGrid Dynamic Email Template Manager")
    .UseTypeActivator(host.Services)
    .Build()
    .RunAsync();
        
