using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CEditor
{
    private LVista? _cEditorVista;

    private bool _cEditorFresh;

    internal CEditor(
        LDraftPort drafts,
        CEntryBundle entries,
        CPhonologyBundle phonology,
        LSettingsPort settings,
        LMediaPort media,
        CEnvoy envoy,
        CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(media);

        CEditorDesk = new CDesk(drafts, settings, "Input", envoy);
        CEditorDisplay = new CDisplay(drafts, entries, phonology, settings, media, envoy, noticed);
        CEditorCard = new CCard(CEditorDesk, drafts, entries.CEntryBundleReference, settings, envoy);
        CEditorSentence = new CSentence(
            CEditorDesk, phonology.CPhonologyBundleSentence, drafts, settings, envoy, noticed);
        CEditorSounding = new CSounding(
            CEditorDesk,
            phonology.CPhonologyBundleFanqie,
            phonology.CPhonologyBundleDiwei,
            phonology.CPhonologyBundleScript,
            phonology.CPhonologyBundleParadigm,
            settings,
            CEditorDisplay.LDisplayRule,
            envoy);
        CEditorFold = new CFold(settings, envoy);
        CEditorEsteem = new CEsteem(CEditorDesk, CEditorDisplay.LDisplayRule);
        CEditorTimbre = new CTimbre(
            CEditorDesk,
            phonology.CPhonologyBundleLanguage,
            phonology.CPhonologyBundleReflex,
            CEditorDisplay.LDisplayRule,
            drafts,
            settings,
            envoy);
        CEditorPlayback = new CPlayback(CEditorDesk, media);
        CEditorSpeech = new CCardSpeech(CEditorDesk, entries.CEntryBundleEntry);
        CEditorDesk.CDeskFinished += CEditorStoredShow;
        CEditorDesk.CDeskDraftPrepared += draft =>
        {
            if (CEditorDesk.CDeskTenure is LTenure held)
            {
                CEditorDraftChanged?.Invoke(CFolio.CFolioEntryRead(
                    draft.LDraftContent,
                    new LQuillChip(held, drafts).LQuillTranslationRead(draft.LDraftContent),
                    CEditorDisplay.LDisplayRule.LDisplayMediaPort));
            }
        };
    }

    public static CEditor CEditorCreate(CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        CEditor editor = new(
            atelier.CAtelierDraftPort,
            atelier.CAtelierEntryBundle,
            atelier.CAtelierPhonologyBundle,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierMediaPort,
            envoy,
            atelier.CAtelierLedger.LLedgerRepaint);
        editor.CEditorDisplay.LDisplayNavigationAttach(atelier.CAtelierNavigation, atelier.CAtelierMention);
        editor.CEditorSounding.LSoundingDiweiChosen +=
            (language, kind, key) => atelier.CAtelierNavigation.LNavigationDiweiOpen(language, kind, key);
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => editor.CEditorEntryOpen(null);
        return editor;
    }

    public event Action<CEntryDraft>? CEditorDraftChanged;

    public CDesk CEditorDesk { get; }

    public CDisplay CEditorDisplay { get; }

    public CCard CEditorCard { get; }

    public CSentence CEditorSentence { get; }

    public CSounding CEditorSounding { get; }

    public CFold CEditorFold { get; }

    public CEsteem CEditorEsteem { get; }

    public CTimbre CEditorTimbre { get; }

    public CPlayback CEditorPlayback { get; }

    public CTranscription CEditorTranscription => new(CEditorDesk);

    public CCardSpeech CEditorSpeech { get; }

    public CCardField CEditorField => new(CEditorDesk);

    public CCardList CEditorList => new(CEditorDesk);

    public CImage CEditorImage => new(CEditorDesk);

    public CVideo CEditorVideo => new(CEditorDesk);

    public bool CEditorOwned => _cEditorVista?.LVistaInput ?? false;

    public string CEditorLanguage => CEditorDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    private LTenure? CEditorTenure => CEditorDesk.CDeskFilling ? null : CEditorDesk.CDeskTenure;

    public void CEditorObserverAttach(Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(marshal);

        CEditorDesk.CDeskObserverAttach(marshal);
        CEditorEsteem.LEsteemObserverAttach(marshal);
        CEditorTimbre.LTimbreObserverAttach(marshal);
        CEditorSounding.LSoundingObserverAttach(marshal);
        CEditorFold.LFoldObserverAttach(marshal);
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
        CEditorFold.LFoldAttach();
        CEditorDesk.CDeskStart(id);
        if (id is not null && !CEditorDesk.CDeskHeld)
        {
            CEditorDesk.CDeskStart(null);
        }
    }

    public void CEditorClose()
    {
        CEditorFold.LFoldDetach();
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

    public string CEditorPronunciationRead()
    {
        return CEditorDesk.CDeskTenure is LTenure held
            ? new LQuillPronunciation(held).LQuillPronunciationRead()
            : string.Empty;
    }

    public IReadOnlyList<CTranslationTarget> CEditorEtymonRead()
    {
        return CEditorDesk.CDeskTenure is LTenure held
            ? CFolio.CFolioTargetRead(new LQuillEtymology(held).LQuillEtymonRead())
            : [];
    }

    public void CEditorHeadwordSet(string text)
    {
        if (CEditorTenure is LTenure held)
        {
            new LQuillEntry(held).LQuillHeadwordSet(text);
        }
    }

    public void CEditorPronunciationSet(string text)
    {
        if (CEditorTenure is LTenure held)
        {
            new LQuillPronunciation(held).LQuillPronunciationSet(text);
        }
    }

    public void CEditorNoteSet(string text)
    {
        if (CEditorTenure is LTenure held)
        {
            new LQuillEntry(held).LQuillNoteSet(text);
        }
    }

    public static bool CEditorNoteCheck(string text, string note) => LQuillEntry.LQuillNoteCheck(text, note);

    public void CEditorLanguageSet(string language)
    {
        if (CEditorTenure is LTenure held)
        {
            new LQuillEntry(held).LQuillLanguageSet(language);
        }
    }

    public void CEditorUnitSet(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (CEditorTenure is LTenure held)
        {
            new LQuillEntry(held).LQuillUnitSet(CCardSpeech.LUnitRowParse(key));
        }
    }
}
