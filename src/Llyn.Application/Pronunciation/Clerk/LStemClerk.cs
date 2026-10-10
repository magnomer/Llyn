using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LStemClerk
{
    private readonly LStemVault _lStemClerkStems;
    private readonly LEntryQueryVault _lStemClerkEntries;
    private readonly LFanqieVault _lStemClerkFanqie;
    private readonly LReflexVault _lStemClerkReflexes;
    private readonly LReflexClerk _lStemClerkReflex;
    private readonly LFoldVault _lStemClerkFolds;

    public LStemClerk(LRig rig, LReflexClerk reflex)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(reflex);
        _lStemClerkStems = rig.LRigSound.LRigSoundStems;
        _lStemClerkEntries = rig.LRigEntryQuery;
        _lStemClerkFanqie = rig.LRigSound.LRigSoundFanqie;
        _lStemClerkReflexes = rig.LRigSound.LRigSoundReflexes;
        _lStemClerkReflex = reflex;
        _lStemClerkFolds = rig.LRigKeeping.LRigKeepingFolds;
    }

    public IReadOnlyList<LStem> LStemClerkRead(string language)
    {
        return _lStemClerkStems.LStemRead(language);
    }

    public LStem? LStemClerkRead(long? id)
    {
        return id is long wanted ? _lStemClerkStems.LStemRead(wanted) : null;
    }

    public LStem? LStemClerkFind(string language, string key)
    {
        return _lStemClerkStems.LStemFind(language, key);
    }

    public IReadOnlyList<LStem> LStemClerkFind(string language, string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(query);

        return LCatalogClerk.LCatalogClerkFind(
            LStemClerkRead(language), query, order, row => row.LStemKey, row => row.LStemCount);
    }

    public IReadOnlyList<LEntry> LStemEntryScan(
        string language, IReadOnlyList<long> stemIds, string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(stemIds);
        ArgumentNullException.ThrowIfNull(query);

        IReadOnlyList<long> ids = _lStemClerkStems.LStemEntryScan(language, stemIds);
        return ids.Count == 0
            ? []
            : LCatalogEntry.LCatalogEntrySort(_lStemClerkEntries.LEntryScan(ids, query), order);
    }

    public LStemPage LStemPageRead(LStem stem, bool fold)
    {
        ArgumentNullException.ThrowIfNull(stem);

        IReadOnlyList<string> characters =
            LGlyphOrder.LGlyphOrderSort(_lStemClerkStems.LStemCharacterRead(stem.LStemId));
        return new LStemPage(stem.LStemLanguage, stem.LStemKey, characters, LStemMemberScan(stem, characters, fold));
    }

    public long? LStemEntryFind(LStem stem, string character)
    {
        ArgumentNullException.ThrowIfNull(stem);
        ArgumentNullException.ThrowIfNull(character);

        return LStemMember.LStemMemberFind(LStemEntryRead(stem), character);
    }

    private IReadOnlyList<LStemMember> LStemMemberScan(LStem stem, IReadOnlyList<string> characters, bool fold)
    {
        if (characters.Count == 0 || string.IsNullOrWhiteSpace(stem.LStemLanguage))
        {
            return LStemMember.LStemBareScan(characters);
        }

        IReadOnlyList<LEntry> entries = LStemEntryRead(stem);
        return characters
            .Select(character =>
                LStemMemberRead(stem, character, LStemMember.LStemMemberFind(entries, character), fold))
            .ToList();
    }

    private IReadOnlyList<LEntry> LStemEntryRead(LStem stem)
    {
        if (string.IsNullOrWhiteSpace(stem.LStemLanguage))
        {
            return [];
        }

        IReadOnlyList<long> ids = _lStemClerkStems.LStemEntryScan(stem.LStemLanguage, [stem.LStemId]);
        return ids.Count == 0 ? [] : _lStemClerkEntries.LEntryScan(ids, string.Empty);
    }

    private LStemMember LStemMemberRead(LStem stem, string character, long? entry, bool fold)
    {
        string language = stem.LStemLanguage;
        IReadOnlyList<LReflexDraft> reflexes = entry is long id
            ? LReflexClerk.LReflexWrittenScan(_lStemClerkReflex.LReflexClerkSort(
                language, LReflexClerk.LReflexClerkScan(_lStemClerkReflexes.LReflexRead(id))))
            : [];
        return new LStemMember(
            character,
            LFanqieGroup.LFanqieMarkedScan(_lStemClerkFanqie.LFanqieRead(language, character), character),
            reflexes)
        {
            LStemMemberOpened = fold && entry is long held && _lStemClerkFolds.LFoldStemCheck(held, stem.LStemKey),
        };
    }
}
