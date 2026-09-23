using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LStemFacade
{
    private readonly LEngine _lStemFacadeEngine;
    private readonly object _lStemFacadeGate;

    public LStemFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lStemFacadeEngine = engine;
        _lStemFacadeGate = engine.LEngineGate;
    }

    internal IReadOnlyList<LStem> LEngineStemRead(string language)
    {
        lock (_lStemFacadeGate)
        {
            return LStemFacadeStaff.LEngineStaffStem.LStemClerkRead(language);
        }
    }

    public LStem? LEngineStemRead(long? id)
    {
        lock (_lStemFacadeGate)
        {
            return LStemFacadeStaff.LEngineStaffStem.LStemClerkRead(id);
        }
    }

    public LStem? LEngineStemFind(string language, string key)
    {
        lock (_lStemFacadeGate)
        {
            return LStemFacadeStaff.LEngineStaffStem.LStemClerkFind(language, key);
        }
    }

    public string? LEngineStemFind()
    {
        foreach (string language in _lStemFacadeEngine.LEngineLanguage.LEngineLanguageRead())
        {
            if (LEngineStemCheck(language))
            {
                return language;
            }
        }

        return null;
    }

    public IReadOnlyList<LStem> LEngineStemFind(LVista vista, string language)
    {
        ArgumentNullException.ThrowIfNull(vista);

        string wanted = vista.LVistaQuery.Trim();
        IEnumerable<LStem> kept = LEngineStemRead(language).Where(row =>
            wanted.Length == 0 || row.LStemKey.Contains(wanted, StringComparison.OrdinalIgnoreCase));
        IReadOnlyList<LStem> sorted = vista.LVistaOrder switch
        {
            LCatalogOrder.LCatalogOrderReverse => [.. kept
                .OrderByDescending(row => row.LStemKey, StringComparer.Ordinal)],
            LCatalogOrder.LCatalogOrderUsage => [.. kept
                .OrderByDescending(row => row.LStemCount)
                .ThenBy(row => row.LStemKey, StringComparer.Ordinal)],
            _ => [.. kept.OrderBy(row => row.LStemKey, StringComparer.Ordinal)],
        };
        if (vista.LVistaChosen is long chosen && LStem.LStemFind(sorted, chosen) is null)
        {
            vista.LVistaSelect(null);
        }

        return [.. sorted.Select(row => row with { LStemChosen = vista.LVistaMatch(row.LStemId) })];
    }

    public LStemPage LEngineStemResolve(long? id)
    {
        lock (_lStemFacadeGate)
        {
            LStem? stem = LStemFacadeStaff.LEngineStaffStem.LStemClerkRead(id);
            return stem is null ? LStemPage.LStemPageBlank : LStemFacadeStaff.LEngineStaffStem.LStemPageRead(stem);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineKindredFind(string language, LVista grove, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(grove);
        ArgumentNullException.ThrowIfNull(vista);

        if (grove.LVistaChosen is not long chosen)
        {
            return [];
        }

        return LEngineKindredFind(language, [chosen], vista.LVistaQuery.Trim(), vista);
    }

    internal IReadOnlyList<LVistaRow> LEngineKindredFind(
        string language, IReadOnlyList<long> stemIds, string query, LVista? vista = null)
    {
        lock (_lStemFacadeGate)
        {
            IReadOnlyList<LEntry> entries = LStemFacadeStaff.LEngineStaffStem.LStemEntryScan(language, stemIds, query);
            return entries.Count == 0
                ? []
                : _lStemFacadeEngine.LEngineVista.LEngineVistaBuild(entries, vista?.LVistaChosen);
        }
    }

    private bool LEngineStemCheck(string language)
    {
        lock (_lStemFacadeGate)
        {
            return LStemFacadeStaff.LEngineStaffShengfu.LShengfuRuleRead(language) is not null;
        }
    }

    private LEngineStaff LStemFacadeStaff => _lStemFacadeEngine.LEngineStaffHeld;
}
