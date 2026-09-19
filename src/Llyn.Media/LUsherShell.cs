using System;
using System.Diagnostics;

namespace Llyn.Media;

public static class LUsherShell
{
    public static void LUsherShellOpen(string target)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(target);

        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
    }
}
