using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CEditor
{
    private LVista? _cEditorVista;

    private bool _cEditorFresh;

    internal CEditor(
        LDraftPort drafts,
        LEntryPort entries,
        LPhonologyPort phonology,
        LSettingsPort settings,
        LMediaPort media,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(media);

        CEditorDesk = new CDesk(drafts, "Input", envoy);
        CEditorDisplay = new LDisplay(drafts, entries, phonology, settings, media, envoy);
        CEditorCard = new CCard(CEditorDesk, drafts, entries, settings, envoy);
        CEditorSentence = new CSentence(CEditorDesk, phonology, drafts, settings, envoy);
        CEditorSounding = new CSounding(
            CEditorDesk, phonology, drafts, settings, CEditorDisplay.LDisplaySound, envoy);
        CEditorEsteem = new CEsteem(CEditorDesk, CEditorDisplay);
        CEditorTimbre = new CTimbre(CEditorDesk, phonology, CEditorDisplay, media);
        CEditorSpeech = new CCardSpeech(CEditorDesk);
        CEditorDesk.CDeskFinished += CEditorStoredShow;
        CEditorDesk.CDeskDraftPrepared += draft =>
        {
            if (CEditorDesk.CDeskTenure is LTenure held)
            {
                CEditorDraftChanged?.Invoke(CFolio.CFolioEntryRead(
                    draft.LDraftContent,
                    held.LTenureTranslationRead(draft.LDraftContent),
                    CEditorDisplay.LDisplayMediaPort));
            }
        };
    }

    public static CEditor CEditorCreate(CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        CEditor editor = new(
            atelier.CAtelierDraftPort,
            atelier.CAtelierEntryPort,
            atelier.CAtelierPhonologyPort,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierMediaPort,
            envoy);
        editor.CEditorDisplay.LDisplayNavigationAttach(atelier.CAtelierNavigation, atelier.CAtelierMention);
        editor.CEditorSounding.LSoundingDiweiChosen +=
            (language, kind, key) => atelier.CAtelierNavigation.LNavigationDiweiOpen(language, kind, key);
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => editor.CEditorEntryOpen(null);
        return editor;
    }

    public event Action<CEntryDraft>? CEditorDraftChanged;

    public CDesk CEditorDesk { get; }

    public LDisplay CEditorDisplay { get; }

    public CCard CEditorCard { get; }

    public CSentence CEditorSentence { get; }

    public CSounding CEditorSounding { get; }

    public CEsteem CEditorEsteem { get; }

    public CTimbre CEditorTimbre { get; }

    public CCardSpeech CEditorSpeech { get; }

    public CCardField CEditorField => new(CEditorDesk);

    public CCardList CEditorList => new(CEditorDesk);

    public CImage CEditorImage => new(CEditorDesk);

    public CVideo CEditorVideo => new(CEditorDesk);

    public bool CEditorOwned => _cEditorVista?.LVistaInput ?? false;

    public string CEditorLanguage => CEditorDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    public long? CEditorEntry => CEditorDesk.CDeskStoredRead();

    private LTenure? CEditorTenure => CEditorDesk.CDeskFilling ? null : CEditorDesk.CDeskTenure;

    public void CEditorObserverAttach(Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(marshal);

        CEditorDesk.CDeskObserverAttach(marshal);
        CEditorEsteem.LEsteemObserverAttach(marshal);
        CEditorTimbre.LTimbreObserverAttach(marshal);
        CEditorSounding.LSoundingObserverAttach(marshal);
        CEditorSentence.LSentenceObserverAttach(marshal);
        CEditorDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectSettings, _ => marshal(CEditorDesk.CDeskDraftResonate));
    }

    internal void LEditorVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _cEditorVista = vista;
        CEditorDesk.CDeskVistaRestore(vista);
        CEditorDisplay.LDisplayVistaRestore(vista);
    }

    public void CEditorEntryOpen(long? id)
    {
        CEditorDesk.CDeskStart(id);
        if (id is not null && !CEditorDesk.CDeskHeld)
        {
            CEditorDesk.CDeskStart(null);
        }
    }

    public void CEditorClose()
    {
        CEditorDesk.CDeskCancel();
        CEditorDesk.CDeskErrand.CErrandCancel();
    }

    public void CEditorEntryUndo()
    {
        CEditorEntryOpen(CEditorDesk.CDeskStoredRead());
    }

    public void CEditorEntrySave()
    {
        if (!CEditorDesk.LDeskChangeCheck())
        {
            return;
        }

        LEditorFinish(true);
    }

    internal bool LEditorFinish(bool store)
    {
        _cEditorFresh = CEditorDesk.CDeskStoredRead() is null;
        return CEditorDesk.CDeskFinish(store);
    }

    private void CEditorStoredShow(long id)
    {
        if (_cEditorFresh && CEditorOwned)
        {
            CEditorEntryOpen(null);
            return;
        }

        CEditorEntryOpen(id);
    }

    public CEntryDraft? CEditorDraftRead()
    {
        return CEditorDesk.CDeskTenure is LTenure held && held.LTenureRead() is { } draft
            ? CFolio.CFolioEntryRead(
                draft.LDraftContent, held.LTenureTranslationRead(draft.LDraftContent), CEditorDisplay.LDisplayMediaPort)
            : null;
    }

    public IReadOnlyList<CReflexHead> CEditorLeadRead(long reflex, string language)
    {
        IReadOnlyList<CReflexDraft> rows = CEditorDraftRead()?.CEntryDraftReflexes ?? [];
        IReadOnlyList<bool> leads = CReflex.LReflexLeadRead(rows
            .Select(row => row.CReflexDraftId == reflex ? language : row.CReflexDraftLanguage)
            .ToList());
        return rows.Select((row, index) => new CReflexHead(row.CReflexDraftId, leads[index])).ToList();
    }

    public string CEditorPronunciationRead()
    {
        return CEditorDesk.CDeskTenure?.LTenurePronunciationRead() ?? string.Empty;
    }

    public IReadOnlyList<CTranslationTarget> CEditorEtymonRead()
    {
        return CEditorDesk.CDeskTenure is LTenure held ? CFolio.CFolioTargetRead(held.LTenureEtymonRead()) : [];
    }

    public void CEditorHeadwordSet(string text)
    {
        CEditorTenure?.LTenureHeadwordSet(text);
    }

    public void CEditorPronunciationSet(string text)
    {
        CEditorTenure?.LTenurePronunciationSet(text);
    }

    public void CEditorNoteSet(string text)
    {
        CEditorTenure?.LTenureNoteSet(text);
    }

    public static bool CEditorNoteCheck(string text, string note) => LTenure.LTenureNoteCheck(text, note);

    public void CEditorLanguageSet(string language)
    {
        CEditorTenure?.LTenureLanguageSet(language);
    }

    public void CEditorVarietySet(bool primary, long pronunciation, string variety)
    {
        CEditorTenure?.LTenureVarietySet(primary, pronunciation, variety);
    }
}
