using System;
using System.IO;

namespace MecabConfig;

public class PathStateMachine
{
    private string? _execPrefix;
    private bool _execPrefixSet;

    public string GetExecPrefix()
    {
        if (_execPrefix != null)
        {
            return _execPrefix;
        }

        if (!MecabPathFinder.TryGetMecabFolderPath(out var mecabDir))
        {
            throw new Exception("mecab not found in PATH");
        }

        _execPrefix = mecabDir;
        return _execPrefix;
    }

    public void SetExecPrefix(string execPrefix)
    {
        _execPrefix = execPrefix;
        _execPrefixSet = true;
    }

    public void SetPrefix(string prefix)
    {
        if (_execPrefixSet)
        {
            return;
        }

        _execPrefix = prefix;
    }


    public string GetLibPath()
    {
        return IsInstalledByInstaller()
            ? Path.Combine(GetExecPrefix(), "sdk")
            : Path.Combine(GetExecPrefix(), "lib");
    }

    public string GetIncludePath()
    {
        return IsInstalledByInstaller()
            ? Path.Combine(GetExecPrefix(), "sdk")
            : Path.Combine(GetExecPrefix(), "include");
    }

    public string GetDicPath()
    {
        return IsInstalledByInstaller()
            ? Path.Combine(GetExecPrefix(), "dic")
            : Path.Combine(GetExecPrefix(), "lib", "mecab", "dic");
    }

    public string GetLibExecPath()
    {
        return IsInstalledByInstaller()
            ? Path.Combine(GetExecPrefix(), "bin")
            : Path.Combine(GetExecPrefix(), "libexec", "mecab");
    }

    private bool IsInstalledByInstaller()
    {
        return InstallerDetector.IsInstalledByInstaller(GetExecPrefix());
    }

    public string GetSysConfPath()
    {
        return Path.Combine(GetExecPrefix(), "etc");
    }
}