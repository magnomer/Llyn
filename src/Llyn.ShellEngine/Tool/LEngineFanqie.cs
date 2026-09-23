using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LFanqieBook> LEngineBookRead(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffFanqie.LFanqieBookRead(language);
        }
    }

    public bool LEngineBookCheck(string language)
    {
        return LEngineBookRead(language).Count > 0;
    }

    public string? LEngineBookFind()
    {
        foreach (string language in LEngineLanguageRead())
        {
            if (LEngineBookCheck(language))
            {
                return language;
            }
        }

        return null;
    }

    public LHypothesis? LEngineHypothesisRead(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffFanqie.LHypothesisRead(language);
        }
    }

    public IReadOnlyList<LFanqieRow> LEngineFanqieRead(long entryId)
    {
        return _lEngineStaff.LEngineStaffFanqie.LFanqieClerkRead(entryId);
    }

    public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId)
    {
        return _lEngineStaff.LEngineStaffFanqie.LFanqieClerkDivide(entryId);
    }

    public void LEngineFanqieStart(long entryId)
    {
        _lEngineStaff.LEngineStaffFanqie.LFanqieClerkStart(entryId);
        _lEngineStaff.LEngineStaffShengfu.LShengfuClerkStart(entryId);
    }

    public void LEngineFanqieRebuild(long entryId)
    {
        _lEngineStaff.LEngineStaffFanqie.LFanqieClerkRebuild(entryId);
        _lEngineStaff.LEngineStaffShengfu.LShengfuClerkRebuild(entryId);
    }

    public void LEngineFanqieSet(long entryId, long fanqieId, int rank)
    {
        _lEngineStaff.LEngineStaffFanqie.LFanqieClerkSet(entryId, fanqieId, rank);
    }

    public bool LEngineFanqieCheck(long entryId)
    {
        return _lEngineStaff.LEngineStaffFanqie.LFanqieClerkCheck(entryId)
            || _lEngineStaff.LEngineStaffShengfu.LShengfuClerkCheck(entryId);
    }

    internal Task<IReadOnlyList<LFanqieRow>> LEngineFanqieFind(
        string character, string language, CancellationToken cancellation)
    {
        return _lEngineStaff.LEngineStaffFanqie.LFanqieClerkFind(character, language, cancellation);
    }

    internal IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffDiwei.LDiweiClerkRead(language, kind);
        }
    }

    public LDiwei? LEngineDiweiRead(long? id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffDiwei.LDiweiClerkRead(id);
        }
    }

    public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(localize);

        lock (LEngineGate)
        {
            LDiwei? diwei = _lEngineStaff.LEngineStaffDiwei.LDiweiClerkRead(id);
            if (diwei is null)
            {
                return LDiweiPage.LDiweiPageBlank;
            }

            return _lEngineStaff.LEngineStaffDiwei.LDiweiPageRead(
                diwei,
                LEngineRespellingCheck(diwei.LDiweiLanguage),
                LEngineSettingsHeld.LSettingsTally,
                localize);
        }
    }

    public LDiwei? LEngineDiweiFind(string language, string kind, string key)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffDiwei.LDiweiClerkFind(language, kind, key);
        }
    }

    public IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, string language, string kind)
    {
        ArgumentNullException.ThrowIfNull(vista);

        string wanted = vista.LVistaQuery.Trim();
        IEnumerable<LDiwei> kept = LEngineDiweiRead(language, kind).Where(row =>
            wanted.Length == 0 || row.LDiweiKey.Contains(wanted, StringComparison.OrdinalIgnoreCase));
        IReadOnlyList<LDiwei> sorted = vista.LVistaOrder switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. kept
                .OrderByDescending(row => row.LDiweiKey, StringComparer.Ordinal)],
            LCatalogOrder.LCatalogOrderUsage => [.. kept
                .OrderByDescending(row => row.LDiweiCount)
                .ThenBy(row => row.LDiweiKey, StringComparer.Ordinal)],
            _ => [.. kept.OrderBy(row => row.LDiweiKey, StringComparer.Ordinal)],
        };
        if (vista.LVistaChosen is long chosen && LDiwei.LDiweiFind(sorted, chosen) is null)
        {
            vista.LVistaSelect(null);
        }

        return [.. sorted.Select(row => row with { LDiweiChosen = vista.LVistaMatch(row.LDiweiId) })];
    }

    public IReadOnlyList<LFanqieRow> LEngineFanqieRead(LDiwei diwei)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffDiwei.LDiweiFanqieRead(diwei);
        }
    }

    internal IReadOnlyList<long> LEngineDiweiScan(string language, IReadOnlyList<long> diweiIds)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffDiwei.LDiweiClerkScan(language, diweiIds);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(
        string language, IReadOnlyList<long> diweiIds, string query, LVista? vista = null)
    {
        lock (LEngineGate)
        {
            IReadOnlyList<LEntry> entries = _lEngineStaff.LEngineStaffDiwei.LDiweiEntryScan(language, diweiIds, query);
            return entries.Count == 0 ? [] : LEngineVistaBuild(entries, vista?.LVistaChosen);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(string language, LVista onset, LVista rime, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(onset);
        ArgumentNullException.ThrowIfNull(rime);
        ArgumentNullException.ThrowIfNull(vista);

        List<long> wanted = [];
        if (onset.LVistaChosen is long initial)
        {
            wanted.Add(initial);
        }

        if (rime.LVistaChosen is long final)
        {
            wanted.Add(final);
        }

        if (wanted.Count == 0)
        {
            return [];
        }

        return LEngineXiaoyunFind(language, wanted, vista.LVistaQuery.Trim(), vista);
    }

    internal void LEngineDiweiRebuild()
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffFanqie.LDiweiApply();
        }
    }

    public IReadOnlyList<LTally> LEngineTallyRead(LDiwei diwei)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffDiwei.LTallyRead(diwei);
        }
    }
}
