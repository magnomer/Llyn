using Llyn.Core;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LRequest TRequestHeadwordCreate(long draftId, string text) =>
        new LRequestHeadword(draftId, text);

    internal static LRequest TRequestTitleCreate(long draftId, long cardId, LStateValue value) =>
        new LRequestCardTitle(draftId, cardId, value);

    internal static LRequest TRequestAdditionCreate(long draftId, LCardKind kind, long parentId, int position) =>
        new LRequestCardAddition(draftId, kind, parentId, position);

    internal static LRequest TRequestRemovalCreate(long draftId, long cardId) =>
        new LRequestCardRemoval(draftId, cardId);

    internal static LRequest TRequestShiftCreate(long draftId, long cardId, long parentId, int position) =>
        new LRequestCardShift(draftId, cardId, parentId, position);
}
