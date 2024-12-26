using System.IO;

namespace MecabConfig;

public static class InstallerDetector
{
    public static bool IsInstalledByInstaller(string execPrefix)
    {
        return Directory.Exists(Path.Combine(execPrefix, "sdk"));
    }
}