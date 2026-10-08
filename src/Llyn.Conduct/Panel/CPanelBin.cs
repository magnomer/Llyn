using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CPanelBin
{
    private readonly CEnvoy _cPanelBinEnvoy;

    private readonly LSettingsPort _cPanelBinSettings;

    private readonly LVistaPort _cPanelBinVistas;

    private readonly CAperture _cPanelBinAperture;

    private readonly string? _cPanelBinScope;

    internal CPanelBin(CEnvoy envoy, LSettingsPort settings, LVistaPort vistas, CAperture aperture, string? scope)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(vistas);
        ArgumentNullException.ThrowIfNull(aperture);

        _cPanelBinEnvoy = envoy;
        _cPanelBinSettings = settings;
        _cPanelBinVistas = vistas;
        _cPanelBinAperture = aperture;
        _cPanelBinScope = scope;
    }

    public event Action? CPanelBinDeleted;

    public void CPanelBinDelete()
    {
        if (_cPanelBinScope is not string scope || _cPanelBinAperture.CApertureVista is not LVista vista)
        {
            return;
        }

        if (!LPanelBinConfirm(scope, _cPanelBinVistas.LEngineUsageRead(vista)))
        {
            return;
        }

        try
        {
            _cPanelBinVistas.LEngineVistaDelete(vista);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cPanelBinEnvoy, _cPanelBinSettings, scope + ".DeleteFailed", exception);
            return;
        }

        CPanelBinDeleted?.Invoke();
    }

    private bool LPanelBinConfirm(string scope, int usage)
    {
        return usage > 0
            ? _cPanelBinEnvoy.CEnvoyConfirm(scope + ".DetachConfirm", scope + ".DetachCount", usage)
            : _cPanelBinEnvoy.CEnvoyConfirm(scope + ".DeleteConfirm");
    }
}
