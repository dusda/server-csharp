using SPTarkov.Common.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.External;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace SPTarkov.Server.Core.Loaders;

[Injectable(InjectableTypeOverride = typeof(IOnLoad), TypePriority = OnLoadOrder.PostDBModLoader)]
public class PostDBModLoader(
    ISptLogger<PostDBModLoader> _logger,
    IEnumerable<IPostDBLoadMod> _postDbLoadMods,
    ServerSettings _settings
) : IOnLoad
{
  public async Task OnLoad()
  {
    if (!_settings.ModsEnabled) return;

    _logger.Info("Loading PostDBMods...");

    foreach (var mod in _postDbLoadMods)
      mod.PostDBLoad();

    _logger.Info("Finished loading PostDBMods...");

    await Task.CompletedTask;
  }

  public string GetRoute() => "spt-post-db-mods";
}
