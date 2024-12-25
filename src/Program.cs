using System;
using System.IO;
using System.Diagnostics;

namespace MecabConfig;

class Program
{
    static int Main(string[] args)
    {
        if (!TryGetMecabFolderPath(out var prefix))
        {
            Console.Error.WriteLine("mecab not found in PATH");
            return 0;
        }

        if (args.Length == 0)
        {
            ShowUsage();
            return 1;
        }

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "--version":
                    Console.WriteLine("0.996-custom");
                    return 0;

                case "--help":
                    ShowUsage();
                    return 0;

                case "--dicdir":
                    Console.WriteLine(Path.Combine(prefix, "dic"));
                    return 0;

                case "--libexecdir":
                    Console.WriteLine(Path.Combine(prefix, "bin"));
                    return 0;

                case "--sysconfdir":
                    Console.WriteLine(Path.Combine(prefix, "etc"));
                    return 0;

                case "--inc-dir":
                    Console.WriteLine(Path.Combine(prefix, "sdk"));
                    return 0;

                case "--cflags":
                {
                    var sdkPath = Path.Combine(prefix, "sdk");
                    Console.WriteLine($"\"-I{sdkPath}\"");
                    return 0;
                }

                case "--libs":
                {
                    var sdkPath = Path.Combine(prefix, "sdk");
                    Console.WriteLine($"\"-L{sdkPath}\" -lmecab -lstdc++");
                    return 0;
                }

                default:
                    ShowUsage();
                    return 1;
            }
        }

        return 1; // Unreachable
    }

    static bool TryGetMecabFolderPath(out string mecabDir)
    {
        // Windows とそれ以外で分ける簡易的な例
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
            using (var process = Process.Start(startInfo))
            {
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
        }
        catch
        {
            mecabDir = string.Empty;
            return false;
        }
    }

    static void ShowUsage()
    {
        Console.WriteLine(
            @"Usage: mecab-config [OPTIONS]

Options:
  [--dicdir]
  [--libexecdir]
  [--sysconfdir]
  [--help]
  [--version]
  [--inc-dir]
  [--cflags]
  [--libs]"
        );
    }
}