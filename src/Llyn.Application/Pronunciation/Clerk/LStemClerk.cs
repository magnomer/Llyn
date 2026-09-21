using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LStemClerk
{
    private readonly LStemVault _lStemClerkStems;
    private readonly LEntryVault _lStemClerkEntries;

    public LStemClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lStemClerkStems = rig.LRigStems;
        _lStemClerkEntries = rig.LRigEntries;
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

    public IReadOnlyList<long> LStemClerkScan(string language, IReadOnlyList<long> stemIds)
    {
        ArgumentNullException.ThrowIfNull(stemIds);
        return _lStemClerkStems.LStemEntryScan(language, stemIds);
    }

    public IReadOnlyList<LEntry> LStemEntryScan(string language, IReadOnlyList<long> stemIds, string query)
    {
        ArgumentNullException.ThrowIfNull(stemIds);
        ArgumentNullException.ThrowIfNull(query);

        IReadOnlyList<long> ids = _lStemClerkStems.LStemEntryScan(language, stemIds);
        return ids.Count == 0 ? [] : _lStemClerkEntries.LEntryScan(ids, query);
    }

    public LStemPage LStemPageRead(LStem stem)
    {
        ArgumentNullException.ThrowIfNull(stem);

        return new LStemPage(
            stem.LStemLanguage, stem.LStemKey, _lStemClerkStems.LStemCharacterRead(stem.LStemId));
    }
}
