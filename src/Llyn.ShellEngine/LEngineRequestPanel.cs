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

    private static LReference LEngineBodyApply(LReference held, LReference? sent)
    {
        if (sent is null)
        {
            throw new LRefusal(LRefusal.LRefusalReference);
        }

        return held with
        {
            LReferenceTitle = LEngineValueRead(sent.LReferenceTitle),
            LReferenceYear = LEngineValueRead(sent.LReferenceYear),
            LReferenceKind = sent.LReferenceKind,
            LReferenceNote = LEngineValueRead(sent.LReferenceNote),
            LReferenceUrl = LEngineValueRead(sent.LReferenceUrl),
            LReferenceAuthorState = sent.LReferenceAuthorState,
        };
    }

    private static LExample LEngineBodyApply(LExample held, LExample? sent)
    {
        if (sent is null)
        {
            throw new LRefusal(LRefusal.LRefusalExample);
        }

        return held with
        {
            LExampleLanguage = sent.LExampleLanguage ?? string.Empty,
            LExampleText = LEngineValueRead(sent.LExampleText),
            LExampleTranslation = LEngineValueRead(sent.LExampleTranslation),
            LExampleSource = sent.LExampleSource ?? LStateAnchor.LStateAnchorUnspecified,
        };
    }

    private static LSituation LEngineBodyApply(LSituation held, LSituation? sent)
    {
        if (sent is null)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        return held with
        {
            LSituationTitle = LEngineValueRead(sent.LSituationTitle),
            LSituationDescription = LEngineValueRead(sent.LSituationDescription),
            LSituationKind = LEngineValueRead(sent.LSituationKind),
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
            ? held with { LReferenceAuthorState = LState.LStateSpecified }
            : held;

        return draft with { LDraftAuthor = authors, LDraftReference = reference };
    }
}
