using Llyn.UIDemeanor;

namespace Llyn.Tests;

internal sealed class TConsoleFake : BConsole
{
    public List<string> TConsoleFakeOutput { get; } = [];

    public List<string> TConsoleFakeError { get; } = [];

    public bool TConsoleFakeTerminal { get; set; }

    public string? TConsoleFakeAnswer { get; set; }

    public override void BConsoleNoticePrint(string text)
    {
        TConsoleFakeError.Add(text);
    }

    public override void BConsoleResultPrint(string text)
    {
        TConsoleFakeOutput.Add(text);
    }

    public override string? BConsoleLineRead()
    {
        return TConsoleFakeAnswer;
    }

    public override bool BConsoleTerminalCheck()
    {
        return TConsoleFakeTerminal;
    }

    public override void BConsoleModeApply()
    {
    }

    public override void BConsoleModeRestore()
    {
    }
}
