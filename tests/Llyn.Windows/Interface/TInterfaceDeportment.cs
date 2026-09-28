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
    internal static LEditor TEditorCreate(LEngine engine) => new(TInterfaceConduct.TEditorCreate(engine));

    internal static void TEditorVistaRestore(this LEditor editor, LVista vista) =>
        editor.LEditorStudio.TEditorVistaRestore(vista);

    internal static LLectern TLecternCreate(LEditor editor) => new(editor.LEditorStudio.CEditorDisplay);

    internal static bool TDeskChangeCheck(this CDesk desk) => desk.CDeskChangeCheck();

    internal static (bool CDeskBackward, bool CDeskForward) TDeskChronicleRead(this CDesk desk) =>
        desk.CDeskChronicleRead();

    internal static QPosture TPostureCreate(string root) => new(() => root);

    internal static IReadOnlyList<QIndexItem> TIndexItemBuild(IReadOnlyList<CVistaRow> rows) =>
        QIndexItem.QIndexItemBuild(rows);

    internal static bool TIndexItemMatch(QIndexItem held, QIndexItem fresh) => QIndexItem.QIndexItemMatch(held, fresh);

    internal static void TIndexItemSync(QIndexItem held, QIndexItem fresh) => QIndexItem.QIndexItemSync(held, fresh);

    internal static long? TIndexNeighbourFind(IReadOnlyList<QIndexItem> items, bool down) =>
        QIndex.QIndexNeighbourFind(items, down);

    internal static bool TCaretKeyApply(
        string key, int caret, int length, int selection, Action<int> remove, Func<int, bool> move, Action place) =>
        QCaret.QCaretKeyApply(key, caret, length, selection, remove, move, place);

    internal static void TLecternFoldSet(this LLectern lectern, bool opened) => lectern.LLecternFoldSet(opened);

    internal static void TLecternReflexAttach(
        this LLectern lectern, ItemsControl reflex, UIElement loading, ToggleButton fold) =>
        lectern.LLecternSound.LLecternReflexAttach(reflex, loading, fold);

    internal static void TLecternFoldHandle(this LLectern lectern, bool opened) =>
        lectern.LLecternSound.LLecternFoldHandle(opened);

    internal static void TReflexFoldRefine(IReadOnlyList<QReflexItem> rows, ToggleButton fold, bool opened) =>
        QReflexItem.QReflexFoldRefine(rows, fold, opened);
}
