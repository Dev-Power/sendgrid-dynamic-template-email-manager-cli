FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DynamicTemplateManager.Cli/DynamicTemplateManager.Cli.csproj", "DynamicTemplateManager.Cli/"]
RUN dotnet restore "DynamicTemplateManager.Cli/DynamicTemplateManager.Cli.csproj"
COPY . .
WORKDIR "/src/DynamicTemplateManager.Cli"
RUN dotnet build "DynamicTemplateManager.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DynamicTemplateManager.Cli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamicTemplateManager.Cli.dll"]
