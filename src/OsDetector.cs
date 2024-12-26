using System.Runtime.InteropServices;

namespace MecabConfig;

static class OsDetector
{
    public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}