using System;
using System.Linq;

namespace Llyn.Core;

public sealed record LMarkupTarget(
    long LMarkupTargetId,
    int LMarkupTargetMeaning,
    int LMarkupTargetCollocation)
{
    public static LMarkupTarget LMarkupTargetCreate(long id, LEntryDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return new LMarkupTarget(
            id,
            draft.LEntryDraftMeanings.Sum(static card => card.LCardDraftTally),
            draft.LEntryDraftCollocations.Sum(static card => card.LCardDraftTally));
    }
}
