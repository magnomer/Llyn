using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LRepertoire
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lRepertoireVista;

    private LVista? _lRepertoireOccurrence;

    public LRepertoire(
        LDraftPort drafts,
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        LRepertoireEditor = editor;
        LRepertoireDesk = new LDesk(drafts, "Situation", unreadableSeam);
    }

    public LEditor LRepertoireEditor { get; }

    public LDesk LRepertoireDesk { get; }

    public void LRepertoireStart(long? id)
    {
        LRepertoireDesk.LDeskStart("Repertoire", LSubject.LSubjectSituation, id);
    }

    public LVista? LRepertoireOccurrenceVista => _lRepertoireOccurrence;

    public long? LRepertoireChosen => _lRepertoireVista?.LVistaChosen;

    public long? LRepertoireOccurrenceChosen => _lRepertoireOccurrence?.LVistaChosen;

    public bool LRepertoireFiltered => _lRepertoireVista?.LVistaFiltered ?? false;

    public void LRepertoireVistaRestore(LVista vista, LVista occurrence)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(occurrence);

        _lRepertoireVista = vista;
        _lRepertoireOccurrence = occurrence;
        LRepertoireEditor.LEditorVistaRestore(occurrence);
    }

    public void LRepertoireInquestSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lRepertoireVista?.LVistaQuerySet(query);
    }

    public void LRepertoireTierSet(LCatalogOrder? order)
    {
        if (_lRepertoireVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LRepertoireMeshSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lRepertoireVista?.LVistaFilterSet(filter);
    }

    public void LRepertoireSortieSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lRepertoireOccurrence?.LVistaQuerySet(query);
    }

    public void LRepertoireSelect(long? id)
    {
        _lRepertoireVista?.LVistaSelect(id);
    }

    public void LRepertoireOccurrenceSelect(long? id)
    {
        _lRepertoireOccurrence?.LVistaSelect(id);
    }

    public void LRepertoireScenarioSet(bool editing)
    {
        _lRepertoireVista?.LVistaEditingSet(editing);
    }

    public void LRepertoireEditorSet(bool editing)
    {
        _lRepertoireOccurrence?.LVistaEditingSet(editing);
    }

    public IReadOnlyList<LCatalogSituation> LRepertoireRowsRead(string unknown, string untitled)
    {
        return _lRepertoireVista is LVista vista ? _lEntryPort.LEngineSituationFind(vista, unknown, untitled) : [];
    }

    public IReadOnlyDictionary<long, int> LRepertoireUsageRead()
    {
        return _lEntryPort.LEngineUsageRead(LOwner.LOwnerSituation);
    }

    public int LRepertoireUsageRead(long? id)
    {
        return id is long stored ? LRepertoireUsageRead().GetValueOrDefault(stored) : 0;
    }

    public IReadOnlyList<LVistaRow> LRepertoireOccurrenceRead()
    {
        return _lEntryPort.LEngineEntryFind(_lRepertoireVista, _lRepertoireOccurrence);
    }

    public LSituation? LRepertoireLoad()
    {
        return _lRepertoireVista?.LVistaLoad()?.LDraftSituation;
    }

    public LEntryDraft? LRepertoireOccurrenceLoad()
    {
        return _lRepertoireOccurrence?.LVistaLoad()?.LDraftContent;
    }

    public void LRepertoireDelete()
    {
        _lRepertoireVista?.LVistaDelete();
    }

    public IReadOnlyList<string> LRepertoireLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LRepertoirePortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lRepertoireOccurrence, label, ticket);
    }

    public Task LRepertoirePortraitPrint(LPortraitLegend legend, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lRepertoireVista, legend, ticket);
    }

    public void LRepertoireVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LRepertoireVistaRestore(
            window.LWindowVistaStart("repertoire", LSubject.LSubjectSituation, LCatalogOrder.LCatalogOrderName),
            window.LWindowVistaStart("occurrence", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public void LRepertoireObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lRepertoireVista?.LVistaObserverAttach(subject, observer);
    }

    public void LRepertoireChosenAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lRepertoireVista?.LVistaChosenAttach(subject, observer);
    }

    public void LRepertoireOccurrenceAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lRepertoireOccurrence?.LVistaObserverAttach(subject, observer);
    }

    public void LRepertoireEntryAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lRepertoireOccurrence?.LVistaChosenAttach(subject, observer);
    }

    public LCatalogOrder LRepertoireOrder => _lRepertoireVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword;

    public LCatalogFilter LRepertoireFilter => _lRepertoireVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;

    public string LRepertoireFileRead()
    {
        return LVista.LVistaFileRead(_lRepertoireOccurrence);
    }

    public Task LRepertoirePortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(_lRepertoireOccurrence, path, format, label);
    }
}
