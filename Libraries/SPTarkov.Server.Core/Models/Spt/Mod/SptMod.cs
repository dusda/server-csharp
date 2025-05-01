using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SPTarkov.Server.Core.Models.Spt.Mod;

public sealed class SptMod
{
  readonly DirectoryInfo _directory;
  readonly PackageJsonData _packageJson;
  readonly List<Assembly> _assemblies = [];
  readonly AssemblyLoadContext _context = AssemblyLoadContext.Default;
  readonly StringComparison _stringComparison = StringComparison.InvariantCultureIgnoreCase;

  public SptMod(DirectoryInfo directory)
  {
    _directory = directory;
    var files = _directory.GetFiles();

    var package = files.SingleOrDefault(_ =>
      _.Name.Equals("package.json", _stringComparison)) ??
      throw new ArgumentNullException($"No package.json found in path: {Path.GetFullPath(_directory.FullName)}");

    _packageJson = JsonSerializer.Deserialize<PackageJsonData>(File.ReadAllText(package.FullName)) ??
      throw new ArgumentNullException($"Failed to deserialize package.json in path: {Path.GetFullPath(_directory.FullName)}");

    if (_packageJson.Name == null || _packageJson.Author == null ||
        _packageJson.Version == null || _packageJson.Licence == null ||
        _packageJson.SptVersion == null)
      throw new ArgumentException($"The package.json is missing one of these properties: name, author, licence, version or sptVersion");

    _assemblies.AddRange(files
      .Where(_ => _.Extension.Equals(".dll", _stringComparison))
      .Select(_ => _context.LoadFromAssemblyPath(Path.GetFullPath(_.FullName))));

    if (Assemblies.Count == 0)
      throw new ArgumentNullException($"No Assemblies found in path: {Path.GetFullPath(_directory.FullName)}");
  }

  [JsonPropertyName("directory")]
  public DirectoryInfo Directory { get => _directory; }

  [JsonPropertyName("packageJson")]
  public PackageJsonData PackageJson { get => _packageJson; }

  [JsonPropertyName("assemblies")]
  public List<Assembly> Assemblies { get => _assemblies; }
}
