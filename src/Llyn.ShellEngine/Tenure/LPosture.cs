using System;
using System.Collections.Generic;
using Llyn.Application;
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

    private double _lPostureStored = 1;

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

    public LVista LPostureVistaStart(string tab, LSubject? subject, LCatalogOrder fallback, bool blank = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tab);

        LPostureState state = LPostureRead();
        LLayout? record = LPostureLayoutFind(state, tab);
        LCatalogOrder order = record?.LLayoutOrder ?? fallback;
        LCatalogFilter filter = record?.LLayoutFilter ?? LCatalogFilter.LCatalogFilterEmpty;

        LVista vista = _lEngine.LEngineVista.LEngineVistaStart(
            tab, subject, order, filter, state.LPostureStateSplit, blank);
        vista.LVistaEditingSaved += LPostureSplitSave;
        lock (_lPostureGate)
        {
            _lPostureVistas[vista.LVistaId] = LPostureVistaRead(vista);
            _lPostureWatched.Add(vista);
        }

        return vista;
    }

    public void LPostureVolumeSet(double volume)
    {
        if (!double.IsFinite(volume))
        {
            return;
        }

        lock (_lPostureGate)
        {
            _lPostureState = _lPostureState with { LPostureStateVolume = Math.Clamp(volume, 0, 1) };
        }
    }

    public void LPostureVolumeSave()
    {
        lock (_lPostureGate)
        {
            if (_lPostureState.LPostureStateVolume == _lPostureStored)
            {
                return;
            }

            LPostureSave();
        }
    }

    public void LPostureModeSave(string mode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mode);
        LPostureChange(state => state with { LPostureStateMode = mode });
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
        if (bulletin.LBulletinSubject == LSubject.LSubjectWorkspace)
        {
            LPostureLoad();
            return;
        }

        if (bulletin.LBulletinSubject != LSubject.LSubjectVista ||
            _lEngine.LEngineVista.LEngineVistaRead(bulletin.LBulletinId) is not LVista vista)
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
        return (vista.LVistaOrder, LCatalogClerk.LCatalogClerkFormat(vista.LVistaFilter));
    }

    private static LLayout? LPostureLayoutFind(LPostureState state, string tab)
    {
        foreach (LLayout record in state.LPostureStateLayout ?? [])
        {
            if (string.Equals(record.LLayoutTab, tab, StringComparison.Ordinal))
            {
                return record;
            }
        }

        return null;
    }

    private static bool LPostureLayoutMatch(LLayout held, LLayout next)
    {
        return held.LLayoutOrder == next.LLayoutOrder
            && string.Equals(
                LCatalogClerk.LCatalogClerkFormat(held.LLayoutFilter),
                LCatalogClerk.LCatalogClerkFormat(next.LLayoutFilter),
                StringComparison.Ordinal);
    }

    private void LPostureLoad()
    {
        lock (_lPostureGate)
        {
            if (!_lEngine.LEngineSettings.LEnginePostureLoad(LPostureName, out LPostureState? kept))
            {
                return;
            }

            if (kept is not null)
            {
                _lPostureState = kept;
                _lPostureStored = kept.LPostureStateVolume;
                return;
            }

            if (!_lEngine.LEngineSettings.LEnginePostureLoad(LPostureLegacy, out LPostureState? legacy))
            {
                return;
            }

            if (legacy is not null && legacy != new LPostureState())
            {
                _lPostureState = legacy;
            }

            LPostureSave();
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
            LPostureSave();
            return true;
        }
    }

    private void LPostureSave()
    {
        _lEngine.LEngineSettings.LEnginePostureSave(LPostureName, _lPostureState);
        _lPostureStored = _lPostureState.LPostureStateVolume;
    }
}
