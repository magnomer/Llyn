using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine : IDisposable
{
    internal object LEngineGate { get; } = new();
    internal LTrove LEngineTrove { get; } = new();
    internal LSettings LEngineSettingsHeld { get; set; }
    internal HashSet<long> LEngineDraftStale { get; } = [];
    private readonly List<Action<LBulletin>> _lEngineObservers = [];
    private LEngineStaff _lEngineStaff;
    private string _lEngineWorkspace;
    private LDoctorRescue _lEngineRescue;
    private LRealm _lEngineRealm;

    public LEngine(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        _lEngineWorkspace = rig.LRigWorkspace;
        _lEngineStaff = LEngineStaff.LEngineStaffBuild(
            rig, LEngineGate, LEngineBulletinRaise, LEngineSettingsRead);
        LEngineSettingsHeld = _lEngineStaff.LEngineStaffWorkspace.LWorkspaceSettingsRead();
        _lEngineRescue = LWorkspaceClerk.LWorkspaceRescueCreate(rig);
        _lEngineRealm = LWorkspaceClerk.LWorkspaceRealmRead(rig);
        LEngineWorkspaceOpen();
    }

    internal LEngineStaff LEngineStaffHeld => _lEngineStaff;

    private void LEngineWorkspaceOpen()
    {
        LEngineLanguageImport();
        _lEngineStaff.LEngineStaffFanqie.LDiweiApply();
        _lEngineStaff.LEngineStaffShengfu.LStemApply();
        if (_lEngineStaff.LEngineStaffWorkspace.LWorkspaceClerkMigrated)
        {
            _lEngineStaff.LEngineStaffWorkspace.LWorkspaceClerkUpdate();
        }
    }

    internal long LEngineIdentityCreate()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffIdentity.LIdentityCreate();
        }
    }

    internal LRealm LEngineRealmRead()
    {
        lock (LEngineGate)
        {
            return _lEngineRealm;
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
            return _lEngineWorkspace;
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
            LRealm realm = LWorkspaceClerk.LWorkspaceRealmRead(rig);
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
            _lEngineRealm = realm;
            _lEngineWorkspace = rig.LRigWorkspace;
            _lEngineStaff = LEngineStaff.LEngineStaffBuild(
                rig, LEngineGate, LEngineBulletinRaise, LEngineSettingsRead);
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

    internal static bool LEngineOwnerCheck(LOwner owner)
    {
        return LCardClerk.LCardOwnerCheck(owner);
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
