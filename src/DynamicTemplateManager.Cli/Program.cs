using CliFx;

await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .SetExecutableName("dtm")
    .SetDescription("CLI to manage SendGrid Dynamic Email Templates")
    .SetTitle("SendGrid Dynamic Email Template Manager")
    .Build()
    .RunAsync();
        
