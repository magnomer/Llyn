using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CFold
{
    private readonly LSettingsPort _cFoldSettingsPort;

    private readonly CEnvoy _cFoldEnvoy;

    private Action? _cFoldObserver;

    internal CFold(LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cFoldSettingsPort = settings;
        _cFoldEnvoy = envoy;
    }

    public event Action? CFoldChanged;

    internal void LFoldObserverAttach(Action<Action> marshal)
    {
        _cFoldObserver = () => marshal(() => CFoldChanged?.Invoke());
        LFoldAttach();
    }

    internal void LFoldAttach()
    {
        _cFoldSettingsPort.LEngineFoldChanged -= _cFoldObserver;
        _cFoldSettingsPort.LEngineFoldChanged += _cFoldObserver;
    }

    internal void LFoldDetach()
    {
        _cFoldSettingsPort.LEngineFoldChanged -= _cFoldObserver;
    }

    public bool CFoldFanqieOpened => _cFoldSettingsPort.LEngineSettingsRead().LSettingsFanqieOpened;

    public bool CFoldScriptOpened => _cFoldSettingsPort.LEngineSettingsRead().LSettingsScriptOpened;

    public void CFoldFanqieToggle(bool opened)
    {
        try
        {
            _cFoldSettingsPort.LEngineFanqieSave(opened);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cFoldEnvoy, _cFoldSettingsPort, "Settings.SaveFailed", exception);
        }

        CFoldChanged?.Invoke();
    }

    public void CFoldScriptToggle(bool opened)
    {
        try
        {
            _cFoldSettingsPort.LEngineScriptSave(opened);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cFoldEnvoy, _cFoldSettingsPort, "Settings.SaveFailed", exception);
        }

        CFoldChanged?.Invoke();
    }
}
