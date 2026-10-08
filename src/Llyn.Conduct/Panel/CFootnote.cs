using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CFootnote
{
    private readonly LVistaPort _cFootnoteVistaPort;

    private readonly LPortraitPort _cFootnotePortraitPort;

    private readonly LSettingsPort _cFootnoteSettingsPort;

    private readonly CEnvoy _cFootnoteEnvoy;

    private readonly CEditor _cFootnoteEditor;

    private LVista? _cFootnoteParent;

    internal CFootnote(
        LVistaPort vistas,
        LPortraitPort portraits,
        LSettingsPort settings,
        CEditor editor,
        CEnvoy envoy,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(vistas);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(envoy);

        _cFootnoteVistaPort = vistas;
        _cFootnotePortraitPort = portraits;
        _cFootnoteSettingsPort = settings;
        _cFootnoteEditor = editor;
        _cFootnoteEnvoy = envoy;
        CFootnotePanel = new CPanel(
            envoy,
            settings,
            vistas,
            "List.LoadFailed",
            null,
            editor.CEditorDesk.LDeskChangeCheck,
            finishSeam,
            shownSeam,
            "Source.Vacant",
            "Source.Unmatched");
        CFootnotePanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        CFootnotePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
    }

    public CPanel CFootnotePanel { get; }

    internal void LFootnoteVistaRestore(LVista parent, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(vista);

        _cFootnoteParent = parent;
        CFootnotePanel.CPanelVistaRestore(vista);
    }

    internal void LFootnoteObserverAttach(Action<Action> marshal, Action roll, Action chosen)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(chosen);

        CFootnotePanel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => CFootnotePanel.CPanelEntryResonate(bulletin)));
        CFootnotePanel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectEntry, _ => marshal(roll));
        CFootnotePanel.CPanelAperture.CApertureChosenAttach(CSubject.CSubjectEntry, _ => marshal(chosen));
        CFootnotePanel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => marshal(CFootnotePanel.CPanelAperture.CApertureRowsResonate));
    }

    public IReadOnlyList<CVistaRow> CFootnoteRowsRead()
    {
        try
        {
            return _cFootnoteVistaPort.LEngineEntryFind(_cFootnoteParent, CFootnotePanel.CPanelAperture.CApertureVista)
                .Select(CCatalog.LCatalogRowRead)
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cFootnoteEnvoy, _cFootnoteSettingsPort, "List.LoadFailed", exception);
            return [];
        }
    }

    public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CFootnoteRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cFootnoteEnvoy, _cFootnoteSettingsPort, "List.LoadFailed", store, CFootnoteRowsRead);

    internal string LFootnoteFileRead()
    {
        return _cFootnoteVistaPort.LEngineFileRead(CFootnotePanel.CPanelAperture.CApertureVista);
    }

    internal void LFootnoteEntryCreate()
    {
        long? reference = _cFootnoteParent?.LVistaChosen;
        CFootnotePanel.CPanelFreshOpen();
        _cFootnoteEditor.CEditorDesk.LDeskRun((drafts, vista) => drafts.LEngineFootnoteStart(vista, reference));
    }

    internal Task LFootnotePortraitPrint(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitTicketPrint(
            envoy,
            settings,
            chosen => _cFootnotePortraitPort.LEnginePortraitPrint(
                CFootnotePanel.CPanelAperture.CApertureVista, CPortrait.LPortraitLabelRead(settings), chosen));
    }

    internal Task LFootnotePortraitExport(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitFileExport(
            envoy,
            settings,
            LFootnoteFileRead,
            (file, medium) => _cFootnotePortraitPort.LEnginePortraitExport(
                CFootnotePanel.CPanelAperture.CApertureVista, file, medium, CPortrait.LPortraitLabelRead(settings)));
    }
}
