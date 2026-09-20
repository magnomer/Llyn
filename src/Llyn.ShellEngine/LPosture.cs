using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LPosture : IDisposable
{
    private const string LPostureName = "posture";
    private const string LPostureLegacy = "settings";

    private readonly LEngine _lEngine;

    private readonly object _lPostureGate = new();

    private readonly Dictionary<long, (LCatalogOrder LPostureOrder, string LPostureFilter)> _lPostureVistas = [];

    private readonly List<LVista> _lPostureWatched = [];

    private LPostureState _lPostureState = new();

    public LPosture(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _lEngine = engine;
        LPostureLoad();
        _lEngine.LEngineObserverAttach(LPostureBulletinHandle);
    }

    public LPostureState LPostureRead()
    {
        lock (_lPostureGate)
        {
            return _lPostureState;
        }
    }

    public bool LPostureModeMatch(string? mode)
    {
        return string.Equals(LPostureRead().LPostureStateMode, mode, StringComparison.Ordinal);
    }

    public bool LPostureVolumeMatch(double volume)
    {
        return LPostureRead().LPostureStateVolume == volume;
    }

    public LVista LPostureVistaStart(string tab, LSubject? subject, LCatalogOrder fallback, bool blank = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tab);

        LCatalogOrder order = fallback;
        LCatalogFilter filter = LCatalogFilter.LCatalogFilterEmpty;
        LPostureState state = LPostureRead();
        foreach (LLayout record in state.LPostureStateLayout ?? [])
        {
            if (!record.LLayoutTabMatch(tab))
            {
                continue;
            }

            order = record.LLayoutOrder ?? fallback;
            filter = record.LLayoutFilter ?? LCatalogFilter.LCatalogFilterEmpty;
            break;
        }

        LVista vista = _lEngine.LEngineVistaStart(tab, subject, order, filter, state.LPostureStateSplit, blank);
        vista.LVistaEditingSaved += LPostureSplitSave;
        lock (_lPostureGate)
        {
            _lPostureVistas[vista.LVistaId] = LPostureVistaRead(vista);
            _lPostureWatched.Add(vista);
        }

        return vista;
    }

    public void LPostureWindowSave(LWindowState window)
    {
        ArgumentNullException.ThrowIfNull(window);
        LPostureChange(state => state with { LPostureStateWindow = window });
    }

    public void LPostureVolumeSave(double volume)
    {
        double level = Math.Clamp(volume, 0, 1);
        LPostureChange(state => state with { LPostureStateVolume = level });
    }

    public void LPostureModeSave(string mode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mode);
        LPostureChange(state => state with { LPostureStateMode = mode });
    }

    public bool LPostureLinkedSave(bool linked)
    {
        return LPostureChange(state => state with { LPostureStateLinked = linked });
    }

    public void LPostureLayoutSave(IEnumerable<LLayout> layout)
    {
        ArgumentNullException.ThrowIfNull(layout);

        lock (_lPostureGate)
        {
            Dictionary<string, LLayout> merged = new(StringComparer.Ordinal);

            foreach (LLayout tab in _lPostureState.LPostureStateLayout ?? [])
            {
                merged[tab.LLayoutTab] = tab;
            }

            bool moved = false;
            foreach (LLayout tab in layout)
            {
                LLayout next = merged.TryGetValue(tab.LLayoutTab, out LLayout? held)
                    ? held with
                    {
                        LLayoutLeft = tab.LLayoutLeft ?? held.LLayoutLeft,
                        LLayoutMiddle = tab.LLayoutMiddle ?? held.LLayoutMiddle,
                        LLayoutOrder = tab.LLayoutOrder ?? held.LLayoutOrder,
                        LLayoutFilter = tab.LLayoutFilter ?? held.LLayoutFilter,
                    }
                    : tab;
                moved |= held is null || !LPostureLayoutMatch(held, next);
                merged[tab.LLayoutTab] = next;
            }

            if (!moved)
            {
                return;
            }

            List<LLayout> list = [.. merged.Values];
            LPostureChange(state => state with { LPostureStateLayout = list });
        }
    }

    public void LPostureLayoutReset()
    {
        lock (_lPostureGate)
        {
            List<LLayout> list = [];

            foreach (LLayout tab in _lPostureState.LPostureStateLayout ?? [])
            {
                list.Add(tab with { LLayoutLeft = null, LLayoutMiddle = null });
            }

            LPostureChange(state => state with { LPostureStateLayout = list });
        }
    }

    public void Dispose()
    {
        _lEngine.LEngineObserverDetach(LPostureBulletinHandle);
        lock (_lPostureGate)
        {
            foreach (LVista vista in _lPostureWatched)
            {
                vista.LVistaEditingSaved -= LPostureSplitSave;
            }

            _lPostureWatched.Clear();
            _lPostureVistas.Clear();
        }
    }

    private void LPostureBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinMatch(LSubject.LSubjectWorkspace))
        {
            LPostureLoad();
            return;
        }

        if (!bulletin.LBulletinMatch(LSubject.LSubjectVista) ||
            _lEngine.LEngineVistaRead(bulletin.LBulletinId) is not LVista vista)
        {
            return;
        }

        LPostureVistaSave(vista);
    }

    private void LPostureVistaSave(LVista vista)
    {
        lock (_lPostureGate)
        {
            (LCatalogOrder LPostureOrder, string LPostureFilter) next = LPostureVistaRead(vista);
            if (_lPostureVistas.TryGetValue(
                    vista.LVistaId, out (LCatalogOrder LPostureOrder, string LPostureFilter) held)
                && held.LPostureOrder == next.LPostureOrder
                && string.Equals(held.LPostureFilter, next.LPostureFilter, StringComparison.Ordinal))
            {
                return;
            }

            _lPostureVistas[vista.LVistaId] = next;
            LPostureLayoutSave(
                [new LLayout(vista.LVistaTab, LLayoutOrder: vista.LVistaOrder, LLayoutFilter: vista.LVistaFilter)]);
        }
    }

    private void LPostureSplitSave(bool editing)
    {
        LPostureChange(state => state with { LPostureStateSplit = editing });
    }

    private static (LCatalogOrder LPostureOrder, string LPostureFilter) LPostureVistaRead(LVista vista)
    {
        return (vista.LVistaOrder, LCatalog.LCatalogFilterFormat(vista.LVistaFilter));
    }

    private static bool LPostureLayoutMatch(LLayout held, LLayout next)
    {
        return held.LLayoutLeft == next.LLayoutLeft
            && held.LLayoutMiddle == next.LLayoutMiddle
            && held.LLayoutOrder == next.LLayoutOrder
            && string.Equals(
                LCatalog.LCatalogFilterFormat(held.LLayoutFilter ?? LCatalogFilter.LCatalogFilterEmpty),
                LCatalog.LCatalogFilterFormat(next.LLayoutFilter ?? LCatalogFilter.LCatalogFilterEmpty),
                StringComparison.Ordinal);
    }

    private void LPostureLoad()
    {
        lock (_lPostureGate)
        {
            LPostureVault vault = _lEngine.LEnginePostureRead();
            LPostureState? kept;
            LPostureState? legacy;
            try
            {
                kept = vault.LPostureRead(LPostureName);
                legacy = kept is null ? vault.LPostureRead(LPostureLegacy) : null;
            }
            catch (LVaultFault exception)
            {
                _lEngine.LEngineAuditRecord(exception);
                return;
            }

            if (kept is not null)
            {
                _lPostureState = kept;
                return;
            }

            if (legacy is not null && legacy != new LPostureState())
            {
                _lPostureState = legacy;
            }

            LPostureSave(vault);
        }
    }

    private bool LPostureChange(Func<LPostureState, LPostureState> change)
    {
        lock (_lPostureGate)
        {
            LPostureState changed = change(_lPostureState);
            if (changed == _lPostureState)
            {
                return false;
            }

            _lPostureState = changed;
            LPostureSave(_lEngine.LEnginePostureRead());
            return true;
        }
    }

    private void LPostureSave(LPostureVault vault)
    {
        try
        {
            vault.LPostureSave(LPostureName, _lPostureState);
        }
        catch (LVaultFault exception)
        {
            _lEngine.LEngineAuditRecord(exception);
        }
    }
}
