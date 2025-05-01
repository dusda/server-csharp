using SPTarkov.Server.Core.Models.Spt.Mod;

namespace SPTarkov.Server;

public class ModLoader(ILogger<ModLoader> logger)
{
  readonly ILogger<ModLoader> _logger = logger;

  public List<SptMod> LoadMods()
  {
    var modsDir = "./user/mods/";
    List<SptMod> mods = [];

    var dirs = Directory
      .CreateDirectory(modsDir)
      .GetDirectories();

    foreach (var dir in dirs)
      try
      {
        var mod = new SptMod(dir);
        mods.Add(mod);
      }
      catch (Exception exc)
      {
        _logger.LogError(exc, "Failed to load mod from {dir}", dir);
      }

    return mods;
  }
}
