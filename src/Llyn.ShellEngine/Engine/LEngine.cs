using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LEngine : IDisposable
{
    internal object LEngineGate { get; } = new();
    internal LVistaFacade LEngineVista { get; }
    internal LTenureFacade LEngineTenure { get; }
    internal LMentionFacade LEngineMention { get; }
    internal LMarkupFacade LEngineMarkup { get; }
    internal LWorkspaceFacade LEngineWorkspace { get; }
    internal LReflexFacade LEngineReflex { get; }
    internal LPortraitFacade LEnginePortrait { get; }
    internal LStemFacade LEngineStem { get; }
    internal LFanqieFacade LEngineFanqie { get; }
    internal LLanguageFacade LEngineLanguage { get; }
    internal LVocabularyFacade LEngineVocabulary { get; }
    internal LPronunciationFacade LEnginePronunciation { get; }
    internal LDraftFacade LEngineDraft { get; }
    internal LSettingsFacade LEngineSettings { get; }
    internal LRequestFacade LEngineRequest { get; }
    internal LAuthorFacade LEngineAuthor { get; }
    internal LExampleFacade LEngineExample { get; }
    internal LReferenceFacade LEngineReference { get; }
    internal LSituationFacade LEngineSituation { get; }
    internal LCardFacade LEngineCard { get; }
    internal LEntryFacade LEngineEntry { get; }
    internal LTrove LEngineTrove { get; } = new();
    internal LSettings LEngineSettingsHeld { get; set; }
    internal HashSet<long> LEngineDraftStale { get; } = [];
    private readonly List<Action<LBulletin>> _lEngineObservers = [];
    private LEngineStaff _lEngineStaff;
    private string _lEngineFolder;
    private LDoctorRescue _lEngineRescue;

    public LEngine(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        _lEngineFolder = rig.LRigWorkspace;
        _lEngineStaff = LEngineStaff.LEngineStaffBuild(
            rig, LEngineGate, LEngineBulletinRaise, LEngineSettingsRead, LEngineDraftStale);
        LEngineSettingsHeld = _lEngineStaff.LEngineStaffWorkspace.LWorkspaceSettingsRead();
        _lEngineRescue = LWorkspaceClerk.LWorkspaceRescueCreate(rig);

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
        LEngineWorkspace = new LWorkspaceFacade(this);
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
        _lEngineStaff.LEngineStaffFanqie.LDiweiApply();
        _lEngineStaff.LEngineStaffShengfu.LStemApply();
        if (_lEngineStaff.LEngineStaffWorkspace.LWorkspaceClerkMigrated)
        {
            _lEngineStaff.LEngineStaffWorkspace.LWorkspaceClerkUpdate();
        }
    }

    public LDoctorRescue LEngineRescueRead()
    {
        lock (LEngineGate)
        {
            return _lEngineRescue;
        }
    }

    public string LEngineWorkspaceRead()
    {
        lock (LEngineGate)
        {
            return _lEngineFolder;
        }
    }

    public string LEngineWorkspaceFormat()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTrail.LWorkspaceFormat();
        }
    }

    public string? LEngineAuditRecord(Exception exception)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffWorkspace.LWorkspaceAuditRecord(exception);
        }
    }

    public string? LEngineNoticeRead(Exception exception)
    {
        return LWorkspaceClerk.LWorkspaceNoticeRead(exception);
    }

    public void LEngineRigApply(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        lock (LEngineGate)
        {
            LDoctorRescue rescue = LWorkspaceClerk.LWorkspaceRescueCreate(rig);
            LSettings settings = LWorkspaceClerk.LWorkspaceSettingsRead(rig, LEngineSettingsHeld, out bool settled);

            foreach (long held in _lEngineStaff.LEngineStaffClaim.LClaimClerkHeld)
            {
                LEngineDraftStale.Add(held);
            }

            LEngineFetchClear();
            _lEngineStaff.LEngineStaffEnsign.LEnsignClear();
            LEngineTrove.LTroveClear();

            LEngineSettingsHeld = settings;
            _lEngineRescue = rescue;
            _lEngineFolder = rig.LRigWorkspace;
            _lEngineStaff = LEngineStaff.LEngineStaffBuild(
                rig, LEngineGate, LEngineBulletinRaise, LEngineSettingsRead, LEngineDraftStale);
            if (!settled)
            {
                _lEngineStaff.LEngineStaffWorkspace.LWorkspaceSettingsSave(LEngineSettingsHeld);
            }

            LEngineWorkspaceOpen();
        }

        LEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);
    }

    private void LEngineFetchClear()
    {
        _lEngineStaff.LEngineStaffFrequency.LFrequencyClerkClear();
        _lEngineStaff.LEngineStaffLacuna.LLacunaClerkClear();
        _lEngineStaff.LEngineStaffScript.LScriptClerkClear();
        _lEngineStaff.LEngineStaffFanqie.LFanqieClerkClear();
        _lEngineStaff.LEngineStaffShengfu.LShengfuClerkClear();
        _lEngineStaff.LEngineStaffReflex.LReflexClerkClear();
    }

    internal static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)
    {
        return new ArgumentOutOfRangeException(
            nameof(owner), owner, "This entity has no reference from that kind of row.");
    }

    public void Dispose()
    {
        lock (LEngineGate)
        {
            LEngineFetchClear();
        }
    }

    public void LEngineObserverAttach(Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (LEngineGate)
        {
            if (!_lEngineObservers.Contains(observer))
            {
                _lEngineObservers.Add(observer);
            }
        }
    }

    public void LEngineObserverDetach(Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (LEngineGate)
        {
            _lEngineObservers.Remove(observer);
        }
    }

    internal void LEngineBulletinRaise(LSubject subject, long id)
    {
        Action<LBulletin>[] observers;
        lock (LEngineGate)
        {
            if (_lEngineObservers.Count == 0)
            {
                return;
            }

            observers = [.. _lEngineObservers];
        }

        LBulletin bulletin = new(subject, id);
        foreach (Action<LBulletin> observer in observers)
        {
            observer(bulletin);
        }
    }
}
