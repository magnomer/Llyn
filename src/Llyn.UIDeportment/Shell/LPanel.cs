using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LPanel
{
    private readonly string _lPanelLoadKey;

    private readonly string _lPanelDeleteKey;

    private readonly Func<bool> _lPanelChangeSeam;

    private readonly Func<bool> _lPanelShownSeam;

    private readonly Func<bool> _lPanelLeaveSeam;

    private readonly Func<bool> _lPanelDeleteSeam;

    private LVista? _lPanelVista;

    public LPanel(
        string loadKey,
        string deleteKey,
        Func<bool> changeSeam,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loadKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(deleteKey);
        ArgumentNullException.ThrowIfNull(changeSeam);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(leaveSeam);
        ArgumentNullException.ThrowIfNull(deleteSeam);

        _lPanelLoadKey = loadKey;
        _lPanelDeleteKey = deleteKey;
        _lPanelChangeSeam = changeSeam;
        _lPanelShownSeam = shownSeam;
        _lPanelLeaveSeam = leaveSeam;
        _lPanelDeleteSeam = deleteSeam;
    }

    public event Action? LPanelChanged;

    public event Action? LPanelRowsChanged;

    public event Action? LPanelCleared;

    public event Action<LDraft>? LPanelDraftChanged;

    public event Action<long>? LPanelEdited;

    public event Action<string, Exception>? LPanelFailed;

    public LVista? LPanelVista => _lPanelVista;

    public bool LPanelEditing => _lPanelVista?.LVistaEditing ?? false;

    public bool LPanelBinEnabled => _lPanelVista?.LVistaChosen is not null;

    public bool LPanelModeEnabled => LPanelBinEnabled || LPanelEditing;

    public bool LPanelViewerChecked => !LPanelEditing;

    public bool LPanelScribeChecked => LPanelEditing;

    public bool LPanelPressAllowed => LPanelBinEnabled && !LPanelEditing;

    public void LPanelRowsUpdate()
    {
        LPanelRowsChanged?.Invoke();
    }

    public void LPanelVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lPanelVista = vista;
        if (vista.LVistaEditing)
        {
            vista.LVistaEditingSet(false);
        }
    }

    public void LPanelObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lPanelVista?.LVistaObserverAttach(subject, observer);
    }

    public void LPanelChosenAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lPanelVista?.LVistaChosenAttach(subject, observer);
    }

    public LCatalogOrder LPanelOrder => _lPanelVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword;

    public LCatalogFilter LPanelFilter => _lPanelVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;

    public bool LPanelChangeCheck()
    {
        if (!LPanelEditing)
        {
            return false;
        }

        return _lPanelChangeSeam();
    }

    public bool LPanelLeaveConfirm()
    {
        if (!LPanelChangeCheck())
        {
            return true;
        }

        return _lPanelLeaveSeam();
    }

    private bool LPanelShownCheck()
    {
        return _lPanelShownSeam();
    }

    private bool LPanelDeleteConfirm()
    {
        return _lPanelDeleteSeam();
    }

    public void LPanelClear()
    {
        _lPanelVista?.LVistaSelect(null);
        LPanelRowsUpdate();
        LPanelCleared?.Invoke();
        LPanelScribeShow(false);
    }

    public void LPanelFreshStart()
    {
        if (!LPanelLeaveConfirm())
        {
            return;
        }

        LPanelFreshOpen();
    }

    public void LPanelFreshOpen()
    {
        LPanelClear();
        LPanelScribeShow(true);
    }

    public void LPanelScribeRestore(bool editing)
    {
        if (editing)
        {
            if (!LPanelBinEnabled)
            {
                return;
            }
        }

        LPanelScribeShow(editing);
    }

    public void LPanelScribeSet(bool editing)
    {
        if (editing)
        {
            if (LPanelEditing)
            {
                return;
            }

            LPanelScribeOpen();
            return;
        }

        if (!LPanelEditing)
        {
            return;
        }

        if (!LPanelLeaveConfirm())
        {
            LPanelScribeShow(true);
            return;
        }

        LPanelScribeShow(false);
        if (LPanelBinEnabled)
        {
            LPanelDraftShow(() => _lPanelVista?.LVistaLoad());
            return;
        }

        LPanelClear();
    }

    private void LPanelScribeOpen()
    {
        if (!LPanelBinEnabled)
        {
            LPanelClear();
            return;
        }

        if (_lPanelVista?.LVistaChosen is long shown)
        {
            LPanelEdited?.Invoke(shown);
        }

        LPanelScribeShow(true);
    }

    public void LPanelScribeShow(bool editing)
    {
        _lPanelVista?.LVistaEditingSet(editing);
        LPanelChanged?.Invoke();
    }

    public void LPanelRowSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!LPanelLeaveConfirm())
        {
            return;
        }

        LPanelRowShow(id);
    }

    public long LPanelVoyageRead()
    {
        return _lPanelVista?.LVistaChosen ?? 0;
    }

    public bool LPanelRowShow(long? id)
    {
        return LPanelDraftShow(() => _lPanelVista?.LVistaLoad(id));
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
            LPanelFailed?.Invoke(_lPanelLoadKey, exception);
            return false;
        }

        LPanelRowsUpdate();
        if (!LPanelBinEnabled)
        {
            LPanelClear();
            return false;
        }

        if (draft is LDraft loaded)
        {
            LPanelDraftChanged?.Invoke(loaded);
            LPanelChanged?.Invoke();
            if (LPanelEditing)
            {
                if (_lPanelVista?.LVistaChosen is long shown)
                {
                    LPanelEdited?.Invoke(shown);
                }
            }
        }

        return true;
    }

    public void LPanelDraftUpdate()
    {
        LDraft? draft;
        try
        {
            draft = _lPanelVista?.LVistaLoad();
        }
        catch (Exception)
        {
            return;
        }

        if (!LPanelBinEnabled)
        {
            LPanelClear();
            return;
        }

        if (draft is LDraft loaded)
        {
            LPanelDraftChanged?.Invoke(loaded);
            LPanelChanged?.Invoke();
        }
    }

    public void LPanelEntryHandle(LBulletin bulletin)
    {
        LPanelEntrySelect(bulletin);
        LPanelChanged?.Invoke();
        LPanelRowsUpdate();
    }

    public void LPanelEntrySelect(LBulletin bulletin)
    {
        ArgumentNullException.ThrowIfNull(bulletin);

        if (!bulletin.LBulletinStored)
        {
            return;
        }

        if (LPanelBinEnabled)
        {
            return;
        }

        if (!LPanelEditing)
        {
            return;
        }

        if (!LPanelShownCheck())
        {
            return;
        }

        _lPanelVista?.LVistaSelect(bulletin.LBulletinId);
    }

    public void LPanelDelete()
    {
        if (!LPanelDeleteConfirm())
        {
            return;
        }

        try
        {
            _lPanelVista?.LVistaDelete();
        }
        catch (Exception exception)
        {
            LPanelFailed?.Invoke(_lPanelDeleteKey, exception);
            return;
        }

        LPanelClear();
    }
}
