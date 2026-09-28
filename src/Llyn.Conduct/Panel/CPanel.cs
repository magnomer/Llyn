using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CPanel
{
    private readonly CEnvoy _cPanelEnvoy;

    private readonly string _cPanelLoadKey;

    private readonly string? _cPanelDeleteScope;

    private readonly Func<bool> _cPanelChangeSeam;

    private readonly Func<bool, bool> _cPanelFinishSeam;

    private readonly Func<bool> _cPanelShownSeam;

    private LVista? _cPanelVista;

    internal CPanel(
        CEnvoy envoy,
        string loadKey,
        string? deleteScope,
        Func<bool> changeSeam,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentException.ThrowIfNullOrWhiteSpace(loadKey);
        ArgumentNullException.ThrowIfNull(changeSeam);
        ArgumentNullException.ThrowIfNull(finishSeam);
        ArgumentNullException.ThrowIfNull(shownSeam);

        _cPanelEnvoy = envoy;
        _cPanelLoadKey = loadKey;
        _cPanelDeleteScope = deleteScope;
        _cPanelChangeSeam = changeSeam;
        _cPanelFinishSeam = finishSeam;
        _cPanelShownSeam = shownSeam;
    }

    public event Action? CPanelChanged;

    public event Action? CPanelRowsChanged;

    public event Action? CPanelCleared;

    internal event Action<LDraft>? CPanelDraftChanged;

    public event Action<long>? CPanelEdited;

    internal LVista? CPanelVista => _cPanelVista;

    public bool CPanelEditing => _cPanelVista?.LVistaEditing ?? false;

    public bool CPanelBinEnabled => _cPanelVista?.LVistaChosen is not null;

    public bool CPanelModeEnabled => CPanelBinEnabled || CPanelEditing;

    public bool CPanelViewerChecked => !CPanelEditing;

    public bool CPanelScribeChecked => CPanelEditing;

    public bool CPanelPressAllowed => CPanelBinEnabled && !CPanelEditing;

    public CCatalogOrder CPanelOrder => CPanelOrderRead(LVista.LVistaOrderRead(_cPanelVista));

    public CCatalogFilter CPanelFilter => CPanelFilterRead(LVista.LVistaFilterRead(_cPanelVista));

    public void CPanelRowsUpdate()
    {
        CPanelRowsChanged?.Invoke();
    }

    internal void CPanelVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _cPanelVista = vista;
        if (vista.LVistaEditing)
        {
            vista.LVistaEditingSet(false);
        }
    }

    public void CPanelObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _cPanelVista?.LVistaObserverAttach(
            CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    public void CPanelChosenAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _cPanelVista?.LVistaChosenAttach(
            CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    public bool CPanelChangeCheck()
    {
        if (!CPanelEditing)
        {
            return false;
        }

        return _cPanelChangeSeam();
    }

    public bool CPanelLeaveConfirm()
    {
        if (!CPanelChangeCheck())
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
        _cPanelVista?.LVistaSelect(null);
        CPanelRowsUpdate();
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

    public void CPanelScribeRestore(bool editing)
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
            LPanelDraftShow(() => _cPanelVista?.LVistaLoad());
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

        if (_cPanelVista?.LVistaChosen is long shown)
        {
            CPanelEdited?.Invoke(shown);
        }

        CPanelScribeSet(true);
    }

    public void CPanelScribeSet(bool editing)
    {
        _cPanelVista?.LVistaEditingSet(editing);
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

        CPanelRowOpen(id);
    }

    public long CPanelChosenRead()
    {
        return _cPanelVista?.LVistaChosen ?? 0;
    }

    public bool CPanelRowOpen(long? id)
    {
        return LPanelDraftShow(() => _cPanelVista?.LVistaLoad(id));
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
            _cPanelEnvoy.CEnvoyFailureShow(_cPanelLoadKey, exception);
            return false;
        }

        CPanelRowsUpdate();
        if (!CPanelBinEnabled)
        {
            CPanelEntryClose();
            return false;
        }

        if (draft is LDraft loaded)
        {
            CPanelDraftChanged?.Invoke(loaded);
            CPanelChanged?.Invoke();
            if (CPanelEditing && _cPanelVista?.LVistaChosen is long shown)
            {
                CPanelEdited?.Invoke(shown);
            }
        }

        return true;
    }

    public void CPanelDraftUpdate()
    {
        LDraft? draft;
        try
        {
            draft = _cPanelVista?.LVistaLoad();
        }
        catch (Exception)
        {
            return;
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

    public void CPanelEntryUpdate(CBulletin bulletin)
    {
        CPanelEntrySelect(bulletin);
        CPanelChanged?.Invoke();
        CPanelRowsUpdate();
    }

    public void CPanelEntrySelect(CBulletin bulletin)
    {
        ArgumentNullException.ThrowIfNull(bulletin);

        if (!bulletin.CBulletinStored || CPanelBinEnabled || !CPanelEditing || !_cPanelShownSeam())
        {
            return;
        }

        _cPanelVista?.LVistaSelect(bulletin.CBulletinId);
    }

    public void CPanelEntryDelete()
    {
        if (_cPanelDeleteScope is not string scope || _cPanelVista is not LVista vista)
        {
            return;
        }

        if (!LPanelDeleteConfirm(scope, vista.LVistaUsageRead()))
        {
            return;
        }

        try
        {
            vista.LVistaDelete();
        }
        catch (Exception exception)
        {
            _cPanelEnvoy.CEnvoyFailureShow(scope + ".DeleteFailed", exception);
            return;
        }

        CPanelEntryClose();
    }

    private bool LPanelDeleteConfirm(string scope, int usage)
    {
        return usage > 0
            ? _cPanelEnvoy.CEnvoyConfirm(scope + ".DetachConfirm", scope + ".DetachCount", usage)
            : _cPanelEnvoy.CEnvoyConfirm(scope + ".DeleteConfirm");
    }

    internal static CCatalogOrder CPanelOrderRead(LCatalogOrder order)
    {
        return (CCatalogOrder)order;
    }

    internal static LCatalogOrder? CPanelOrderRead(CCatalogOrder? order)
    {
        return (LCatalogOrder?)order;
    }

    internal static CCatalogFilter CPanelFilterRead(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return new CCatalogFilter(filter.LCatalogFilterHidden);
    }

    internal static LSubject CPanelSubjectRead(CSubject subject)
    {
        return (LSubject)subject;
    }

    internal static CVistaRow CPanelRowRead(LVistaRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new CVistaRow(
            row.LVistaRowId,
            row.LVistaRowHeadword,
            row.LVistaRowLanguage,
            row.LVistaRowEpithet ?? string.Empty,
            row.LVistaRowName,
            row.LVistaRowChosen);
    }
}
