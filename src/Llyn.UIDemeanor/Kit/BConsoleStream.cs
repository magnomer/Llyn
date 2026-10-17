using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Llyn.UIDemeanor;

public sealed class BConsoleStream : BConsole
{
    private ConsoleCancelEventHandler? _bConsoleStreamInterruption;
    private Encoding? _bConsoleStreamOutput;
    private Encoding? _bConsoleStreamInput;
    private StrongBox<bool>? _bConsoleStreamControl;

    public override void BConsoleNoticePrint(string text)
    {
        Console.Error.WriteLine(text);
    }

    public override void BConsoleResultPrint(string text)
    {
        Console.Out.WriteLine(text);
    }

    public override string? BConsoleLineRead()
    {
        return Console.In.ReadLine();
    }

    public override bool BConsoleTerminalCheck()
    {
        return !Console.IsInputRedirected && !Console.IsOutputRedirected;
    }

    public override void BConsoleFramePrint(string frame)
    {
        Console.Out.Write(frame);
        Console.Out.Flush();
    }

    public override (int BConsoleWidth, int BConsoleHeight) BConsoleSizeRead()
    {
        return (Console.WindowWidth, Console.WindowHeight);
    }

    public override ConsoleKeyInfo BConsoleKeyRead()
    {
        return Console.ReadKey(intercept: true);
    }

    public override void BConsoleWindowApply()
    {
        StrongBox<bool> control = new(Console.TreatControlCAsInput);
        Console.TreatControlCAsInput = true;
        _bConsoleStreamControl = control;
        BConsoleFramePrint("\e[?1049h\e[?25l");
    }

    public override void BConsoleWindowRestore()
    {
        if (Interlocked.Exchange(ref _bConsoleStreamControl, null) is StrongBox<bool> control)
        {
            BConsoleFramePrint("\e[?25h\e[?1049l");
            Console.TreatControlCAsInput = control.Value;
        }
    }

    public override void BConsoleModeApply(Action interruption)
    {
        ArgumentNullException.ThrowIfNull(interruption);

        Encoding output = Console.OutputEncoding;
        Encoding input = Console.InputEncoding;
        UTF8Encoding utf8 = new(encoderShouldEmitUTF8Identifier: false);
        Console.OutputEncoding = utf8;
        Console.InputEncoding = utf8;
        _bConsoleStreamOutput = output;
        _bConsoleStreamInput = input;
        _bConsoleStreamInterruption = (_, _) =>
        {
            try
            {
                interruption();
            }
            finally
            {
                BConsoleWindowRestore();
                BConsoleModeRestore();
                Environment.Exit(130);
            }
        };
        Console.CancelKeyPress += _bConsoleStreamInterruption;
    }

    public override void BConsoleModeRestore()
    {
        Console.CancelKeyPress -= _bConsoleStreamInterruption;

        if (Interlocked.Exchange(ref _bConsoleStreamOutput, null) is Encoding output)
        {
            Console.OutputEncoding = output;
        }

        if (Interlocked.Exchange(ref _bConsoleStreamInput, null) is Encoding input)
        {
            Console.InputEncoding = input;
        }
    }
}
