using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal static class BInvocationLectern
{
    internal static void BInvocationLecternObserve(
        long id, CPanel panel, BLectern lectern, BEnvoyStrict envoy, BConsole console)
    {
        ArgumentNullException.ThrowIfNull(panel);
        ArgumentNullException.ThrowIfNull(lectern);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(console);

        const int plain = 80;
        bool opened = panel.CPanelRowSelect(id) == CPanelOutcome.CPanelOutcomeOpened;
        if (!opened && !envoy.BEnvoyStrictRefusal)
        {
            envoy.CEnvoyFailureShow("Invocation.EntryMissing");
        }

        if (!opened)
        {
            return;
        }

        int width = console.BConsoleTerminalCheck() ? console.BConsoleSizeRead().BConsoleWidth : plain;
        BInvocationLecternPrint(
            lectern.BLecternPageBuild(new Dictionary<string, IReadOnlyList<string>>(), width, new BCanvasWidth(false)),
            console);
    }

    private static void BInvocationLecternPrint(IReadOnlyList<BLecternLine> lines, BConsole console)
    {
        foreach (BLecternLine line in lines)
        {
            string text = string.Concat(line.BLecternLinePieces.Select(static piece => piece.BLecternLineText));
            console.BConsoleResultPrint((new string(' ', line.BLecternLineIndent) + text).TrimEnd(' '));
        }
    }
}
