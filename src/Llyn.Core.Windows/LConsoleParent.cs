using System;
using System.Runtime.InteropServices;
using Llyn.Core;

namespace Llyn.Core.Windows;

public sealed class LConsoleParent : LConsole
{
    private const string LConsoleParentMark = "LLYN_LAUNCHER";
    private const int LConsoleParentProcess = -1;

    public bool LConsoleAttach()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable(LConsoleParentMark), "console", StringComparison.Ordinal))
        {
            return false;
        }

        Environment.SetEnvironmentVariable(LConsoleParentMark, null);
        AttachConsole(LConsoleParentProcess);
        return true;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AttachConsole(int process);
}
