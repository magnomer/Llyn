using System;
using System.Collections.Generic;
using Llyn.Application;
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

    public LStem? LEngineStemRead(long? id)
    {
        lock (_lStemFacadeGate)
        {
            return LStemFacadeStaff.LEngineStaffStem.LStemClerkRead(id);
        }
    }

    public long? LEngineStemFind(string language, string? key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        lock (_lStemFacadeGate)
        {
            return LStemFacadeStaff.LEngineStaffStem.LStemClerkFind(language, key)?.LStemId;
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

    public bool LEngineStemCheck()
    {
        return LEngineStemFind() is not null;
    }

    public IReadOnlyList<LStem> LEngineStemFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        if (!LEngineStemCheck())
        {
            return [];
        }

        string language = LEngineLanguageRead(vista.LVistaChosen);
        LStemClerk stems;
        lock (_lStemFacadeGate)
        {
            stems = LStemFacadeStaff.LEngineStaffStem;
        }

        List<LStem> rows = [];
        bool kept = false;
        foreach (LStem row in stems.LStemClerkFind(language, vista.LVistaQuery, vista.LVistaOrder))
        {
            bool chosen = vista.LVistaMatch(row.LStemId);
            kept |= chosen;
            rows.Add(row with { LStemChosen = chosen });
        }

        if (!kept)
        {
            vista.LVistaSelect(null);
        }

        return rows;
    }

    public LStemPage LEngineStemResolve(long? id)
    {
        lock (_lStemFacadeGate)
        {
            LStem? stem = LStemFacadeStaff.LEngineStaffStem.LStemClerkRead(id);
            return stem is null ? LStemPage.LStemPageBlank : LStemFacadeStaff.LEngineStaffStem.LStemPageRead(stem);
        }
    }

    public long LEngineStemResolve(long? id, string character)
    {
        LStem? stem;
        lock (_lStemFacadeGate)
        {
            stem = LStemFacadeStaff.LEngineStaffStem.LStemClerkRead(id);
        }

        return _lStemFacadeEngine.LEngineEntry.LEngineGlyphResolve(
            character, stem?.LStemLanguage ?? LStemPage.LStemPageBlank.LStemPageLanguage).LEntryId;
    }

    public IReadOnlyList<LVistaRow> LEngineKindredFind(LVista grove, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(grove);
        ArgumentNullException.ThrowIfNull(vista);

        if (grove.LVistaChosen is not long chosen)
        {
            return [];
        }

        return LEngineKindredFind(LEngineLanguageRead(chosen), [chosen], vista.LVistaQuery.Trim(), vista);
    }

    internal IReadOnlyList<LVistaRow> LEngineKindredFind(
        string language, IReadOnlyList<long> stemIds, string query, LVista? vista = null)
    {
        lock (_lStemFacadeGate)
        {
            IReadOnlyList<LEntry> entries = LStemFacadeStaff.LEngineStaffStem.LStemEntryScan(
                language, stemIds, query, vista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword);
            return entries.Count == 0
                ? []
                : _lStemFacadeEngine.LEngineVista.LEngineVistaBuild(entries, vista?.LVistaChosen);
        }
    }

    private string LEngineLanguageRead(long? chosen)
    {
        return LEngineStemRead(chosen)?.LStemLanguage ?? LEngineStemFind() ?? string.Empty;
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
