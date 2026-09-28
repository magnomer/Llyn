using System;
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
        CAtelierMarkdown = new CMarkdown(this);
        CAtelierRespelling = new CRespelling(this);
    }

    public CMention CAtelierMention { get; }

    public CMarkdown CAtelierMarkdown { get; }

    public CRespelling CAtelierRespelling { get; }

    public CCatalog CAtelierCatalog => new(this);

    public CLedger CAtelierLedger => new(this);

    internal LPosture CAtelierPosture { get; }

    internal LDraftPort CAtelierDraftPort { get; }

    internal LEntryPort CAtelierEntryPort { get; }

    internal LSettingsPort CAtelierSettingsPort { get; }

    internal LPhonologyPort CAtelierPhonologyPort { get; }

    internal LMediaPort CAtelierMediaPort { get; }

    internal LPortraitPort CAtelierPortraitPort { get; }

    internal LVista CAtelierVistaStart(string tab, CSubject? subject, CCatalogOrder fallback, bool blank = false)
    {
        return CAtelierPosture.LPostureVistaStart(tab, (LSubject?)subject, (LCatalogOrder)fallback, blank);
    }


    internal void CAtelierInputRestore(CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        editor.CEditorVistaRestore(
            CAtelierVistaStart("input", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    public bool CAtelierSplitRead()
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

    public void CAtelierLeftoverSweep()
    {
        CAtelierDraftPort.LEngineLeftoverSweep();
        CAtelierMediaPort.LEngineRecordingSweep();
    }

    public bool CAtelierQuitConfirm(
        IReadOnlyList<Func<bool>> pending, IReadOnlyList<Func<bool, bool>> closures, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(pending);
        ArgumentNullException.ThrowIfNull(closures);
        ArgumentNullException.ThrowIfNull(envoy);

        bool unsaved = false;
        foreach (Func<bool> check in pending)
        {
            unsaved |= check();
        }

        bool store = false;
        if (unsaved)
        {
            if (envoy.CEnvoyLeaveConfirm() is not bool answer)
            {
                return false;
            }

            store = answer;
        }

        bool finished = true;
        foreach (Func<bool, bool> finish in closures)
        {
            finished &= finish(store);
        }

        return finished;
    }

    public CWorkspaceState? CAtelierWorkspaceChange(string chosen, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(chosen);
        ArgumentNullException.ThrowIfNull(envoy);

        string path = chosen.Trim();
        if (path.Length == 0
            || string.Equals(path, CAtelierPathRead(), StringComparison.Ordinal)
            || !envoy.CEnvoyDiscardConfirm())
        {
            return null;
        }

        CAtelierSettingsPort.LEngineWorkspaceChange(path);
        return CAtelierStateRead();
    }

    public string CAtelierPathRead()
    {
        return CAtelierSettingsPort.LEngineWorkspaceRead();
    }

    public CWorkspaceState CAtelierStateRead()
    {
        LWorkspaceState state = CAtelierSettingsPort.LEngineStateRead();
        return new CWorkspaceState(state.LWorkspaceStateLeft, state.LWorkspaceStateRight);
    }

    public CEstablishment CAtelierEstablishmentRead()
    {
        LEstablishment establishment = CAtelierSettingsPort.LEngineEstablishmentRead();
        return new CEstablishment(
            establishment.LEstablishmentUnsaved,
            establishment.LEstablishmentEntry,
            establishment.LEstablishmentSize,
            establishment.LEstablishmentPending,
            establishment.LEstablishmentSingle);
    }

    public Action CAtelierEstablishmentAttach(Action<CEstablishment> show)
    {
        ArgumentNullException.ThrowIfNull(show);

        Action detach = CAtelierObserverAdd(_ => CAtelierEstablishmentShow(show));
        CAtelierEstablishmentShow(show);
        return detach;
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

    public void CAtelierLocationOpen(string target)
    {
        CAtelierMediaPort.LEngineLocationOpen(target);
    }

    public static string CAtelierAboutRead()
    {
        return "Headquarter.Version";
    }

    public void Dispose()
    {
        CAtelierDraftPort.LEngineLeftoverSweep();
        CAtelierPosture.Dispose();
    }

    public Action CAtelierObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        return CAtelierObserverAdd(bulletin =>
        {
            if ((CSubject)bulletin.LBulletinSubject == subject)
            {
                observer(CAtelierBulletinRead(bulletin));
            }
        });
    }

    private Action CAtelierObserverAdd(Action<LBulletin> sent)
    {
        CAtelierDraftPort.LEngineObserverAttach(sent);
        return () => CAtelierDraftPort.LEngineObserverDetach(sent);
    }

    private void CAtelierEstablishmentShow(Action<CEstablishment> show)
    {
        CEstablishment establishment;
        try
        {
            establishment = CAtelierEstablishmentRead();
        }
        catch (Exception)
        {
            return;
        }

        show(establishment);
    }

    internal static CBulletin CAtelierBulletinRead(LBulletin bulletin)
    {
        return new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored);
    }
}
