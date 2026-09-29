using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CWorkspace
{
    private readonly CAtelier _cWorkspaceAtelier;

    private readonly List<(Func<bool> LWorkspacePending, Func<bool, bool> LWorkspaceClosure)> _cWorkspaceDrafts = [];

    private readonly List<Action> _cWorkspaceVistas = [];

    private readonly List<Action> _cWorkspaceClosures = [];

    private CEditor? _cWorkspaceInput;

    private bool _cWorkspaceHeard;

    internal CWorkspace(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cWorkspaceAtelier = atelier;
    }

    public event Action? CWorkspaceOpened;

    internal event Action<CWorkspaceState>? LWorkspaceStateOpened;

    public event Action<CEstablishment>? CWorkspaceEstablishmentChanged;

    public string CWorkspaceChange(string chosen, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(chosen);
        ArgumentNullException.ThrowIfNull(envoy);

        LSettingsPort settings = _cWorkspaceAtelier.CAtelierSettingsPort;
        CWorkspaceState state;
        try
        {
            if (!settings.LEngineWorkspaceCheck(chosen) || !LWorkspaceQuitConfirm(envoy))
            {
                return _cWorkspaceAtelier.CAtelierPathRead();
            }

            state = CAtelier.LAtelierStateRead(settings.LEngineWorkspaceChange(chosen));
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Workspace.OpenFailed", exception);
            return _cWorkspaceAtelier.CAtelierPathRead();
        }

        LWorkspaceVistaRestore();
        LWorkspaceOpen(state);
        return _cWorkspaceAtelier.CAtelierPathRead();
    }

    public string CWorkspaceChange(CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        string? chosen;
        try
        {
            chosen = envoy.CEnvoyWorkspaceRead(_cWorkspaceAtelier.CAtelierPathRead());
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(
                envoy, _cWorkspaceAtelier.CAtelierSettingsPort, "Workspace.OpenFailed", exception);
            return _cWorkspaceAtelier.CAtelierPathRead();
        }

        return chosen is null ? _cWorkspaceAtelier.CAtelierPathRead() : CWorkspaceChange(chosen, envoy);
    }

    internal void LWorkspaceOpen(CWorkspaceState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (!_cWorkspaceHeard)
        {
            _cWorkspaceHeard = true;
            _cWorkspaceAtelier.CAtelierLedger.LLedgerAttach();
            _cWorkspaceAtelier.LAtelierObserverAdd(_ => LWorkspaceEstablishmentRaise());
        }

        CWorkspaceOpened?.Invoke();
        _cWorkspaceAtelier.CAtelierLedger.LLedgerRaise();
        LWorkspaceEstablishmentRaise();
        LWorkspaceStateOpened?.Invoke(state);
        _cWorkspaceAtelier.CAtelierNavigation.LNavigationTabOpen();
    }

    internal bool LWorkspaceQuitConfirm(CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(envoy);

        bool unsaved = _cWorkspaceInput?.CEditorDesk.LDeskChangeCheck() ?? false;
        foreach ((Func<bool> pending, _) in _cWorkspaceDrafts)
        {
            unsaved |= pending();
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

        bool finished = _cWorkspaceInput?.LEditorFinish(store) ?? true;
        foreach ((_, Func<bool, bool> closure) in _cWorkspaceDrafts)
        {
            finished &= closure(store);
        }

        return finished;
    }

    internal void LWorkspaceDraftAdd(Func<bool> pending, Func<bool, bool> closure)
    {
        ArgumentNullException.ThrowIfNull(pending);
        ArgumentNullException.ThrowIfNull(closure);

        _cWorkspaceDrafts.Add((pending, closure));
    }

    internal void LWorkspaceVistaAdd(Action restore)
    {
        ArgumentNullException.ThrowIfNull(restore);

        _cWorkspaceVistas.Add(restore);
    }

    internal void LWorkspaceClosureAdd(Action closure)
    {
        ArgumentNullException.ThrowIfNull(closure);

        _cWorkspaceClosures.Add(closure);
    }

    internal void LWorkspaceClose()
    {
        foreach (Action closure in _cWorkspaceClosures)
        {
            closure();
        }
    }

    private void LWorkspaceVistaRestore()
    {
        foreach (Action restore in _cWorkspaceVistas)
        {
            restore();
        }

        if (_cWorkspaceInput is CEditor input)
        {
            _cWorkspaceAtelier.LAtelierInputRestore(input);
        }
    }

    internal void LWorkspaceInputSet(CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        _cWorkspaceInput = editor;
    }

    private void LWorkspaceEstablishmentRaise()
    {
        CEstablishment establishment;
        try
        {
            establishment = _cWorkspaceAtelier.LAtelierEstablishmentRead();
        }
        catch (Exception)
        {
            return;
        }

        CWorkspaceEstablishmentChanged?.Invoke(establishment);
    }
}
