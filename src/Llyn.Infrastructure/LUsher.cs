using System;
using System.Diagnostics;
using System.IO;

namespace Llyn.Infrastructure;

public static class LUsher
{
    public static void LUsherFolderOpen(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }

    public static void LUsherLinkOpen(string address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(address);

        Process.Start(new ProcessStartInfo(address) { UseShellExecute = true });
    }

    public static bool LUsherPathExist(string? path)
    {
        return path is not null && File.Exists(path);
    }
}
