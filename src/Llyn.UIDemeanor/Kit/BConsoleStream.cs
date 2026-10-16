using System;
using System.Text;
using System.Threading;

namespace Llyn.UIDemeanor;

public sealed class BConsoleStream : BConsole
{
    private readonly ConsoleCancelEventHandler _bConsoleStreamInterruption;
    private Encoding? _bConsoleStreamOutput;
    private Encoding? _bConsoleStreamInput;

    public BConsoleStream()
    {
        _bConsoleStreamInterruption = (_, _) =>
        {
            BConsoleModeRestore();
            Environment.Exit(130);
        };
    }

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

    public override void BConsoleModeApply()
    {
        Encoding output = Console.OutputEncoding;
        Encoding input = Console.InputEncoding;
        UTF8Encoding utf8 = new(encoderShouldEmitUTF8Identifier: false);
        Console.OutputEncoding = utf8;
        Console.InputEncoding = utf8;
        _bConsoleStreamOutput = output;
        _bConsoleStreamInput = input;
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
