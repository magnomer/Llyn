using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CFootnote
{
    private readonly LEntryPort _cFootnoteEntryPort;

    private readonly LPortraitPort _cFootnotePortraitPort;

    private readonly CEditor _cFootnoteEditor;

    private LVista? _cFootnoteParent;

    private LVista? _cFootnoteVista;

    internal CFootnote(
        LEntryPort entries,
        LPortraitPort portraits,
        CEditor editor,
        CEnvoy envoy,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);

        _cFootnoteEntryPort = entries;
        _cFootnotePortraitPort = portraits;
        _cFootnoteEditor = editor;
        CFootnotePanel = new CPanel(
            envoy, "List.LoadFailed", null, editor.CEditorDesk.CDeskChangeCheck, finishSeam, shownSeam);
        CFootnotePanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CFootnotePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
    }

    public CPanel CFootnotePanel { get; }

    public string CFootnoteEmptyKey =>
        _cFootnoteVista?.LVistaQueried ?? false ? "Source.Unmatched" : "Source.Vacant";

    internal void CFootnoteVistaRestore(LVista parent, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(vista);

        _cFootnoteParent = parent;
        _cFootnoteVista = vista;
        CFootnotePanel.CPanelVistaRestore(vista);
    }

    public void CFootnoteQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cFootnoteVista?.LVistaQuerySet(query);
    }

    public IReadOnlyList<CVistaRow> CFootnoteRowsRead()
    {
        return _cFootnoteEntryPort.LEngineEntryFind(_cFootnoteParent, _cFootnoteVista)
            .Select(CPanel.CPanelRowRead)
            .ToList();
    }

    public void CFootnoteEntryCreate()
    {
        CFootnotePanel.CPanelFreshOpen();
        if (_cFootnoteParent?.LVistaChosen is long reference)
        {
            _cFootnoteEditor.CEditorReferenceAdd(reference);
        }
    }

    public Task CFootnotePortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _cFootnotePortraitPort.LEnginePortraitPrint(
            _cFootnoteVista, CPortrait.CPortraitLabelRead(label), CPortrait.CPortraitTicketRead(ticket));
    }
}
