using SPTarkov.Common.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.External;
using SPTarkov.Server.Core.Models.Utils;

namespace SPTarkov.Server.Core.Loaders;

[Injectable(InjectableTypeOverride = typeof(IOnLoad), TypePriority = OnLoadOrder.PostSptModLoader)]
public class PostSptModLoader(
    ISptLogger<PostSptModLoader> _logger,
    IEnumerable<IPostSptLoadMod> _postSptLoadMods,
    ServerSettings _settings
) : IOnLoad
{
  public async Task OnLoad()
  {
    if (!_settings.ModsEnabled) return;

    _logger.Info("Loading PostSptMods...");

    foreach (var mod in _postSptLoadMods)
      mod.PostSptLoad();

    _logger.Info("Finished loading PostSptMods...");

    await Task.CompletedTask;
  }

  public string GetRoute() => "spt-post-spt-mods";
}
