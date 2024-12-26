using System;
using System.Diagnostics;
using System.IO;

namespace MecabConfig;

public static class MecabPathFinder
{
    public static bool TryGetMecabFolderPath(out string mecabDir)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "where",
            Arguments = "mecab",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process == null)
            {
                mecabDir = string.Empty;
                return false;
            }

            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            if (process.ExitCode != 0 || string.IsNullOrEmpty(output))
            {
                mecabDir = string.Empty;
                return false;
            }

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                mecabDir = string.Empty;
                return false;
            }

            var unsafeMecabBinDir = Path.GetDirectoryName(lines[0]);
            if (string.IsNullOrEmpty(unsafeMecabBinDir))
            {
                mecabDir = string.Empty;
                return false;
            }

            var mecabBinDir = unsafeMecabBinDir;

            var unsafeMecabDir = Path.GetDirectoryName(mecabBinDir);
            if (string.IsNullOrEmpty(unsafeMecabDir))
            {
                mecabDir = string.Empty;
                return false;
            }

            mecabDir = unsafeMecabDir;
            return true;
        }
        catch
        {
            mecabDir = string.Empty;
            return false;
        }
    }

}