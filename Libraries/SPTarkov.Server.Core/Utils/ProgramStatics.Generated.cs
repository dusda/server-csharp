using SPTarkov.Server.Core.Models.Enums;

namespace SPTarkov.Server.Core.Utils;

public static partial class ProgramStatics
{
    static string? _sptVersion = "4.0.0";
    static string? _commit = "a12b34";
    static double? _buildTime = 0000000000;
    static EntryType? BuildType = EntryType.LOCAL;
}
