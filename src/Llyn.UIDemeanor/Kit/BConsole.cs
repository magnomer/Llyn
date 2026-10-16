namespace Llyn.UIDemeanor;

public abstract class BConsole
{
    public abstract void BConsoleNoticePrint(string text);

    public abstract void BConsoleResultPrint(string text);

    public abstract string? BConsoleLineRead();

    public abstract bool BConsoleTerminalCheck();

    public abstract void BConsoleModeApply();

    public abstract void BConsoleModeRestore();
}
