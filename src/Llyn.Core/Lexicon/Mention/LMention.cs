using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LMention(
    long LMentionId,
    int LMentionOffset,
    int LMentionLength,
    long LMentionEntryId,
    long LMentionSenseId = 0)
{
    public bool LMentionLinked => LMentionEntryId != 0;

    public bool LMentionSensed => LMentionSenseId != 0;

    public static bool LMentionOverlapCheck(IReadOnlyList<LMention> mentions)
    {
        IReadOnlyList<LMention> sorted = LMentionSort(mentions);
        for (int index = 1; index < sorted.Count; index++)
        {
            LMention previous = sorted[index - 1];
            if (sorted[index].LMentionOffset < previous.LMentionOffset + previous.LMentionLength)
            {
                return true;
            }
        }

        return false;
    }

    public static IReadOnlyList<LMention> LMentionSort(IReadOnlyList<LMention> mentions)
    {
        return mentions.OrderBy(static mention => mention.LMentionOffset).ToList();
    }
}
