using System;
using System.IO;

namespace MecabConfig;

class Program
{
    static int Main(string[] args)
    {
        if (!OsDetector.IsWindows())
        {
            Console.Error.WriteLine("This program is only for Windows");
            return 1;
        }

        if (args.Length == 0)
        {
            ShowUsage();
            return 1;
        }

        var pathStateMachine = new PathStateMachine();

        foreach (var arg in args)
        {
            const string prefixFlag = "--prefix=";
            if (arg.StartsWith(prefixFlag))
            {
                pathStateMachine.SetPrefix(arg[prefixFlag.Length..]);
                continue;
            }

            const string execPrefixFlag = "--exec-prefix=";
            if (arg.StartsWith(execPrefixFlag))
            {
                pathStateMachine.SetExecPrefix(arg[execPrefixFlag.Length..]);
                continue;
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
                    Console.WriteLine(pathStateMachine.GetExecPrefix());
                    break;

                case "--exec-prefix":
                    Console.WriteLine(pathStateMachine.GetExecPrefix());
                    break;

                case "--version":
                    Console.WriteLine("0.996-custom-windows");
                    return 0;

                case "--help":
                    ShowUsage();
                    return 0;

                case "--cflags":
                {
                    var includePath = pathStateMachine.GetIncludePath();
                    var isUsrInclude = includePath == Path.Combine(Path.DirectorySeparatorChar.ToString(), "usr", "include");
                    Console.WriteLine(isUsrInclude ? "" : $"\"-I{includePath}\"");
                    break;
                }

                case "--libs":
                {
                    Console.WriteLine($"\"-L{pathStateMachine.GetLibPath()}\" -lmecab -lstdc++");
                    break;
                }

                case "--dicdir":
                    Console.WriteLine(pathStateMachine.GetDicPath());
                    break;

                case "--inc-dir":
                {
                    Console.WriteLine(pathStateMachine.GetIncludePath());
                    break;
                }

                case "--libs-only-L":
                {
                    Console.WriteLine(pathStateMachine.GetLibPath());
                    break;
                }

                case "--libs-only-l":
                    Console.WriteLine("mecab stdc++");
                    break;

                case "--libexecdir":
                {
                    Console.WriteLine(pathStateMachine.GetLibExecPath());
                    break;
                }

                case "--sysconfdir":
                    Console.WriteLine(pathStateMachine.GetSysConfPath());
                    break;

                default:
                    ShowUsage();
                    return 1;
            }
        }

        return 0;
    }



    private static void ShowUsage()
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