using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LCorpus
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lCorpusVista;

    private LVista? _lCorpusQuotation;

    public LCorpus(
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
        LCorpusEditor = editor;
        LCorpusDesk = new LDesk(drafts, "Example", unreadableSeam);
    }

    public LEditor LCorpusEditor { get; }

    public LDesk LCorpusDesk { get; }

    public void LCorpusStart(long? id)
    {
        LCorpusDesk.LDeskStart("Corpus", LSubject.LSubjectExample, id);
    }

    public LVista? LCorpusQuotationVista => _lCorpusQuotation;

    public long? LCorpusChosen => _lCorpusVista?.LVistaChosen;

    public long? LCorpusQuotationChosen => _lCorpusQuotation?.LVistaChosen;

    public bool LCorpusFiltered => _lCorpusVista?.LVistaFiltered ?? false;

    public void LCorpusVistaRestore(LVista vista, LVista quotation)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(quotation);

        _lCorpusVista = vista;
        _lCorpusQuotation = quotation;
        LCorpusEditor.LEditorVistaRestore(quotation);
    }

    public void LCorpusQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lCorpusVista?.LVistaQuerySet(query);
    }

    public void LCorpusRankSet(LCatalogOrder? order)
    {
        if (_lCorpusVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LCorpusGauzeSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lCorpusVista?.LVistaFilterSet(filter);
    }

    public void LCorpusDredgeSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lCorpusQuotation?.LVistaQuerySet(query);
    }

    public void LCorpusSelect(long? id)
    {
        _lCorpusVista?.LVistaSelect(id);
    }

    public void LCorpusQuotationSelect(long? id)
    {
        _lCorpusQuotation?.LVistaSelect(id);
    }

    public void LCorpusTranscriptSet(bool editing)
    {
        _lCorpusVista?.LVistaEditingSet(editing);
    }

    public void LCorpusEditorSet(bool editing)
    {
        _lCorpusQuotation?.LVistaEditingSet(editing);
    }

    public IReadOnlyList<LCatalogExample> LCorpusRowsRead(string unknown, string unwritten)
    {
        return _lCorpusVista is LVista vista ? _lEntryPort.LEngineExampleFind(vista, unknown, unwritten) : [];
    }

    public IReadOnlyDictionary<long, int> LCorpusUsageRead()
    {
        return _lEntryPort.LEngineUsageRead(LOwner.LOwnerExample);
    }

    public int LCorpusUsageRead(long? id)
    {
        return id is long stored ? LCorpusUsageRead().GetValueOrDefault(stored) : 0;
    }

    public IReadOnlyList<LVistaRow> LCorpusQuotationRead()
    {
        return _lEntryPort.LEngineEntryFind(_lCorpusVista, _lCorpusQuotation);
    }

    public LExample? LCorpusLoad()
    {
        return _lCorpusVista?.LVistaLoad()?.LDraftExample;
    }

    public LEntryDraft? LCorpusQuotationLoad()
    {
        return _lCorpusQuotation?.LVistaLoad()?.LDraftContent;
    }

    public void LCorpusDelete()
    {
        _lCorpusVista?.LVistaDelete();
    }

    public IReadOnlyList<LCatalogReference> LCorpusReferenceFind()
    {
        return _lEntryPort.LEngineReferenceFind(string.Empty, LCatalogOrder.LCatalogOrderAuthor);
    }

    public IReadOnlyList<LCatalogReference> LCorpusReferenceFind(string word)
    {
        return _lEntryPort.LEngineReferenceFind(word, LCatalogOrder.LCatalogOrderUsage);
    }

    public void LCorpusCitationSet(string title)
    {
        LCorpusDesk.LDeskSend(new LRequestExampleReference(
            LCorpusDesk.LDeskId, _lEntryPort.LEngineCitationResolve(LCorpusDesk.LDeskId, 0, 0, title)));
    }

    public LMentionResult LCorpusMentionFind(long id, int offset)
    {
        return _lEntryPort.LEngineMentionFind(id, offset);
    }

    public IReadOnlyList<string> LCorpusLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LCorpusPortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lCorpusQuotation, label, ticket);
    }

    public Task LCorpusPortraitPrint(LPortraitLegend legend, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lCorpusVista, legend, ticket);
    }

    public void LCorpusVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LCorpusVistaRestore(
            window.LWindowVistaStart("corpus", LSubject.LSubjectExample, LCatalogOrder.LCatalogOrderText),
            window.LWindowVistaStart("quotation", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public void LCorpusObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lCorpusVista?.LVistaObserverAttach(subject, observer);
    }

    public void LCorpusChosenAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lCorpusVista?.LVistaChosenAttach(subject, observer);
    }

    public void LCorpusQuotationAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lCorpusQuotation?.LVistaObserverAttach(subject, observer);
    }

    public void LCorpusEntryAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lCorpusQuotation?.LVistaChosenAttach(subject, observer);
    }

    public LCatalogOrder LCorpusOrder => _lCorpusVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword;

    public LCatalogFilter LCorpusFilter => _lCorpusVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;

    public string LCorpusFileRead()
    {
        return LVista.LVistaFileRead(_lCorpusQuotation);
    }

    public Task LCorpusPortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(_lCorpusQuotation, path, format, label);
    }
}
