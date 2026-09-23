using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LFanqieFacade
{
    private readonly LEngine _lFanqieFacadeEngine;
    private readonly object _lFanqieFacadeGate;

    public LFanqieFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lFanqieFacadeEngine = engine;
        _lFanqieFacadeGate = engine.LEngineGate;
    }

    public IReadOnlyList<LFanqieBook> LEngineBookRead(string language)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieBookRead(language);
        }
    }

    public bool LEngineBookCheck(string language)
    {
        return LEngineBookRead(language).Count > 0;
    }

    public string? LEngineBookFind()
    {
        foreach (string language in _lFanqieFacadeEngine.LEngineLanguage.LEngineLanguageRead())
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
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffFanqie.LHypothesisRead(language);
        }
    }

    public IReadOnlyList<LFanqieRow> LEngineFanqieRead(long entryId)
    {
        return LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkRead(entryId);
    }

    public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId)
    {
        return LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkDivide(entryId);
    }

    public void LEngineFanqieStart(long entryId)
    {
        LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkStart(entryId);
        LFanqieFacadeStaff.LEngineStaffShengfu.LShengfuClerkStart(entryId);
    }

    public void LEngineFanqieRebuild(long entryId)
    {
        LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkRebuild(entryId);
        LFanqieFacadeStaff.LEngineStaffShengfu.LShengfuClerkRebuild(entryId);
    }

    public void LEngineFanqieSet(long entryId, long fanqieId, int rank)
    {
        LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkSet(entryId, fanqieId, rank);
    }

    public bool LEngineFanqieCheck(long entryId)
    {
        return LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkCheck(entryId)
            || LFanqieFacadeStaff.LEngineStaffShengfu.LShengfuClerkCheck(entryId);
    }

    internal Task<IReadOnlyList<LFanqieRow>> LEngineFanqieFind(
        string character, string language, CancellationToken cancellation)
    {
        return LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkFind(character, language, cancellation);
    }

    internal IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiClerkRead(language, kind);
        }
    }

    public LDiwei? LEngineDiweiRead(long? id)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiClerkRead(id);
        }
    }

    public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(localize);

        lock (_lFanqieFacadeGate)
        {
            LDiwei? diwei = LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiClerkRead(id);
            if (diwei is null)
            {
                return LDiweiPage.LDiweiPageBlank;
            }

            return LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiPageRead(
                diwei,
                _lFanqieFacadeEngine.LEngineSettings.LEngineRespellingCheck(diwei.LDiweiLanguage),
                _lFanqieFacadeEngine.LEngineSettingsHeld.LSettingsTally,
                localize);
        }
    }

    public LDiwei? LEngineDiweiFind(string language, string kind, string key)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiClerkFind(language, kind, key);
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
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiFanqieRead(diwei);
        }
    }

    internal IReadOnlyList<long> LEngineDiweiScan(string language, IReadOnlyList<long> diweiIds)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiClerkScan(language, diweiIds);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(
        string language, IReadOnlyList<long> diweiIds, string query, LVista? vista = null)
    {
        lock (_lFanqieFacadeGate)
        {
            IReadOnlyList<LEntry> entries =
                LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiEntryScan(language, diweiIds, query);
            return entries.Count == 0
                ? []
                : _lFanqieFacadeEngine.LEngineVista.LEngineVistaBuild(entries, vista?.LVistaChosen);
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
        lock (_lFanqieFacadeGate)
        {
            LFanqieFacadeStaff.LEngineStaffFanqie.LDiweiApply();
        }
    }

    public IReadOnlyList<LTally> LEngineTallyRead(LDiwei diwei)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffDiwei.LTallyRead(diwei);
        }
    }

    private LEngineStaff LFanqieFacadeStaff => _lFanqieFacadeEngine.LEngineStaffHeld;
}
