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
        CFootnotePanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        CFootnotePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
    }

    public CPanel CFootnotePanel { get; }

    public string CFootnoteEmptyKey =>
        _cFootnoteVista?.LVistaQueried ?? false ? "Source.Unmatched" : "Source.Vacant";

    internal void LFootnoteVistaRestore(LVista parent, LVista vista)
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

    public string CFootnoteFileRead()
    {
        return LVista.LVistaFileRead(_cFootnoteVista);
    }

    internal void LFootnoteEntryCreate()
    {
        long? reference = _cFootnoteParent?.LVistaChosen;
        CFootnotePanel.CPanelFreshOpen();
        _cFootnoteEditor.CEditorDesk.LDeskFootnoteStart(reference);
    }

    internal Task LFootnotePortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _cFootnotePortraitPort.LEnginePortraitPrint(
            _cFootnoteVista, CPortrait.CPortraitLabelRead(label), CPortrait.CPortraitTicketRead(ticket));
    }

    internal Task LFootnotePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _cFootnotePortraitPort.LEnginePortraitExport(
            _cFootnoteVista, path, CPortrait.CPortraitMediumRead(format), CPortrait.CPortraitLabelRead(label));
    }
}
