using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LEngine : IDisposable
{
    internal object LEngineGate { get; } = new();
    public LVistaFacade LEngineVista { get; }
    internal LTenureFacade LEngineTenure { get; }
    public LMentionFacade LEngineMention { get; }
    internal LMarkupFacade LEngineMarkup { get; }
    internal LCourierFacade LEngineCourier { get; }
    internal LLiveryFacade LEngineLivery { get; }
    public LWorkspaceFacade LEngineWorkspace { get; }
    public LReflexFacade LEngineReflex { get; }
    internal LPortraitFacade LEnginePortrait { get; }
    public LStemFacade LEngineStem { get; }
    public LFanqieFacade LEngineFanqie { get; }
    public LLanguageFacade LEngineLanguage { get; }
    public LVocabularyFacade LEngineVocabulary { get; }
    public LPronunciationFacade LEnginePronunciation { get; }
    public LDraftFacade LEngineDraft { get; }
    internal LSettingsFacade LEngineSettings { get; }
    internal LRequestFacade LEngineRequest { get; }
    public LAuthorFacade LEngineAuthor { get; }
    public LExampleFacade LEngineExample { get; }
    public LReferenceFacade LEngineReference { get; }
    public LSituationFacade LEngineSituation { get; }
    public LCardFacade LEngineCard { get; }
    public LEntryFacade LEngineEntry { get; }
    internal LTrove LEngineTrove { get; } = new();
    internal LSettings LEngineSettingsHeld { get; set; }
    internal HashSet<long> LEngineDraftStale { get; } = [];
    internal long LEngineRevision { get; set; }
    private readonly LBulletinRoster _lEngineRoster = new(null);
    private LEngineStaff _lEngineStaff;

    public LEngine(LRig rig, Func<string, LRig> factory, Action<string> pointer)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(pointer);

        _lEngineStaff = LEngineStaff.LEngineStaffBuild(
            rig, LEngineGate, LEngineBulletinRaise, LEngineSettingsRead, LEngineDraftStale);
        LEngineSettingsHeld = _lEngineStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceSettingsRead();

        LEngineVista = new LVistaFacade(this);
        LEngineDraft = new LDraftFacade(this);
        LEngineEntry = new LEntryFacade(this);
        LEngineCard = new LCardFacade(this);
        LEngineReference = new LReferenceFacade(this);
        LEngineSituation = new LSituationFacade(this);
        LEngineAuthor = new LAuthorFacade(this);
        LEngineExample = new LExampleFacade(this);
        LEngineSettings = new LSettingsFacade(this);
        LEngineRequest = new LRequestFacade(this);
        LEngineTenure = new LTenureFacade(this);
        LEngineMention = new LMentionFacade(this);
        LEngineMarkup = new LMarkupFacade(this);
        LEngineCourier = new LCourierFacade(this);
        LEngineLivery = new LLiveryFacade(this);
        LEngineWorkspace = new LWorkspaceFacade(this, rig, factory, pointer);
        LEngineReflex = new LReflexFacade(this);
        LEnginePortrait = new LPortraitFacade(this);
        LEngineStem = new LStemFacade(this);
        LEngineFanqie = new LFanqieFacade(this);
        LEngineLanguage = new LLanguageFacade(this);
        LEngineVocabulary = new LVocabularyFacade(this);
        LEnginePronunciation = new LPronunciationFacade(this);
        LEngineWorkspaceOpen();
    }

    internal LEngineStaff LEngineStaffHeld
    {
        get
        {
            lock (LEngineGate)
            {
                return _lEngineStaff;
            }
        }
    }

    internal LSettings LEngineSettingsRead()
    {
        lock (LEngineGate)
        {
            return LEngineSettingsHeld;
        }
    }

    private void LEngineWorkspaceOpen()
    {
        LEngineVocabulary.LEngineLanguageImport();
        _lEngineStaff.LEngineStaffLanguage.LLanguageStaffApply();
        if (_lEngineStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceClerkMigrated)
        {
            _lEngineStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceClerkUpdate();
        }
    }

    internal void LEngineRigApply(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        lock (LEngineGate)
        {
            LSettings settings = LWorkspaceClerk.LWorkspaceSettingsRead(rig, LEngineSettingsHeld, out bool settled);

            foreach (long held in _lEngineStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimClerkHeld)
            {
                LEngineDraftStale.Add(held);
            }

            _lEngineStaff.LEngineStaffLanguage.LLanguageStaffRecording.LRecordingClerkClear();
            _lEngineStaff.LEngineStaffLanguage.LLanguageStaffClear();
            _lEngineStaff.LEngineStaffLanguage.LLanguageStaffEnsign.LEnsignClear();
            LEngineTrove.LTroveClear();

            LEngineSettingsHeld = settings;
            _lEngineStaff = LEngineStaff.LEngineStaffBuild(
                rig, LEngineGate, LEngineBulletinRaise, LEngineSettingsRead, LEngineDraftStale);
            if (!settled)
            {
                _lEngineStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceFallbackSave(
                    LEngineSettingsHeld);
            }

            LEngineWorkspaceOpen();
        }
    }

    public void Dispose()
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffLanguage.LLanguageStaffClear();
        }
    }

    public void LEngineObserverAttach(Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (LEngineGate)
        {
            if (!_lEngineRoster.LBulletinRosterCheck(observer))
            {
                _lEngineRoster.LBulletinRosterAttach(null, observer, null);
            }
        }
    }

    public void LEngineObserverDetach(Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (LEngineGate)
        {
            _lEngineRoster.LBulletinRosterDetach(observer);
        }
    }

    internal void LEngineBulletinRaise(LSubject subject, long id)
    {
        (LSubject?, Action<LBulletin>, long?)[] snapshot;
        lock (LEngineGate)
        {
            LEngineRevision++;
            snapshot = _lEngineRoster.LBulletinRosterRead();
            if (snapshot.Length == 0)
            {
                return;
            }
        }

        _lEngineRoster.LBulletinRosterDispatch(new LBulletin(subject, id), snapshot);
    }
}
