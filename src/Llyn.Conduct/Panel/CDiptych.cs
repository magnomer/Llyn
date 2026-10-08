using System;

namespace Llyn.Conduct;

public sealed class CDiptych
{
    private readonly CPanel _cDiptychParent;

    private readonly CPanel _cDiptychChild;

    private readonly CSession _cDiptychSession;

    private readonly CNavigation _cDiptychStation;

    private readonly Action<long?> _cDiptychStartSeam;

    private readonly Action _cDiptychCancelSeam;

    private readonly Action? _cDiptychCreateSeam;

    internal CDiptych(
        CPanel parent,
        CPanel child,
        CSession session,
        CNavigation station,
        Action<long?> startSeam,
        Action cancelSeam,
        Action? createSeam)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(child);
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(station);
        ArgumentNullException.ThrowIfNull(startSeam);
        ArgumentNullException.ThrowIfNull(cancelSeam);

        _cDiptychParent = parent;
        _cDiptychChild = child;
        _cDiptychSession = session;
        _cDiptychStation = station;
        _cDiptychStartSeam = startSeam;
        _cDiptychCancelSeam = cancelSeam;
        _cDiptychCreateSeam = createSeam;
    }

    public bool CDiptychChildSide => _cDiptychChild.CPanelModeEnabled;

    public bool CDiptychParentEditing => !CDiptychChildSide && _cDiptychParent.CPanelEditing;

    public bool CDiptychParentShown => !CDiptychChildSide && !_cDiptychParent.CPanelEditing;

    public bool CDiptychChildEditing => _cDiptychChild.CPanelEditing;

    public bool CDiptychChildShown => CDiptychChildSide && !_cDiptychChild.CPanelEditing;

    public bool CDiptychScribeChecked => CDiptychParentEditing || CDiptychChildEditing;

    public bool CDiptychModeEnabled => CDiptychChildSide || _cDiptychParent.CPanelModeEnabled;

    public bool CDiptychBinEnabled => !CDiptychChildSide && _cDiptychParent.CPanelBinEnabled;

    private bool LDiptychRowHeld => _cDiptychParent.CPanelBinEnabled || _cDiptychChild.CPanelBinEnabled;

    public void CDiptychParentSelect(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        bool editing = CDiptychScribeChecked;
        if (!_cDiptychSession.LSessionLeaveConfirm(false))
        {
            return;
        }

        _cDiptychStation.LNavigationStationAdd();
        LDiptychParentShow(chosen, editing);
    }

    internal void LDiptychParentShow(long id, bool editing)
    {
        _cDiptychChild.CPanelEntryClose();
        _cDiptychParent.CPanelScribeSet(editing);
        _cDiptychParent.CPanelRowOpen(id);
    }

    public void CDiptychChildSelect(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        bool editing = CDiptychScribeChecked;
        if (!_cDiptychSession.LSessionLeaveConfirm(true))
        {
            return;
        }

        LDiptychChildOpen(chosen, editing);
    }

    private void LDiptychChildOpen(long id, bool editing)
    {
        if (!_cDiptychChild.CPanelRowOpen(id))
        {
            return;
        }

        _cDiptychCancelSeam();
        _cDiptychParent.CPanelScribeSet(false);
        if (editing)
        {
            _cDiptychChild.CPanelScribeToggle(true);
        }
    }

    public void CDiptychEntryCreate()
    {
        if (!_cDiptychSession.LSessionLeaveConfirm(true))
        {
            return;
        }

        if (_cDiptychCreateSeam is not null && LDiptychRowHeld)
        {
            _cDiptychCreateSeam();
            return;
        }

        _cDiptychChild.CPanelEntryClose();
        _cDiptychParent.CPanelFreshOpen();
        _cDiptychStartSeam(null);
    }

    public void CDiptychScribeToggle(bool editing)
    {
        if (CDiptychChildSide)
        {
            _cDiptychChild.CPanelScribeToggle(editing);
            if (!CDiptychChildSide)
            {
                LDiptychParentRestore(editing);
            }

            return;
        }

        _cDiptychParent.CPanelScribeToggle(editing);
        if (!_cDiptychParent.CPanelEditing)
        {
            _cDiptychCancelSeam();
        }
    }

    private void LDiptychParentRestore(bool editing)
    {
        if (_cDiptychParent.CPanelAperture.CApertureChosen is long chosen)
        {
            LDiptychParentShow(chosen, editing);
            return;
        }

        LDiptychEntryClose();
    }

    internal void LDiptychEntryClose()
    {
        _cDiptychChild.CPanelEntryClose();
        _cDiptychParent.CPanelEntryClose();
    }

    internal void LDiptychChildResonate()
    {
        if (!_cDiptychChild.CPanelBinEnabled)
        {
            return;
        }

        bool editing = CDiptychScribeChecked;
        _cDiptychChild.CPanelDraftResonate();
        if (CDiptychChildSide)
        {
            return;
        }

        LDiptychParentRestore(editing);
    }

    public void CDiptychEntryDelete()
    {
        if (CDiptychChildSide)
        {
            return;
        }

        _cDiptychParent.CPanelBin.CPanelBinDelete();
    }
}
