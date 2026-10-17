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

    private readonly Func<long?, bool> _cPanelBinSeam;

    internal CPanelBin(
        CEnvoy envoy,
        LSettingsPort settings,
        LVistaPort vistas,
        CAperture aperture,
        string? scope,
        Func<long?, bool> seam)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(vistas);
        ArgumentNullException.ThrowIfNull(aperture);
        ArgumentNullException.ThrowIfNull(seam);

        _cPanelBinEnvoy = envoy;
        _cPanelBinSettings = settings;
        _cPanelBinVistas = vistas;
        _cPanelBinAperture = aperture;
        _cPanelBinScope = scope;
        _cPanelBinSeam = seam;
    }

    public event Action? CPanelBinDeleted;

    public bool CPanelBinDelete(long id)
    {
        return _cPanelBinSeam(id) && CPanelBinDelete();
    }

    public bool CPanelBinDelete()
    {
        if (_cPanelBinScope is not string scope || _cPanelBinAperture.CApertureVista is not LVista vista)
        {
            return false;
        }

        if (!LPanelBinConfirm(scope, _cPanelBinVistas.LEngineUsageRead(vista)))
        {
            return false;
        }

        try
        {
            _cPanelBinVistas.LEngineVistaDelete(vista);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cPanelBinEnvoy, _cPanelBinSettings, scope + ".DeleteFailed", exception);
            return false;
        }

        CPanelBinDeleted?.Invoke();
        return true;
    }

    private bool LPanelBinConfirm(string scope, int usage)
    {
        return usage > 0
            ? _cPanelBinEnvoy.CEnvoyConfirm(scope + ".DetachConfirm", scope + ".DetachCount", usage)
            : _cPanelBinEnvoy.CEnvoyConfirm(scope + ".DeleteConfirm");
    }
}
