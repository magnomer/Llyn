using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CPanel
{
    private readonly CEnvoy _cPanelEnvoy;

    private readonly LSettingsPort _cPanelSettingsPort;

    private readonly LVistaPort _cPanelVistaPort;

    private readonly string _cPanelLoadKey;

    private readonly Func<bool> _cPanelChangeSeam;

    private readonly Func<bool, bool> _cPanelFinishSeam;

    private readonly Func<bool> _cPanelShownSeam;

    private Action? _cPanelStation;

    internal CPanel(
        CEnvoy envoy,
        LSettingsPort settings,
        LVistaPort vistas,
        string loadKey,
        string? deleteScope,
        Func<bool> changeSeam,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam,
        string? vacantKey = null,
        string? unmatchedKey = null)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(vistas);
        ArgumentException.ThrowIfNullOrWhiteSpace(loadKey);
        ArgumentNullException.ThrowIfNull(changeSeam);
        ArgumentNullException.ThrowIfNull(finishSeam);
        ArgumentNullException.ThrowIfNull(shownSeam);

        _cPanelEnvoy = envoy;
        _cPanelSettingsPort = settings;
        _cPanelVistaPort = vistas;
        _cPanelLoadKey = loadKey;
        _cPanelChangeSeam = changeSeam;
        _cPanelFinishSeam = finishSeam;
        _cPanelShownSeam = shownSeam;
        CPanelAperture = new CAperture(envoy, settings, vistas, loadKey, vacantKey, unmatchedKey);
        CPanelBin = new CPanelBin(envoy, settings, vistas, CPanelAperture, deleteScope);
        CPanelBin.CPanelBinDeleted += CPanelEntryClose;
    }

    public event Action? CPanelChanged;

    public event Action? CPanelCleared;

    internal event Action<LDraft>? CPanelDraftChanged;

    public event Action<long>? CPanelEdited;

    public CAperture CPanelAperture { get; }

    public CPanelBin CPanelBin { get; }

    public bool CPanelEditing => CPanelAperture.CApertureVista?.LVistaEditing ?? false;

    public bool CPanelBinEnabled => CPanelAperture.CApertureVista?.LVistaChosen is not null;

    public bool CPanelModeEnabled => CPanelBinEnabled || CPanelEditing;

    public bool CPanelViewerChecked => !CPanelEditing;

    public bool CPanelScribeChecked => CPanelEditing;

    public bool CPanelPressAllowed => CPanelBinEnabled && !CPanelEditing;

    internal void CPanelVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        CPanelAperture.CApertureRestore(vista);
        if (vista.LVistaEditing)
        {
            vista.LVistaEditingSet(false);
        }
    }

    internal bool LPanelChangeCheck()
    {
        if (!CPanelEditing)
        {
            return false;
        }

        return _cPanelChangeSeam();
    }

    public bool CPanelLeaveConfirm()
    {
        if (!LPanelChangeCheck())
        {
            return true;
        }

        if (_cPanelEnvoy.CEnvoyLeaveConfirm() is not bool store)
        {
            return false;
        }

        return !store || _cPanelFinishSeam(true);
    }

    public void CPanelEntryClose()
    {
        CPanelAperture.CApertureVista?.LVistaSelect(null);
        CPanelAperture.CApertureRowsResonate();
        CPanelCleared?.Invoke();
        CPanelScribeSet(false);
    }

    public void CPanelEntryCreate()
    {
        if (!CPanelLeaveConfirm())
        {
            return;
        }

        CPanelFreshOpen();
    }

    public void CPanelFreshOpen()
    {
        CPanelEntryClose();
        CPanelScribeSet(true);
    }

    internal void LPanelScribeRestore(bool editing)
    {
        if (editing && !CPanelBinEnabled)
        {
            return;
        }

        CPanelScribeSet(editing);
    }

    public void CPanelScribeToggle(bool editing)
    {
        if (editing == CPanelEditing)
        {
            return;
        }

        if (editing)
        {
            LPanelScribeOpen();
            return;
        }

        if (!CPanelLeaveConfirm())
        {
            CPanelScribeSet(true);
            return;
        }

        CPanelScribeSet(false);
        if (CPanelBinEnabled)
        {
            LPanelDraftShow(
                () => CPanelAperture.CApertureVista is null
                    ? null
                    : _cPanelVistaPort.LEngineVistaLoad(CPanelAperture.CApertureVista));
            return;
        }

        CPanelEntryClose();
    }

    private void LPanelScribeOpen()
    {
        if (!CPanelBinEnabled)
        {
            CPanelEntryClose();
            return;
        }

        if (CPanelAperture.CApertureVista?.LVistaChosen is long shown)
        {
            CPanelEdited?.Invoke(shown);
        }

        CPanelScribeSet(true);
    }

    public void CPanelScribeSet(bool editing)
    {
        CPanelAperture.CApertureVista?.LVistaEditingSet(editing);
        CPanelChanged?.Invoke();
    }

    public void CPanelRowSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!CPanelLeaveConfirm())
        {
            return;
        }

        _cPanelStation?.Invoke();
        CPanelRowOpen(id);
    }

    internal void LPanelStationAttach(Action record)
    {
        ArgumentNullException.ThrowIfNull(record);

        _cPanelStation = record;
    }

    internal long LPanelChosenRead()
    {
        return CPanelAperture.CApertureVista?.LVistaChosen ?? 0;
    }

    public bool CPanelRowOpen(long? id)
    {
        return LPanelDraftShow(
            () => CPanelAperture.CApertureVista is null
                ? null
                : _cPanelVistaPort.LEngineVistaLoad(CPanelAperture.CApertureVista, id));
    }

    private bool LPanelDraftShow(Func<LDraft?> load)
    {
        LDraft? draft;
        try
        {
            draft = load();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cPanelEnvoy, _cPanelSettingsPort, _cPanelLoadKey, exception);
            return false;
        }

        CPanelAperture.CApertureRowsResonate();
        if (!CPanelBinEnabled)
        {
            CPanelEntryClose();
            return false;
        }

        if (draft is LDraft loaded)
        {
            CPanelDraftChanged?.Invoke(loaded);
            CPanelChanged?.Invoke();
            if (CPanelEditing && CPanelAperture.CApertureVista?.LVistaChosen is long shown)
            {
                CPanelEdited?.Invoke(shown);
            }
        }

        return true;
    }

    public void CPanelDraftResonate()
    {
        LDraft? draft;
        try
        {
            draft = CPanelAperture.CApertureVista is null
                ? null
                : _cPanelVistaPort.LEngineVistaLoad(CPanelAperture.CApertureVista);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cPanelEnvoy, _cPanelSettingsPort, _cPanelLoadKey, exception);
            draft = null;
        }

        if (!CPanelBinEnabled)
        {
            CPanelEntryClose();
            return;
        }

        if (draft is LDraft loaded)
        {
            CPanelDraftChanged?.Invoke(loaded);
            CPanelChanged?.Invoke();
        }
    }

    public void CPanelEntryResonate(CBulletin bulletin)
    {
        CPanelEntrySelect(bulletin);
        CPanelChanged?.Invoke();
        CPanelAperture.CApertureRowsResonate();
    }

    public void CPanelEntrySelect(CBulletin bulletin)
    {
        ArgumentNullException.ThrowIfNull(bulletin);

        if (!bulletin.CBulletinStored || CPanelBinEnabled || !CPanelEditing || !_cPanelShownSeam())
        {
            return;
        }

        CPanelAperture.CApertureVista?.LVistaSelect(bulletin.CBulletinId);
    }
}
