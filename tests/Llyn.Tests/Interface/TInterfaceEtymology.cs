using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LEtymology TEtymologyCreate(long entryId, string text, IReadOnlyList<LMention> mentions) =>
        new(0, entryId, text, mentions);

    internal static LRequest TEtymologyTextCreate(long draftId, string text) =>
        new LRequestEtymologyText(draftId, text);

    internal static LRequest TEtymologyMentionCreate(long draftId, int offset, int length, long entryId) =>
        new LRequestEtymologyMention(draftId, offset, length, entryId);

    internal static LRequest TEtymonAdditionCreate(long draftId, long entryId, int position) =>
        new LRequestEtymonAddition(draftId, entryId, position);

    internal static LRequest TEtymonRemovalCreate(long draftId, long entryId) =>
        new LRequestEtymonRemoval(draftId, entryId);

    internal static LRequest TEtymonShiftCreate(long draftId, long entryId, int position) =>
        new LRequestEtymonShift(draftId, entryId, position);
}
