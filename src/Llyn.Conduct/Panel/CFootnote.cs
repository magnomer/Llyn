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

    private readonly LSettingsPort _cFootnoteSettingsPort;

    private readonly CEditor _cFootnoteEditor;

    private LVista? _cFootnoteParent;

    private LVista? _cFootnoteVista;

    internal CFootnote(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        CEditor editor,
        CEnvoy envoy,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);

        _cFootnoteEntryPort = entries;
        _cFootnotePortraitPort = portraits;
        _cFootnoteSettingsPort = settings;
        _cFootnoteEditor = editor;
        CFootnotePanel = new CPanel(
            envoy,
            settings,
            "List.LoadFailed", null, editor.CEditorDesk.LDeskChangeCheck, finishSeam, shownSeam);
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

        vista.LVistaQuerySet(_cFootnoteVista?.LVistaQuery ?? string.Empty);
        _cFootnoteParent = parent;
        _cFootnoteVista = vista;
        CFootnotePanel.CPanelVistaRestore(vista);
    }

    internal void LFootnoteObserverAttach(Action<Action> marshal, Action roll, Action chosen)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(chosen);

        CFootnotePanel.CPanelObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => CFootnotePanel.CPanelEntryResonate(bulletin)));
        CFootnotePanel.CPanelObserverAttach(CSubject.CSubjectEntry, _ => marshal(roll));
        CFootnotePanel.CPanelChosenAttach(CSubject.CSubjectEntry, _ => marshal(chosen));
        CFootnotePanel.CPanelObserverAttach(
            CSubject.CSubjectVista, _ => marshal(CFootnotePanel.CPanelRowsResonate));
    }

    public void CFootnoteQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cFootnoteVista?.LVistaQuerySet(query);
    }

    public IReadOnlyList<CVistaRow> CFootnoteRowsRead()
    {
        return _cFootnoteEntryPort.LEngineEntryFind(_cFootnoteParent, _cFootnoteVista)
            .Select(CCatalog.LCatalogRowRead)
            .ToList();
    }

    public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CFootnoteRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(_cFootnoteSettingsPort, store, CFootnoteRowsRead);

    internal string LFootnoteFileRead()
    {
        return LVista.LVistaFileRead(_cFootnoteVista);
    }

    internal void LFootnoteEntryCreate()
    {
        long? reference = _cFootnoteParent?.LVistaChosen;
        CFootnotePanel.CPanelFreshOpen();
        _cFootnoteEditor.CEditorDesk.LDeskFootnoteStart(reference);
    }

    internal Task LFootnotePortraitPrint(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitTicketPrint(
            envoy,
            settings,
            chosen => _cFootnotePortraitPort.LEnginePortraitPrint(
                _cFootnoteVista, CPortrait.LPortraitLabelRead(settings), chosen));
    }

    internal Task LFootnotePortraitExport(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitFileExport(
            envoy,
            settings,
            LFootnoteFileRead,
            (file, medium) => _cFootnotePortraitPort.LEnginePortraitExport(
                _cFootnoteVista, file, medium, CPortrait.LPortraitLabelRead(settings)));
    }
}
