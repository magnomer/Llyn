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

    private bool _cDeskChronicleHalted;

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

    public bool CDeskChronicleHalted =>
        _cDeskChronicleDraft.CDeskDraftTenure?.LTenureGauge.LTenureGaugeRead() is { LTenureStateHalted: true };

    public bool CDeskChronicleRunning => _cDeskChronicleDraft.CDeskDraftTenure is not null && !CDeskChronicleHalted;

    private bool CDeskChronicleStalling => CDeskChronicleHalted && !_cDeskChronicleHalted;

    internal void LDeskChronicleClear()
    {
        _cDeskChronicleHalted = false;
    }

    public void CDeskChronicleResonate()
    {
        if (CDeskChronicleStalling)
        {
            _cDeskChronicleEnvoy.CEnvoyFailureShow(_cDeskChronicleScope + ".HoldFailed");
        }

        _cDeskChronicleHalted = CDeskChronicleHalted;
        CDeskChronicleChanged?.Invoke();
    }

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
