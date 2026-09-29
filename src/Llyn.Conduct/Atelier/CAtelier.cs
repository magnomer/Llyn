using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CAtelier : IDisposable
{
    internal CAtelier(
        LPosture posture,
        LDraftPort drafts,
        LEntryPort entries,
        LSettingsPort settings,
        LPhonologyPort phonology,
        LMediaPort media,
        LPortraitPort portraits)
    {
        ArgumentNullException.ThrowIfNull(posture);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(portraits);

        CAtelierPosture = posture;
        CAtelierDraftPort = drafts;
        CAtelierEntryPort = entries;
        CAtelierSettingsPort = settings;
        CAtelierPhonologyPort = phonology;
        CAtelierMediaPort = media;
        CAtelierPortraitPort = portraits;
        CAtelierMention = new CMention(this);
        CAtelierNavigation = new CNavigation(this);
        CAtelierLedger = new CLedger(this);
        CAtelierWorkspace = new CWorkspace(this);
    }

    public CMention CAtelierMention { get; }

    public CMarkdown CAtelierMarkdown => new(this);

    public CRespelling CAtelierRespelling => new(this);

    public CNavigation CAtelierNavigation { get; }

    public CCatalog CAtelierCatalog => new(this);

    public CLedger CAtelierLedger { get; }

    public CWorkspace CAtelierWorkspace { get; }

    internal LPosture CAtelierPosture { get; }

    internal LDraftPort CAtelierDraftPort { get; }

    internal LEntryPort CAtelierEntryPort { get; }

    internal LSettingsPort CAtelierSettingsPort { get; }

    internal LPhonologyPort CAtelierPhonologyPort { get; }

    internal LMediaPort CAtelierMediaPort { get; }

    internal LPortraitPort CAtelierPortraitPort { get; }

    internal LVista CAtelierVistaStart(string tab, CSubject? subject, CCatalogOrder fallback, bool blank = false)
    {
        return CAtelierPosture.LPostureVistaStart(
            tab,
            subject is CSubject held ? CPanel.CPanelSubjectRead(held) : null,
            CPanel.LPanelOrderRead(fallback),
            blank);
    }

    public CEditor CAtelierInputCreate(CEnvoy envoy)
    {
        CEditor editor = CEditor.CEditorCreate(this, envoy);
        LAtelierInputRestore(editor);
        return editor;
    }

    internal void LAtelierInputRestore(CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        CAtelierWorkspace.LWorkspaceInputSet(editor);
        editor.LEditorVistaRestore(
            CAtelierVistaStart("input", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    internal bool LAtelierSplitRead()
    {
        return CAtelierPosture.LPostureRead().LPostureStateSplit;
    }

    public double CAtelierVolumeRead()
    {
        return CAtelierPosture.LPostureRead().LPostureStateVolume;
    }

    public void CAtelierVolumeSet(double volume, bool settled)
    {
        CAtelierPosture.LPostureVolumeSet(volume);
        CAtelierMediaPort.LEngineVolumeSet(volume);
        if (settled)
        {
            CAtelierPosture.LPostureVolumeSave();
        }
    }

    public void CAtelierOpen()
    {
        CAtelierWorkspace.LWorkspaceOpen(LAtelierStateRead(CAtelierSettingsPort.LEngineWorkspaceStart()));
    }

    public bool CAtelierQuitConfirm(CEnvoy envoy)
    {
        return CAtelierWorkspace.LWorkspaceQuitConfirm(envoy);
    }

    public string CAtelierPathRead()
    {
        return CAtelierSettingsPort.LEngineWorkspaceRead();
    }

    internal static CWorkspaceState LAtelierStateRead(LWorkspaceState state)
    {
        return new CWorkspaceState(state.LWorkspaceStateLeft, state.LWorkspaceStateRight);
    }

    internal CEstablishment LAtelierEstablishmentRead()
    {
        LEstablishment establishment = CAtelierSettingsPort.LEngineEstablishmentRead();
        return new CEstablishment(
            establishment.LEstablishmentUnsaved,
            establishment.LEstablishmentEntry,
            establishment.LEstablishmentPending,
            establishment.LEstablishmentSingle ? "Establishment.EntryOne" : "Establishment.Entry",
            establishment.LEstablishmentLarge ? "Establishment.Megabyte" : "Establishment.Kilobyte",
            establishment.LEstablishmentAmount.ToString(
                establishment.LEstablishmentLarge ? "0.0" : "0", CultureInfo.CurrentCulture));
    }

    public bool CAtelierRecordingExist(string? file)
    {
        return CAtelierMediaPort.LEngineRecordingExist(file);
    }

    public Task<string> CAtelierRecordingPrepare(CRecording recording, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);

        return CAtelierMediaPort.LEngineRecordingPrepare(CErrand.CErrandRecordingRead(recording), cancellation);
    }

    public Uri? CAtelierLocationRead(string? location)
    {
        return CAtelierMediaPort.LEngineLocationRead(location);
    }

    public CScreen? CAtelierScreenRead(string? location)
    {
        return CAtelierMediaPort.LEngineScreenRead(location) is (Uri address, var film)
            ? new CScreen(address, film)
            : null;
    }

    public void CAtelierLocationOpen(string target)
    {
        CAtelierMediaPort.LEngineLocationOpen(target);
    }

    public static string CAtelierAboutRead()
    {
        return "Headquarter.Version";
    }

    public static string CAtelierRefusalRead(bool busy)
    {
        return busy ? "Workspace.Busy" : "Workspace.OpenFailed";
    }

    public static string? CAtelierRescueRead(bool done)
    {
        return done ? "Workspace.DatabaseReset" : null;
    }

    public void CAtelierClose()
    {
        CAtelierDraftPort.LEngineLeftoverSweep();
        CAtelierPosture.Dispose();
    }

    public void Dispose()
    {
        CAtelierClose();
    }

    internal Action LAtelierObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        return LAtelierObserverAdd(bulletin =>
        {
            if (bulletin.LBulletinSubject == CPanel.CPanelSubjectRead(subject))
            {
                observer(CAtelierBulletinRead(bulletin));
            }
        });
    }

    internal Action LAtelierObserverAdd(Action<LBulletin> sent)
    {
        CAtelierDraftPort.LEngineObserverAttach(sent);
        return () => CAtelierDraftPort.LEngineObserverDetach(sent);
    }

    internal static CBulletin CAtelierBulletinRead(LBulletin bulletin)
    {
        return new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored);
    }
}
