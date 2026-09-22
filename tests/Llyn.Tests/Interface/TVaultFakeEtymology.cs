using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeEtymology : LEtymologyVault
{
    private readonly Dictionary<long, LEtymology> _tVaultFakeNarratives = [];
    private readonly Dictionary<long, IReadOnlyList<LEtymon>> _tVaultFakeLinks = [];

    public LEtymology? LEtymologyRead(long entryId) =>
        _tVaultFakeNarratives.TryGetValue(entryId, out LEtymology? held) ? held : null;

    public IReadOnlyList<LEtymon> LEtymologyEtymonRead(long entryId) =>
        _tVaultFakeLinks.TryGetValue(entryId, out IReadOnlyList<LEtymon>? held) ? held : [];

    public LEtymology? LEtymologySave(long entryId, LEtymology? etymology)
    {
        if (etymology is null || etymology.LEtymologyText.Trim().Length == 0)
        {
            _tVaultFakeNarratives.Remove(entryId);
            return null;
        }

        List<LMention> mentions = [];
        foreach (LMention mention in etymology.LEtymologyMentions)
        {
            mentions.Add(mention with { LMentionId = mentions.Count + 1 });
        }

        LEtymology stored = new(entryId, entryId, etymology.LEtymologyText, mentions);
        _tVaultFakeNarratives[entryId] = stored;
        return stored;
    }

    public IReadOnlyList<LEtymon> LEtymologyEtymonSet(long entryId, IReadOnlyList<long> targetIds)
    {
        List<LEtymon> stored = [];
        foreach (long targetId in targetIds)
        {
            stored.Add(new LEtymon(stored.Count + 1, entryId, stored.Count, targetId));
        }

        _tVaultFakeLinks[entryId] = stored;
        return stored;
    }

    public IReadOnlyList<LEntry> LEtymologySourceScan(long entryId) => [];
}
