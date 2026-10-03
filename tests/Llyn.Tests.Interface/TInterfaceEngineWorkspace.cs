using System;
using System.Net;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEngineWorkspace
{
    internal static LWorkspaceState TEngineWorkspaceStart(this LEngine engine) =>
        new LSettingsOutlet(engine).LEngineWorkspaceStart();

    internal static void TEngineRecordingSweep(this LEngine engine)
    {
        engine.LEnginePronunciation.LEngineRecordingSweep();
    }

    internal static string TEngineWorkspaceRead(this LEngine engine) =>
        engine.LEngineWorkspaceRead();

    internal static void TEngineFolderOpen(this LEngine engine)
    {
        new LSettingsOutlet(engine).LEngineFolderOpen();
    }

    internal static LRig TRigUsherSet(LRig rig, LUsher usher) => rig with { LRigUsher = usher };

    internal static void TEngineWorkspaceOpen(this LEngine engine, string path)
    {
        engine.LEngineRigApply(LRigFactory.LRigFactoryBuild(
            path,
            TPronunciationHelper.TSourceClientCreate(string.Empty, HttpStatusCode.NotFound),
            new LUsherFile(),
            new TPress(),
            new TPhonographFake())
            with { LRigClock = new TClockFake() });
    }

    internal static void TEngineRigApply(this LEngine engine, LRig rig)
    {
        engine.LEngineRigApply(rig);
    }

    internal static LDoctorRescue TEngineRescueRead(this LEngine engine) =>
        engine.LEngineRescueRead();

    internal static LEstablishment TEngineEstablishmentRead(this LEngine engine) =>
        engine.LEngineEntry.LEngineEstablishmentRead();

    internal static LEstablishment TEstablishmentCreate(int unsaved, long entry, long size) =>
        new(unsaved, entry, size);

    internal static LWorkspaceState TWorkspaceStateCreate() => new(1);

    internal static void TEngineObserverAttach(this LEngine engine, Action<LBulletin> observer)
    {
        engine.LEngineObserverAttach(observer);
    }

    internal static void TEngineObserverDetach(this LEngine engine, Action<LBulletin> observer)
    {
        engine.LEngineObserverDetach(observer);
    }

    internal static Uri? TEngineLocationResolve(this LEngine engine, string location) =>
        engine.LEngineStaffHeld.LEngineStaffTrail.LTrailClerkResolve(location);

    internal static Uri? TEngineLocationRead(this LEngine engine, string location) =>
        engine.LEngineWorkspace.LEngineLocationRead(location);

    internal static string TEngineWorkspaceFormat(this LEngine engine) =>
        engine.LEngineWorkspaceFormat();

    internal static long? TEngineRevisionRead(this LEngine engine) =>
        engine.LEngineWorkspace.LEngineStateRead().LWorkspaceStateRevision;

    internal static LWorkspaceState TEngineStateRead(this LEngine engine) =>
        engine.LEngineWorkspace.LEngineStateRead();
}
