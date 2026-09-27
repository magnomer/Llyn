using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LTaxonomy
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lTaxonomyVista;

    private LVista? _lTaxonomyMembership;

    internal LTaxonomy(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        LTaxonomyEditor = editor;

        LTaxonomyPanel = new LPanel(
            "Tag.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorDesk.CDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LTaxonomyPanel.LPanelCleared += LTaxonomyEditorClear;
        LTaxonomyPanel.LPanelEdited += LTaxonomyEditorOpen;
        LTaxonomyPanel.LPanelDraftChanged += lectern.LLecternDraftShow;
        LTaxonomyPanel.LPanelCleared += lectern.LLecternClear;
    }

    public LEditor LTaxonomyEditor { get; }

    public LPanel LTaxonomyPanel { get; }

    private void LTaxonomyEditorClear()
    {
        LTaxonomyEditor.LEditorOpen(null);
    }

    private void LTaxonomyEditorOpen(long id)
    {
        LTaxonomyEditor.LEditorOpen(id);
    }

    public void LTaxonomyEntryCreate()
    {
        LTaxonomyPanel.LPanelFreshOpen();
        if (LTaxonomyChosen is long tag)
        {
            LTaxonomyEditor.LEditorTagAdd(tag);
        }
    }

    public bool LTaxonomyCoinageCheck()
    {
        return LTaxonomyChosen is null && !LTaxonomyPanel.LPanelBinEnabled;
    }

    public long? LTaxonomyChosen => _lTaxonomyVista?.LVistaChosen;

    public bool LTaxonomyFiltered => _lTaxonomyVista?.LVistaFiltered ?? false;

    public string LTaxonomyEmptyRead(string query)
    {
        return string.IsNullOrWhiteSpace(query) ? "Tag.Vacant" : "Tag.Unmatched";
    }

    internal void LTaxonomyVistaRestore(LVista vista, LVista membership)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(membership);

        _lTaxonomyVista = vista;
        _lTaxonomyMembership = membership;
        LTaxonomyPanel.LPanelVistaRestore(membership);
        LTaxonomyEditor.LEditorVistaRestore(membership);
    }

    public void LTaxonomyExplorationSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lTaxonomyVista?.LVistaQuerySet(query);
    }

    public void LTaxonomyFunnelSet(CCatalogOrder? order)
    {
        if (_lTaxonomyVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LPanel.LPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LTaxonomyLatticeSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lTaxonomyVista?.LVistaFilterSet(LPanel.LPanelFilterRead(filter));
    }

    public void LTaxonomyScoutSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lTaxonomyMembership?.LVistaQuerySet(query);
    }

    public void LTaxonomySelect(long? id)
    {
        _lTaxonomyVista?.LVistaSelect(id);
    }

    public IReadOnlyList<CCatalogTag> LTaxonomyRowsRead()
    {
        return _lTaxonomyVista is LVista vista ? LTaxonomyTagRead(_lEntryPort.LEngineTagFind(vista)) : [];
    }

    internal static IReadOnlyList<CCatalogTag> LTaxonomyTagRead(IReadOnlyList<LCatalogTag> rows)
    {
        return LSplice.LSpliceBuild(
            rows,
            static row => new CCatalogTag(
                new CTag(row.LCatalogTagStored.LTagId, row.LCatalogTagStored.LTagText), row.LCatalogTagChosen));
    }

    public IReadOnlyList<CVistaRow> LTaxonomyMembershipRead()
    {
        return LSplice.LSpliceBuild(
            _lEntryPort.LEngineEntryFind(_lTaxonomyVista, _lTaxonomyMembership), LPanel.LPanelRowRead);
    }

    public long LTaxonomyTagCreate(string name)
    {
        return _lEntryPort.LEngineTagCreate(name).LTagId;
    }

    public IReadOnlyList<string> LTaxonomyLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LTaxonomyPortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lTaxonomyMembership, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }

    internal void LTaxonomyVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LTaxonomyVistaRestore(
            atelier.CAtelierVistaStart("taxonomy", CSubject.CSubjectTag, CCatalogOrder.CCatalogOrderName),
            atelier.CAtelierVistaStart(
                "membership", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    public void LTaxonomyObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _lTaxonomyVista?.LVistaObserverAttach(LPanel.LPanelSubjectRead(subject), LTaxonomyBulletinSend);

        void LTaxonomyBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    public CCatalogOrder LTaxonomyOrder =>
        LPanel.LPanelOrderRead(_lTaxonomyVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword);

    public CCatalogFilter LTaxonomyFilter =>
        LPanel.LPanelFilterRead(_lTaxonomyVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);

    public string LTaxonomyFileRead()
    {
        return LVista.LVistaFileRead(_lTaxonomyMembership);
    }

    public Task LTaxonomyPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            _lTaxonomyMembership, path, QPortrait.QPortraitMediumRead(format), QPortrait.QPortraitLabelRead(label));
    }
}
