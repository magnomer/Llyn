using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CFavorite
{
    private readonly CAtelier _cFavoriteAtelier;

    private readonly LEntryPort _cFavoriteEntryPort;

    private readonly LPortraitPort _cFavoritePortraitPort;

    private readonly LSettingsPort _cFavoriteSettingsPort;

    private LVista? _cFavoriteVista;

    private CFavorite(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cFavoriteAtelier = atelier;
        _cFavoriteEntryPort = atelier.CAtelierEntryPort;
        _cFavoritePortraitPort = atelier.CAtelierPortraitPort;
        _cFavoriteSettingsPort = atelier.CAtelierSettingsPort;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CFavoriteEditor = editor;
        CFavoritePanel = new CPanel(
            envoy, "Favorite.LoadFailed", "Scribe", editor.CEditorDesk.CDeskChangeCheck, editor.CEditorFinish,
            shownSeam);
        CFavoritePanel.CPanelCleared += () => editor.CEditorEntryOpen(null);
        CFavoritePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
    }

    public static CFavorite CFavoriteCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CFavorite(atelier, shownSeam, envoy);
    }

    public CEditor CFavoriteEditor { get; }

    public CPanel CFavoritePanel { get; }

    public bool CFavoriteFiltered => _cFavoriteVista?.LVistaFiltered ?? false;

    public void CFavoriteVistaRestore()
    {
        LVista vista = _cFavoriteAtelier.CAtelierVistaStart(
            "favorite", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        _cFavoriteVista = vista;
        CFavoritePanel.CPanelVistaRestore(vista);
        CFavoriteEditor.CEditorVistaRestore(vista);
    }

    public void CFavoriteGraspResonate()
    {
        if (CFavoritePanel.CPanelOrder == CCatalogOrder.CCatalogOrderGrasp)
        {
            CFavoritePanel.CPanelRowsResonate();
        }
    }

    public void CFavoriteQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cFavoriteVista?.LVistaQuerySet(query);
    }

    public void CFavoriteOrderSet(CCatalogOrder? order)
    {
        _cFavoriteVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CFavoriteFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cFavoriteVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public IReadOnlyList<CVistaRow> CFavoriteRowsRead()
    {
        return _cFavoriteVista is LVista vista
            ? _cFavoriteEntryPort.LEngineFavoriteFind(vista).Select(CPanel.CPanelRowRead).ToList()
            : [];
    }

    public IReadOnlyList<string> CFavoriteLanguageRead()
    {
        return _cFavoriteSettingsPort.LEngineLanguageRead();
    }

    public string CFavoriteFileRead()
    {
        return LVista.LVistaFileRead(_cFavoriteVista);
    }

    public Task CFavoritePortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _cFavoritePortraitPort.LEnginePortraitPrint(
            _cFavoriteVista, CPortrait.CPortraitLabelRead(label), CPortrait.CPortraitTicketRead(ticket));
    }

    public Task CFavoritePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _cFavoritePortraitPort.LEnginePortraitExport(
            _cFavoriteVista, path, CPortrait.CPortraitMediumRead(format), CPortrait.CPortraitLabelRead(label));
    }
}
