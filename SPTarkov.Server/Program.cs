using System.Runtime;
using Serilog;
using Serilog.Exceptions;
using SPTarkov.Common.Semver;
using SPTarkov.Common.Semver.Implementations;
using SPTarkov.DI;
using SPTarkov.Server;
using SPTarkov.Server.Core.Context;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.External;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Logger;
using SPTarkov.Server.Modding;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, provider, logger) =>
  logger
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(provider)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .Enrich.WithThreadName()
    .Enrich.WithThreadId());

services
  .Configure<ServerSettings>(config.GetSection("Settings"));

services
  .AddScoped<ModLoader>()
  .AddScoped<ISptLogger<ModValidator>, SptWebApplicationLogger<ModValidator>>()
  .AddScoped<ISemVer, SemanticVersioningSemVer>()
  .AddSingleton<ModValidator>()
  .AddSingleton<ModLoadOrder>();

// register SPT components
DependencyInjectionRegistrator.RegisterSptComponents(typeof(Program).Assembly, typeof(App).Assembly, builder.Services);

var app = builder.Build();
var provider = app.Services;
var logger = provider.GetService<ILoggerFactory>()!.CreateLogger("Server");
var settings = provider.GetService<ServerSettings>()!;

try
{
  var watermark = provider.GetService<Watermark>()!;
  watermark.Initialize();

  var context = provider.GetService<ApplicationContext>()!;
  context.AddValue(ContextVariableType.SERVICE_PROVIDER, provider);

  // Initialize PreSptMods
  var preSptLoadMods = provider.GetServices<IPreSptLoadMod>();
  foreach (var preSptLoadMod in preSptLoadMods)
    preSptLoadMod.PreSptLoad();

  if (settings.ModsEnabled)
  {
    var modLoader = provider.GetService<ModLoader>()!;
    var modValidator = provider.GetService<ModValidator>()!;
    var mods = modLoader.LoadMods();
    // validate and sort mods, this will also discard any mods that are invalid
    var sortedLoadedMods = modValidator.ValidateAndSort(mods);
    // for harmony, we use the original list, as some mods may only be bepinex patches only
    HarmonyBootstrapper.LoadAllPatches([.. mods.SelectMany(asm => asm.Assemblies)]);

    // register mod components from the filtered list
    DependencyInjectionRegistrator
      .RegisterModOverrideComponents(builder.Services, [.. sortedLoadedMods.SelectMany(a => a.Assemblies)]);

    // Add the Loaded Mod Assemblies for later
    context.AddValue(ContextVariableType.LOADED_MOD_ASSEMBLIES, mods);
  }

  // This is the builder that will get use by the HttpServer to start up the web application
  context.AddValue(ContextVariableType.APP_BUILDER, builder);

  // Get the Built app and run it
  app.Run();

  // Run garbage collection now the server is ready to start
  GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
  GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);

  // When the application is started by the HttpServer it will be added into the AppContext of the WebApplication
  // object, which we can use here to start the webapp.
  context
    .GetLatestValue(ContextVariableType.WEB_APPLICATION)!
    .GetValue<WebApplication>()
    .Run();
}
catch (Exception ex)
{
  logger.LogCritical(ex, "Critical exception, stopping server...");
}
