using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LEntryQueryClerk
{
    private readonly LEntryVault _lEntryQueryEntries;

    public LEntryQueryClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lEntryQueryEntries = rig.LRigEntries;
    }

    public IReadOnlyList<LEntry> LEntryFind(string query)
    {
        return _lEntryQueryEntries.LEntryFind(query);
    }

    public IReadOnlyList<LEntry> LEntryHeadwordFind(string headword, string language)
    {
        ArgumentNullException.ThrowIfNull(headword);
        ArgumentNullException.ThrowIfNull(language);
        return _lEntryQueryEntries.LEntryHeadwordFind(language.Trim(), headword.Trim());
    }

    public IReadOnlyList<LEntry> LEntryFind(string query, LCatalogOrder order)
    {
        return LCatalogEntry.LCatalogEntrySort(_lEntryQueryEntries.LEntryFind(query), order);
    }

    public IReadOnlyList<LEntry> LEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return filter.LCatalogFilterApply(LEntryFind(query, order), LEntryLanguageRead);
    }

    private static string? LEntryLanguageRead(LEntry entry)
    {
        return entry.LEntryLanguage;
    }

    public IReadOnlyList<LEntry> LEntryFind(LTag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        return _lEntryQueryEntries.LEntryTagFind(tag.LTagId);
    }

    public IReadOnlyList<LEntry> LEntryFind(LRegister register)
    {
        ArgumentNullException.ThrowIfNull(register);
        return _lEntryQueryEntries.LEntryRegisterFind(register.LRegisterId);
    }

    public IReadOnlyList<LEntry> LEntryFind(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);
        return _lEntryQueryEntries.LEntrySituationFind(situation.LSituationId);
    }

    public IReadOnlyList<LEntry> LEntryFind(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        return _lEntryQueryEntries.LEntryExampleFind(example.LExampleId);
    }

    public IReadOnlyList<LEntry> LEntryFind(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        return _lEntryQueryEntries.LEntryReferenceFind(reference.LReferenceId);
    }

    public static IReadOnlyList<LEntry> LEntryMatch(IReadOnlyList<LEntry> entries, string query)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(query);

        string trimmed = query.Trim();
        return trimmed.Length == 0
            ? entries
            : [.. entries.Where(entry => LCatalog.LCatalogTextMatch(entry.LEntryHeadword, trimmed))];
    }

    public static IReadOnlyList<LEntry> LEntryMatch(
        IReadOnlyList<LEntry> entries, LCatalogFilter filter, string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return LCatalogEntry.LCatalogEntrySort(
            LEntryMatch(filter.LCatalogFilterApply(entries, LEntryLanguageRead), query), order);
    }

    public IReadOnlyDictionary<long, string> LEntryEpithetScan(IReadOnlyList<long> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return _lEntryQueryEntries.LEntryEpithetScan(ids);
    }

    public long LEntryCountRead()
    {
        return _lEntryQueryEntries.LEntryCountRead();
    }
}
