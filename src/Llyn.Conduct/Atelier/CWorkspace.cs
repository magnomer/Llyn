using System;
using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed class CWorkspace
{
    private readonly CAtelier _cWorkspaceAtelier;

    private readonly List<(Func<bool> LWorkspacePending, Func<bool, bool> LWorkspaceClosure)> _cWorkspaceDrafts = [];

    private CEditor? _cWorkspaceInput;

    private bool _cWorkspaceHeard;

    internal CWorkspace(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cWorkspaceAtelier = atelier;
    }

    public event Action? CWorkspaceOpened;

    public event Action<CWorkspaceState>? CWorkspaceStateOpened;

    public event Action<CEstablishment>? CWorkspaceEstablishmentChanged;

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
        CWorkspaceStateOpened?.Invoke(state);
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
