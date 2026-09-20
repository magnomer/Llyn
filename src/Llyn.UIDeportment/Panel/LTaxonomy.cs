using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public LTaxonomy(LEntryPort entries, LPortraitPort portraits, LSettingsPort settings, LEditor editor)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        LTaxonomyEditor = editor;
    }

    public LEditor LTaxonomyEditor { get; }

    public LVista? LTaxonomyMembershipVista => _lTaxonomyMembership;

    public long? LTaxonomyChosen => _lTaxonomyVista?.LVistaChosen;

    public long? LTaxonomyMembershipChosen => _lTaxonomyMembership?.LVistaChosen;

    public bool LTaxonomyFiltered => _lTaxonomyVista?.LVistaFiltered ?? false;

    public void LTaxonomyVistaRestore(LVista vista, LVista membership)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(membership);

        _lTaxonomyVista = vista;
        _lTaxonomyMembership = membership;
        LTaxonomyEditor.LEditorVistaRestore(membership);
    }

    public void LTaxonomyExplorationSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lTaxonomyVista?.LVistaQuerySet(query);
    }

    public void LTaxonomyFunnelSet(LCatalogOrder? order)
    {
        if (_lTaxonomyVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LTaxonomyLatticeSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lTaxonomyVista?.LVistaFilterSet(filter);
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

    public void LTaxonomyMembershipSelect(long? id)
    {
        _lTaxonomyMembership?.LVistaSelect(id);
    }

    public void LTaxonomyScribeSet(bool editing)
    {
        _lTaxonomyMembership?.LVistaEditingSet(editing);
    }

    public IReadOnlyList<LCatalogTag> LTaxonomyRowsRead()
    {
        return _lTaxonomyVista is LVista vista ? _lEntryPort.LEngineTagFind(vista) : [];
    }

    public IReadOnlyList<LVistaRow> LTaxonomyMembershipRead()
    {
        return _lEntryPort.LEngineEntryFind(_lTaxonomyVista, _lTaxonomyMembership);
    }

    public LEntryDraft? LTaxonomyMembershipLoad()
    {
        return _lTaxonomyMembership?.LVistaLoad()?.LDraftContent;
    }

    public void LTaxonomyMembershipDelete()
    {
        _lTaxonomyMembership?.LVistaDelete();
    }

    public LTag LTaxonomyTagCreate(string name)
    {
        return _lEntryPort.LEngineTagCreate(name);
    }

    public IReadOnlyList<string> LTaxonomyLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LTaxonomyPortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lTaxonomyMembership, label, ticket);
    }

    public void LTaxonomyVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LTaxonomyVistaRestore(
            window.LWindowVistaStart("taxonomy", LSubject.LSubjectTag, LCatalogOrder.LCatalogOrderName),
            window.LWindowVistaStart("membership", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public void LTaxonomyObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lTaxonomyVista?.LVistaObserverAttach(subject, observer);
    }

    public void LTaxonomyChosenAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lTaxonomyVista?.LVistaChosenAttach(subject, observer);
    }

    public void LTaxonomyMembershipAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lTaxonomyMembership?.LVistaObserverAttach(subject, observer);
    }

    public void LTaxonomyEntryAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lTaxonomyMembership?.LVistaChosenAttach(subject, observer);
    }

    public LCatalogOrder LTaxonomyOrder => _lTaxonomyVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword;

    public LCatalogFilter LTaxonomyFilter => _lTaxonomyVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;

    public string LTaxonomyFileRead()
    {
        return LVista.LVistaFileRead(_lTaxonomyMembership);
    }

    public Task LTaxonomyPortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(_lTaxonomyMembership, path, format, label);
    }
}
