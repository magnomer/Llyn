using System;

namespace Llyn.UIDemeanor;

public abstract class BConsole
{
    public abstract void BConsoleFramePrint(string frame);

    public abstract (int BConsoleWidth, int BConsoleHeight) BConsoleSizeRead();

    public abstract ConsoleKeyInfo BConsoleKeyRead();

    public abstract void BConsoleWindowApply();

    public abstract void BConsoleWindowRestore();

    public abstract void BConsoleNoticePrint(string text);

    public abstract void BConsoleResultPrint(string text);

    public abstract string? BConsoleLineRead();

    public abstract bool BConsoleTerminalCheck();

    public abstract void BConsoleModeApply(Action interruption);

    public abstract void BConsoleModeRestore();
}
