using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LWorkspaceFacade
{
    private readonly LEngineHearth _lWorkspaceFacadeHearth;
    private readonly LDraftFacade _lWorkspaceFacadeDraft;
    private readonly LPronunciationFacade _lWorkspaceFacadePronunciation;
    private readonly object _lWorkspaceFacadeGate;
    private readonly Func<string, LRig> _lWorkspaceFacadeFactory;
    private readonly Action<string> _lWorkspaceFacadePointer;
    private string _lWorkspaceFacadeFolder;
    private LDoctorRescue _lWorkspaceFacadeRescue;

    internal LWorkspaceFacade(
        LEngineHearth hearth,
        LDraftFacade draft,
        LPronunciationFacade pronunciation,
        LRig rig,
        Func<string,
        LRig> factory,
        Action<string> pointer)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(pronunciation);
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(pointer);
        _lWorkspaceFacadeHearth = hearth;
        _lWorkspaceFacadeDraft = draft;
        _lWorkspaceFacadePronunciation = pronunciation;
        _lWorkspaceFacadeGate = _lWorkspaceFacadeHearth.LEngineGate;
        _lWorkspaceFacadeFactory = factory;
        _lWorkspaceFacadePointer = pointer;
        _lWorkspaceFacadeFolder = rig.LRigWorkspace;
        _lWorkspaceFacadeRescue = LWorkspaceClerk.LWorkspaceRescueCreate(rig);
    }

    private LEngineStaff LWorkspaceFacadeStaff => _lWorkspaceFacadeHearth.LEngineStaffHeld;

    public LWorkspaceState LEngineWorkspaceStart()
    {
        _lWorkspaceFacadeDraft.LEngineLeftoverSweep();
        _lWorkspaceFacadePronunciation.LEngineRecordingSweep();
        return LEngineStateRead();
    }

    public LWorkspaceState LEngineWorkspaceChange(string chosen)
    {
        string path = LWorkspaceClerk.LWorkspaceChosenRead(chosen, LEngineWorkspaceRead())
            ?? throw new ArgumentException("The chosen folder names no other workspace.", nameof(chosen));

        LEngineRigApply(_lWorkspaceFacadeFactory(path));
        _lWorkspaceFacadePointer(path);
        return LEngineWorkspaceStart();
    }

    public void LEngineRigApply(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        lock (_lWorkspaceFacadeGate)
        {
            LDoctorRescue rescue = LWorkspaceClerk.LWorkspaceRescueCreate(rig);
            _lWorkspaceFacadeHearth.LEngineRigApply(rig);
            _lWorkspaceFacadeFolder = rig.LRigWorkspace;
            _lWorkspaceFacadeRescue = rescue;
        }

        _lWorkspaceFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);
    }

    public LDoctorRescue LEngineRescueRead()
    {
        lock (_lWorkspaceFacadeGate)
        {
            return _lWorkspaceFacadeRescue;
        }
    }

    public string LEngineWorkspaceRead()
    {
        lock (_lWorkspaceFacadeGate)
        {
            return _lWorkspaceFacadeFolder;
        }
    }

    public string LEngineWorkspaceFormat()
    {
        lock (_lWorkspaceFacadeGate)
        {
            return LWorkspaceFacadeStaff.LEngineStaffLanguage.LLanguageStaffTrail.LWorkspaceFormat();
        }
    }

    public string? LEngineAuditRecord(Exception exception)
    {
        lock (_lWorkspaceFacadeGate)
        {
            return LWorkspaceFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace
                .LWorkspaceAuditRecord(exception);
        }
    }

    public string? LEngineNoticeRead(Exception exception)
    {
        return LWorkspaceClerk.LWorkspaceNoticeRead(exception);
    }

    public (string LEngineFailureNotice, string? LEngineFailureLabel, string? LEngineFailurePath) LEngineFailureRead(
        Exception exception, string unexpected, string recorded)
    {
        if (LEngineNoticeRead(exception) is string notice)
        {
            return (notice, null, null);
        }

        return LEngineAuditRecord(exception) is string path ? (unexpected, recorded, path) : (unexpected, null, null);
    }

    public bool LEngineWorkspaceCheck(string chosen)
    {
        return LWorkspaceClerk.LWorkspaceChosenRead(chosen, LEngineWorkspaceRead()) is not null;
    }

    public LWorkspaceState LEngineStateRead()
    {
        lock (_lWorkspaceFacadeGate)
        {
            return LWorkspaceFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceStateRead();
        }
    }

    public void LEngineLeftSave(long? id)
    {
        LWorkspaceStateChange(state => state with { LWorkspaceStateLeft = id });
    }

    public void LEngineRightSave(long? id)
    {
        LWorkspaceStateChange(state => state with { LWorkspaceStateRight = id });
    }

    private void LWorkspaceStateChange(Func<LWorkspaceState, LWorkspaceState> change)
    {
        lock (_lWorkspaceFacadeGate)
        {
            LWorkspaceState current = LWorkspaceFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace
                .LWorkspaceStateRead();
            LWorkspaceFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceStateSave(change(current));
        }
    }

    public Uri? LEngineLocationRead(string? location)
    {
        lock (_lWorkspaceFacadeGate)
        {
            return LWorkspaceFacadeStaff.LEngineStaffLanguage.LLanguageStaffTrail.LTrailClerkRead(location);
        }
    }

    public (Uri, string?)? LEngineScreenRead(string? location)
    {
        lock (_lWorkspaceFacadeGate)
        {
            return LWorkspaceFacadeStaff.LEngineStaffLanguage.LLanguageStaffTrail.LTrailScreenRead(location);
        }
    }

    public void LEngineLocationOpen(string target)
    {
        LWorkspaceFacadeStaff.LEngineStaffLanguage.LLanguageStaffTrail.LTrailClerkOpen(target);
    }

    public void LEngineFolderOpen()
    {
        LEngineLocationOpen(LEngineWorkspaceRead());
    }
}
