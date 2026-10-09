using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LStemFacade : LStemPort
{
    private readonly LEngineHearth _lStemFacadeHearth;
    private readonly LEntryFacade _lStemFacadeEntry;
    private readonly LLanguageFacade _lStemFacadeLanguage;
    private readonly LVistaRowFacade _lStemFacadeRow;
    private readonly object _lStemFacadeGate;

    internal LStemFacade(LEngineHearth hearth, LEntryFacade entry, LLanguageFacade language, LVistaRowFacade row)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(row);
        _lStemFacadeHearth = hearth;
        _lStemFacadeEntry = entry;
        _lStemFacadeLanguage = language;
        _lStemFacadeRow = row;
        _lStemFacadeGate = _lStemFacadeHearth.LEngineGate;
    }

    public LStem? LEngineStemRead(long? id)
    {
        lock (_lStemFacadeGate)
        {
            return LStemFacadeStaff.LEngineStaffLanguage.LLanguageStaffStem.LStemClerkRead(id);
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
            return LStemFacadeStaff.LEngineStaffLanguage.LLanguageStaffStem.LStemClerkFind(language, key)?.LStemId;
        }
    }

    public string? LEngineStemFind()
    {
        foreach (string language in _lStemFacadeLanguage.LEngineLanguageRead())
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
            stems = LStemFacadeStaff.LEngineStaffLanguage.LLanguageStaffStem;
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
            LStem? stem = LStemFacadeStaff.LEngineStaffLanguage.LLanguageStaffStem.LStemClerkRead(id);
            return stem is null
                ? LStemPage.LStemPageBlank
                : LStemFacadeStaff.LEngineStaffLanguage.LLanguageStaffStem.LStemPageRead(stem);
        }
    }

    public long LEngineStemResolve(long? id, string character)
    {
        LStem? stem;
        lock (_lStemFacadeGate)
        {
            stem = LStemFacadeStaff.LEngineStaffLanguage.LLanguageStaffStem.LStemClerkRead(id);
        }

        return _lStemFacadeEntry.LEngineGlyphResolve(
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
            IReadOnlyList<LEntry> entries = LStemFacadeStaff.LEngineStaffLanguage.LLanguageStaffStem.LStemEntryScan(
                language, stemIds, query, vista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword);
            return entries.Count == 0
                ? []
                : _lStemFacadeRow.LEngineVistaBuild(entries, vista?.LVistaChosen);
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
            return LStemFacadeStaff.LEngineStaffLanguage.LLanguageStaffShengfu.LShengfuRuleRead(language) is not null;
        }
    }

    private LEngineStaff LStemFacadeStaff => _lStemFacadeHearth.LEngineStaffHeld;
}
