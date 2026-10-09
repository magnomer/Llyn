using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LEngineHearth
{
    internal object LEngineGate { get; } = new();
    internal LTrove LEngineTrove { get; } = new();
    internal LSettings LEngineSettingsHeld { get; set; }
    internal HashSet<long> LEngineDraftStale { get; } = [];
    internal long LEngineRevision { get; set; }
    private readonly LBulletinRoster _lEngineHearthRoster = new(null);
    private LEngineStaff _lEngineHearthStaff;

    internal LEngineHearth(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        _lEngineHearthStaff = LEngineStaff.LEngineStaffBuild(
            rig, LEngineGate, LEngineBulletinRaise, LEngineSettingsRead, LEngineDraftStale);
        LEngineSettingsHeld = _lEngineHearthStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace
            .LWorkspaceSettingsRead();
    }

    internal LEngineStaff LEngineStaffHeld
    {
        get
        {
            lock (LEngineGate)
            {
                return _lEngineHearthStaff;
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

    internal void LEngineWorkspaceOpen()
    {
        _lEngineHearthStaff.LEngineStaffLanguage.LLanguageStaffVocabulary.LLanguageImport();
        _lEngineHearthStaff.LEngineStaffLanguage.LLanguageStaffApply();
        if (_lEngineHearthStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceClerkMigrated)
        {
            _lEngineHearthStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceClerkUpdate();
        }
    }

    internal void LEngineRigApply(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);

        lock (LEngineGate)
        {
            LSettings settings = LWorkspaceClerk.LWorkspaceSettingsRead(rig, LEngineSettingsHeld, out bool settled);

            foreach (long held in _lEngineHearthStaff.LEngineStaffClaim.LClaimStaffClaim.LClaimClerkHeld)
            {
                LEngineDraftStale.Add(held);
            }

            _lEngineHearthStaff.LEngineStaffLanguage.LLanguageStaffRecording.LRecordingClerkClear();
            _lEngineHearthStaff.LEngineStaffLanguage.LLanguageStaffClear();
            _lEngineHearthStaff.LEngineStaffLanguage.LLanguageStaffEnsign.LEnsignClear();
            LEngineTrove.LTroveClear();

            LEngineSettingsHeld = settings;
            _lEngineHearthStaff = LEngineStaff.LEngineStaffBuild(
                rig, LEngineGate, LEngineBulletinRaise, LEngineSettingsRead, LEngineDraftStale);
            if (!settled)
            {
                _lEngineHearthStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceFallbackSave(
                    LEngineSettingsHeld);
            }

            LEngineWorkspaceOpen();
        }
    }

    internal void LEngineStaffClear()
    {
        lock (LEngineGate)
        {
            _lEngineHearthStaff.LEngineStaffLanguage.LLanguageStaffClear();
        }
    }

    internal void LEngineObserverAttach(Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (LEngineGate)
        {
            if (!_lEngineHearthRoster.LBulletinRosterCheck(observer))
            {
                _lEngineHearthRoster.LBulletinRosterAttach(null, observer, null);
            }
        }
    }

    internal void LEngineObserverDetach(Action<LBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        lock (LEngineGate)
        {
            _lEngineHearthRoster.LBulletinRosterDetach(observer);
        }
    }

    internal void LEngineBulletinRaise(LSubject subject, long id)
    {
        (LSubject?, Action<LBulletin>, long?)[] snapshot;
        lock (LEngineGate)
        {
            LEngineRevision++;
            snapshot = _lEngineHearthRoster.LBulletinRosterRead();
            if (snapshot.Length == 0)
            {
                return;
            }
        }

        _lEngineHearthRoster.LBulletinRosterDispatch(new LBulletin(subject, id), snapshot);
    }
}
