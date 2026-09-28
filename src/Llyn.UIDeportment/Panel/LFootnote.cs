using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LFootnote
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lFootnoteParent;

    private LVista? _lFootnoteVista;

    private int _lFootnoteCount;

    internal LFootnote(
        LEntryPort entries, LPortraitPort portraits, LEditor editor, Func<bool> shownSeam, Func<bool> leaveSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        LFootnoteEditor = editor;
        LFootnotePanel = new LPanel(
            "List.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorStudio.CEditorDesk.CDeskChangeCheck, shownSeam, leaveSeam, static () => false);
        LFootnotePanel.LPanelCleared += LFootnoteEditorClear;
        LFootnotePanel.LPanelEdited += LFootnoteEditorOpen;
        LFootnoteCreated += editor.LEditorStudio.CEditorReferenceAdd;
    }

    public event Action<long>? LFootnoteCreated;

    public LEditor LFootnoteEditor { get; }

    public LPanel LFootnotePanel { get; }

    private void LFootnoteEditorClear()
    {
        LFootnoteEditor.LEditorStudio.CEditorEntryOpen(null);
    }

    private void LFootnoteEditorOpen(long id)
    {
        LFootnoteEditor.LEditorStudio.CEditorEntryOpen(id);
    }

    public bool LFootnoteEmpty => _lFootnoteCount == 0;

    public string LFootnoteEmptyKey => LFootnoteQueried ? "Source.Unmatched" : "Source.Vacant";

    private bool LFootnoteQueried => _lFootnoteVista?.LVistaQueried ?? false;

    internal void LFootnoteVistaRestore(LVista parent, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(vista);

        _lFootnoteParent = parent;
        _lFootnoteVista = vista;
        LFootnotePanel.LPanelVistaRestore(vista);
    }

    public IReadOnlyList<CVistaRow> LFootnoteRowsRead()
    {
        IReadOnlyList<CVistaRow> rows = LSplice.LSpliceBuild(
            _lEntryPort.LEngineEntryFind(_lFootnoteParent, _lFootnoteVista), CPanel.CPanelRowRead);
        _lFootnoteCount = rows.Count;
        return rows;
    }

    public void LFootnoteQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lFootnoteVista?.LVistaQuerySet(query);
    }

    public void LFootnoteEntryCreate()
    {
        LFootnotePanel.LPanelFreshOpen();
        if (_lFootnoteParent?.LVistaChosen is long reference)
        {
            LFootnoteCreated?.Invoke(reference);
        }
    }

    public Task LFootnotePortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lFootnoteVista, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }
}
