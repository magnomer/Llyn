using System;
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
        CLedgerNoticed noticed,
        Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(marshal);

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
        CEditorTimbre = new CTimbre(CEditorDesk, phonology.CPhonologyBundleLanguage, settings, envoy);
        CEditorKindred = new CKindred(
            CEditorDesk, phonology.CPhonologyBundleReflex, CEditorDisplay.LDisplayRule, drafts, settings, envoy);
        CEditorPlayback = new CPlayback(CEditorDesk, media);
        CEditorSpeech = new CCardSpeech(CEditorDesk, entries.CEntryBundleEntry);
        CEditorDesk.CDeskFinished += CEditorStoredShow;
        CEditorEntry = new CEntry(CEditorDesk, drafts, media);
        CEditorDesk.CDeskObserverAttach(marshal);
        CEditorEsteem.LEsteemObserverAttach(marshal);
        CEditorTimbre.LTimbreObserverAttach(marshal);
        CEditorKindred.LKindredObserverAttach(marshal);
        CEditorSounding.LSoundingObserverAttach(marshal);
        CEditorFold.LFoldObserverAttach(marshal);
        CEditorSentence.LSentenceObserverAttach(marshal);
        CEditorDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectSettings, _ => marshal(CEditorDesk.CDeskDraft.CDeskDraftResonate));
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
            atelier.CAtelierLedger.LLedgerRepaint,
            atelier.CAtelierMarshal);
        editor.CEditorDisplay.LDisplayNavigationAttach(atelier.CAtelierNavigation, atelier.CAtelierMention);
        editor.CEditorSounding.LSoundingDiweiChosen +=
            (language, kind, key) => atelier.CAtelierNavigation.LNavigationDiweiOpen(language, kind, key);
        atelier.CAtelierWorkspace.CWorkspaceOpened += () => editor.CEditorEntryOpen(null);
        return editor;
    }

    public CDesk CEditorDesk { get; }

    public CEntry CEditorEntry { get; }

    public CDisplay CEditorDisplay { get; }

    public CCard CEditorCard { get; }

    public CSentence CEditorSentence { get; }

    public CSounding CEditorSounding { get; }

    public CFold CEditorFold { get; }

    public CEsteem CEditorEsteem { get; }

    public CTimbre CEditorTimbre { get; }

    public CKindred CEditorKindred { get; }

    public CPlayback CEditorPlayback { get; }

    public CTranscription CEditorTranscription => new(CEditorDesk);

    public CCardSpeech CEditorSpeech { get; }

    public CCardField CEditorField => new(CEditorDesk);

    public CCardList CEditorList => new(CEditorDesk);

    public CImage CEditorImage => new(CEditorDesk);

    public CVideo CEditorVideo => new(CEditorDesk);

    public bool CEditorOwned => _cEditorVista?.LVistaInput ?? false;

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
}
