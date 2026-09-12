using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private static LDraft LEngineExampleChange(LDraft draft, Func<LExample, LExample> change)
    {
        LExample held = draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample);
        return draft with { LDraftExample = change(held) };
    }

    private static LDraft LEngineReferenceChange(LDraft draft, Func<LReference, LReference> change)
    {
        LReference held = draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference);
        return draft with { LDraftReference = change(held) };
    }

    private static LReference LEngineBodyApply(LReference held, LRequestReferenceBody sent)
    {
        return held with
        {
            LReferenceTitle = LEngineValueRead(sent.LRequestTitle),
            LReferenceYear = LEngineValueRead(sent.LRequestYear),
            LReferenceKind = sent.LRequestKind,
            LReferenceNote = LEngineValueRead(sent.LRequestNote),
            LReferenceUrl = LEngineValueRead(sent.LRequestUrl),
            LReferenceAuthorState = LStateMark.LStateMarkRead(sent.LRequestAuthorState),
        };
    }

    private static LExample LEngineBodyApply(LExample held, LRequestExampleBody sent)
    {
        return held with
        {
            LExampleLanguage = sent.LRequestLanguage ?? string.Empty,
            LExampleText = LEngineValueRead(sent.LRequestText),
            LExampleTranslation = LEngineValueRead(sent.LRequestTranslation),
            LExampleSource = LStateAnchor.LStateAnchorRead(sent.LRequestReferenceId),
        };
    }

    private static LSituation LEngineBodyApply(LSituation held, LRequestSituationBody sent)
    {
        return held with
        {
            LSituationTitle = LEngineValueRead(sent.LRequestTitle),
            LSituationDescription = LEngineValueRead(sent.LRequestDescription),
            LSituationKind = LEngineValueRead(sent.LRequestKind),
        };
    }

    private LDraft LEngineAuthorAdd(LDraft draft, LRequestAuthorAddition request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.LRequestText);

        LAuthor author = new(LEngineIdentityCreate(), request.LRequestText.Trim());
        return LEngineAuthorApply(
            draft, authors => LEngineListAdd(authors, author, request.LRequestPosition));
    }

    private LDraft LEngineAuthorInsert(LDraft draft, LRequestAuthorPick request)
    {
        if (request.LRequestAuthorId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LAuthor stored = new LAuthorArchive(_lEngineDatabase).LAuthorRead(request.LRequestAuthorId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return LEngineAuthorApply(
            draft,
            authors => LEngineListInsert(authors, stored, stored.LAuthorId, request.LRequestPosition, static row => row.LAuthorId));
    }

    private static LDraft LEngineAuthorRemove(LDraft draft, long authorId)
    {
        return LEngineAuthorApply(
            draft, authors => LEngineListRemove(authors, authorId, static row => row.LAuthorId), false);
    }

    private static LDraft LEngineAuthorMove(LDraft draft, LRequestAuthorShift request)
    {
        return LEngineAuthorApply(
            draft,
            authors => LEngineListMove(
                authors, request.LRequestAuthorId, request.LRequestPosition, static row => row.LAuthorId),
            false);
    }

    private static LDraft LEngineAuthorApply(
        LDraft draft, Func<IReadOnlyList<LAuthor>, IReadOnlyList<LAuthor>> change, bool credited = true)
    {
        LReference held = draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference);
        IReadOnlyList<LAuthor> authors = change(draft.LDraftAuthor);

        LReference reference = credited && authors.Count > 0
            ? held with { LReferenceAuthorState = LStateMark.LStateMarkSpecified }
            : held;

        return draft with { LDraftAuthor = authors, LDraftReference = reference };
    }
}
