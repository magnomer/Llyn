using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.InteropServices;

namespace Llyn.Launcher;

internal static class LConsoleUsher
{
    private const string LConsoleUsherTarget = "Llyn.exe";
    private const string LConsoleUsherMark = "LLYN_LAUNCHER";

    private const int LConsoleUsherStartup = 104;
    private const int LConsoleUsherFlags = 60;
    private const int LConsoleUsherStreams = 80;
    private const int LConsoleUsherInformation = 24;
    private const int LConsoleUsherHandles = 0x100;

    private static int Main() => LConsoleUsherRun();

    private static int LConsoleUsherRun()
    {
        string target = Path.Combine(AppContext.BaseDirectory, LConsoleUsherTarget);
        string line = Marshal.PtrToStringUni(GetCommandLineW()) ?? "";
        char[] command = ("\"" + target + "\"" + LConsoleUsherShorten(line) + "\0").ToCharArray();

        byte[] start = new byte[LConsoleUsherStartup];
        BinaryPrimitives.WriteInt32LittleEndian(start, LConsoleUsherStartup);
        BinaryPrimitives.WriteInt32LittleEndian(start.AsSpan(LConsoleUsherFlags), LConsoleUsherHandles);
        for (int stream = 0; stream < 3; stream++)
        {
            BinaryPrimitives.WriteInt64LittleEndian(
                start.AsSpan(LConsoleUsherStreams + (stream * 8)), GetStdHandle(-10 - stream));
        }

        byte[] process = new byte[LConsoleUsherInformation];
        Environment.SetEnvironmentVariable(LConsoleUsherMark, "console");
        SetConsoleCtrlHandler(IntPtr.Zero, true);
        if (!CreateProcessW(target, command, IntPtr.Zero, IntPtr.Zero, true, 0, IntPtr.Zero, null, start, process))
        {
            Console.Error.WriteLine($"Llyn.com cannot start {target} (error {Marshal.GetLastPInvokeError()}).");
            return 1;
        }

        IntPtr guest = (IntPtr)BinaryPrimitives.ReadInt64LittleEndian(process);
        IntPtr thread = (IntPtr)BinaryPrimitives.ReadInt64LittleEndian(process.AsSpan(8));
        WaitForSingleObject(guest, -1);
        GetExitCodeProcess(guest, out int code);
        CloseHandle(thread);
        CloseHandle(guest);
        return code;
    }

    private static string LConsoleUsherShorten(string line)
    {
        if (line.StartsWith('"'))
        {
            int close = line.IndexOf('"', 1);
            return close < 0 ? "" : line[(close + 1)..];
        }

        int gap = line.IndexOfAny([' ', '\t']);
        return gap < 0 ? "" : line[gap..];
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCommandLineW();

    [DllImport("kernel32.dll")]
    private static extern long GetStdHandle(int which);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetConsoleCtrlHandler(IntPtr handler, [MarshalAs(UnmanagedType.Bool)] bool add);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateProcessW(
        string application,
        [In, Out] char[] command,
        IntPtr processAttributes,
        IntPtr threadAttributes,
        [MarshalAs(UnmanagedType.Bool)] bool inherit,
        int flags,
        IntPtr environment,
        string? directory,
        [In] byte[] start,
        [Out] byte[] process);

    [DllImport("kernel32.dll")]
    private static extern int WaitForSingleObject(IntPtr handle, int milliseconds);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetExitCodeProcess(IntPtr process, out int code);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr handle);
}
