namespace SPTarkov.Server
{
  public class ServerSettings
  {
    public string? SptVersion { get; set; }
    public string? Commit { get; set; }
    public double? BuildTime { get; set; }
    public string? BuildType { get; set; }
    public bool ModsEnabled { get; set; }
  }
}
