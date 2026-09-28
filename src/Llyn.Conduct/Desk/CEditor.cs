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
        CEditorDisplay = new LDisplay(drafts, entries, phonology, settings, media);
        CEditorCard = new CCard(CEditorDesk, drafts, entries);
        CEditorSentence = new CSentence(CEditorDesk, phonology);
        CEditorSounding = new CSounding(CEditorDesk, phonology, drafts, envoy);
        CEditorEsteem = new CEsteem(CEditorDesk, CEditorDisplay);
        CEditorTimbre = new CTimbre(CEditorDesk, phonology, CEditorDisplay);
        CEditorDesk.CDeskFinished += CEditorStoredShow;
        CEditorDesk.CDeskDraftPrepared +=
            draft => CEditorDraftChanged?.Invoke(CFolio.CFolioEntryRead(draft.LDraftContent));
    }

    public static CEditor CEditorCreate(CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        return new CEditor(
            atelier.CAtelierDraftPort,
            atelier.CAtelierEntryPort,
            atelier.CAtelierPhonologyPort,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierMediaPort,
            envoy);
    }

    public event Action<CEntryDraft>? CEditorDraftChanged;

    public CDesk CEditorDesk { get; }

    public LDisplay CEditorDisplay { get; }

    public CCard CEditorCard { get; }

    public CSentence CEditorSentence { get; }

    public CSounding CEditorSounding { get; }

    public CEsteem CEditorEsteem { get; }

    public CTimbre CEditorTimbre { get; }

    public bool CEditorOwned => _cEditorVista?.LVistaInput ?? false;

    public string CEditorOrigin => _cEditorVista?.LVistaTab ?? string.Empty;

    private LTenure? CEditorTenure => CEditorDesk.CDeskFilling ? null : CEditorDesk.CDeskTenure;

    internal void CEditorVistaRestore(LVista vista)
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

    public void CEditorEntryUndo()
    {
        CEditorEntryOpen(CEditorDesk.CDeskStoredRead());
    }

    public void CEditorEntrySave()
    {
        if (!CEditorDesk.CDeskChangeCheck())
        {
            return;
        }

        CEditorFinish(true);
    }

    public bool CEditorFinish(bool store)
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
        return CEditorDesk.CDeskDraft is { } held ? CFolio.CFolioEntryRead(held.LDraftContent) : null;
    }

    public string CEditorPronunciationRead()
    {
        return CEditorDesk.CDeskTenure?.LTenurePronunciationRead() ?? string.Empty;
    }

    public IReadOnlyList<CTranslationTarget> CEditorEtymonRead()
    {
        return CEditorDesk.CDeskTenure is LTenure held ? CFolio.CFolioTargetRead(held.LTenureEtymonRead()) : [];
    }

    public IReadOnlyDictionary<long, CTranslationTarget> CEditorTargetRead()
    {
        if (CEditorDesk.CDeskTenure is not LTenure held)
        {
            return new Dictionary<long, CTranslationTarget>();
        }

        return held.LTenureTargetRead()
            .ToDictionary(pair => pair.Key, pair => CFolio.CFolioTargetRead([pair.Value])[0]);
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

    public void CEditorLanguageSet(string language)
    {
        CEditorTenure?.LTenureLanguageSet(language);
    }

    public void CEditorVarietySet(bool primary, long pronunciation, string variety)
    {
        long target = primary
            ? CEditorDraftRead()?.CEntryDraftPronunciation?.CPronunciationDraftId ?? 0
            : pronunciation;
        CEditorTenure?.LTenureVarietySet(target, variety);
    }
}
