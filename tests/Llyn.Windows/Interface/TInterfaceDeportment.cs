using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Llyn.Tests;

internal static class TInterfaceDeportment
{
    internal static QLectern TLecternCreate(CEditor editor) => new(editor.CEditorDisplay);

    internal static (bool CDeskBackward, bool CDeskForward) TDeskChronicleRead(this CDesk desk) =>
        desk.CDeskChronicleRead();

    internal static QPosture TPostureCreate(string root) => new(() => root);

    internal static IReadOnlyList<QIndexItem> TIndexItemBuild(IReadOnlyList<CVistaRow> rows) =>
        QIndexItem.QIndexItemBuild(rows);

    internal static bool TIndexItemMatch(QIndexItem held, QIndexItem fresh) => QIndexItem.QIndexItemMatch(held, fresh);

    internal static void TIndexItemSync(QIndexItem held, QIndexItem fresh) => QIndexItem.QIndexItemSync(held, fresh);

    internal static bool TCaretEdgeApply(string key, int caret, int length, int selection, Action<int> remove) =>
        QCaret.QCaretEdgeApply(key, caret, length, selection, remove);

    internal static bool TCaretStepApply(string key, int length, int selection, Func<int, bool> move, Action place) =>
        QCaret.QCaretStepApply(key, length, selection, move, place);

    internal static void TDisplayFoldSet(this CEditor editor, bool opened) =>
        editor.CEditorDisplay.CDisplaySound.CDisplayReflexToggle(opened);

    internal static bool TDisplayFoldRead(this CEditor editor) =>
        editor.CEditorDisplay.LDisplaySound.LDisplayFoldOpened;

    internal static void TLecternReflexAttach(
        this QLectern lectern, ItemsControl reflex, UIElement loading, ToggleButton fold) =>
        lectern.QLecternSound.QLecternReflexIntroduce(reflex, loading, fold);

    internal static void TLecternFoldObserve(this QLectern lectern) => lectern.QLecternSound.QLecternFoldObserve();

    internal static void TReflexFoldRefine(IReadOnlyList<QReflexItem> rows, ToggleButton fold, bool opened) =>
        QReflexItem.QReflexFoldRefine(rows, fold, opened);
}
