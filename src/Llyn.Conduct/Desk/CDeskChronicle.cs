using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDeskChronicle
{
    private readonly CDeskDraft _cDeskChronicleDraft;

    private readonly LSettingsPort _cDeskChronicleSettings;

    private readonly string _cDeskChronicleScope;

    private readonly CEnvoy _cDeskChronicleEnvoy;

    internal CDeskChronicle(CDeskDraft draft, LSettingsPort settings, string scope, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDeskChronicleDraft = draft;
        _cDeskChronicleSettings = settings;
        _cDeskChronicleScope = scope;
        _cDeskChronicleEnvoy = envoy;
    }

    internal event Action? CDeskChronicleChanged;

    public bool CDeskChronicleHalted { get; private set; }

    public bool CDeskChronicleRunning => _cDeskChronicleDraft.CDeskDraftTenure is not null && !CDeskChronicleHalted;

    internal void LDeskChronicleClear()
    {
        CDeskChronicleHalted = false;
    }

    public void CDeskChronicleResonate()
    {
        bool halted =
            _cDeskChronicleDraft.CDeskDraftTenure?.LTenureGauge.LTenureGaugeRead() is { LTenureStateHalted: true };
        if (halted && !CDeskChronicleHalted)
        {
            _cDeskChronicleEnvoy.CEnvoyFailureShow(_cDeskChronicleScope + ".HoldFailed");
        }

        CDeskChronicleHalted = halted;
        CDeskChronicleChanged?.Invoke();
    }

    public (bool CDeskBackward, bool CDeskForward) CDeskChronicleState { get; internal set; }

    public (bool CDeskBackward, bool CDeskForward) CDeskChronicleRead()
    {
        if (_cDeskChronicleDraft.CDeskDraftTenure is not LTenure held)
        {
            return (false, false);
        }

        LTenureState state = held.LTenureGauge.LTenureGaugeRead();
        return (state.LTenureStateBackward, state.LTenureStateForward);
    }

    public void CDeskChronicleUndo()
    {
        try
        {
            _cDeskChronicleDraft.CDeskDraftTenure?.LTenureUndo();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(
                _cDeskChronicleEnvoy, _cDeskChronicleSettings, _cDeskChronicleScope + ".HoldFailed", exception);
        }

        CDeskChronicleChanged?.Invoke();
    }

    public void CDeskChronicleRedo()
    {
        try
        {
            _cDeskChronicleDraft.CDeskDraftTenure?.LTenureRedo();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(
                _cDeskChronicleEnvoy, _cDeskChronicleSettings, _cDeskChronicleScope + ".HoldFailed", exception);
        }

        CDeskChronicleChanged?.Invoke();
    }
}
