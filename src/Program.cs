using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MecabConfig;

class Program
{
    static int Main(string[] args)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.Error.WriteLine("This program is only for Windows");
            return 1;
        }

        if (args.Length == 0)
        {
            ShowUsage();
            return 1;
        }

        var execPrefixSet = false;

        if (!TryGetMecabFolderPath(out var execPrefix))
        {
            Console.Error.WriteLine("mecab not found in PATH");
            return 0;
        }

        foreach (var arg in args)
        {
            const string prefixFlag = "--prefix=";
            var prefixFlagLength = prefixFlag.Length;
            if (arg.StartsWith(prefixFlag))
            {
                if (!execPrefixSet)
                {
                    execPrefix = arg[prefixFlagLength..];
                }
            }

            const string execPrefixFlag = "--exec-prefix=";
            var execPrefixFlagLength = execPrefixFlag.Length;
            if (arg.StartsWith(execPrefixFlag))
            {
                execPrefix = arg[execPrefixFlagLength..];
                execPrefixSet = true;
            }

            switch (arg)
            {
                case "--cxx":
                    Console.WriteLine("clang++");
                    break;

                case "--cc":
                    Console.WriteLine("clang");
                    break;

                case "--prefix":
                    Console.WriteLine(execPrefix);
                    break;

                case "--exec-prefix":
                    Console.WriteLine(execPrefix);
                    break;

                case "--version":
                    Console.WriteLine("0.996-custom-windows");
                    return 0;

                case "--help":
                    ShowUsage();
                    return 0;

                case "--cflags":
                {
                    var sdkPath = Path.Combine(execPrefix, "sdk");
                    Console.WriteLine($"\"-I{sdkPath}\"");
                    break;
                }

                case "--libs":
                {
                    var sdkPath = Path.Combine(execPrefix, "sdk");
                    Console.WriteLine($"\"-L{sdkPath}\" -lmecab -lstdc++");
                    break;
                }

                case "--dicdir":
                    Console.WriteLine(Path.Combine(execPrefix, "dic"));
                    break;

                case "--inc-dir":
                    Console.WriteLine(Path.Combine(execPrefix, "sdk"));
                    break;

                case "--libs-only-L":
                    Console.WriteLine(Path.Combine(execPrefix, "sdk"));
                    break;

                case "--libs-only-l":
                    Console.WriteLine("-lmecab -lstdc++");
                    break;

                case "--libexecdir":
                    Console.WriteLine(Path.Combine(execPrefix, "bin"));
                    break;

                case "--sysconfdir":
                    Console.WriteLine(Path.Combine(execPrefix, "etc"));
                    break;

                default:
                    ShowUsage();
                    return 1;
            }
        }

        return 0;
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

    static void ShowUsage()
    {
        Console.WriteLine(
            @"Usage: mecab-config [OPTIONS]

Options:
  [--cxx]
  [--prefix[=DIR]]
  [--exec-prefix[=DIR]]
  [--libs]
  [--cflags]
  [--dicdir]
  [--libexecdir]
  [--sysconfdir]
  [--libs-only-L]
  [--libs-only-l]
  [--inc-dir]
  [--help]
  [--version]
"
        );
    }
}