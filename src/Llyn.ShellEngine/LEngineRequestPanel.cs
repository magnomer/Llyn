using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

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
        return LEngineMentionUpdate(held, LEngineValueRead(sent.LRequestText)) with
        {
            LExampleLanguage = sent.LRequestLanguage ?? string.Empty,
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

        string name = request.LRequestText.Trim();
        LAuthor author = LEngineAuthorMatch(draft.LDraftAuthor, name)
            ?? LEngineAuthorMatch(_lEngineAuthors.LAuthorAllRead(), name)
            ?? new LAuthor(LEngineIdentityCreate(), name);
        return LEngineAuthorApply(
            draft,
            authors => LEngineListChange(
                authors,
                author,
                author.LAuthorId,
                request.LRequestPosition,
                request.LRequestFormerId,
                static row => row.LAuthorId));
    }

    private static LAuthor? LEngineAuthorMatch(IReadOnlyList<LAuthor> authors, string name)
    {
        string written = LCatalog.LCatalogTextNormalize(name);
        foreach (LAuthor author in authors)
        {
            if (string.Equals(LCatalog.LCatalogTextNormalize(author.LAuthorName), written, StringComparison.Ordinal))
            {
                return author;
            }
        }

        return null;
    }

    private LDraft LEngineAuthorInsert(LDraft draft, LRequestAuthorPick request)
    {
        if (request.LRequestAuthorId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LAuthor stored = _lEngineAuthors.LAuthorRead(request.LRequestAuthorId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return LEngineAuthorApply(
            draft,
            authors => LEngineListChange(
                authors,
                stored,
                stored.LAuthorId,
                request.LRequestPosition,
                request.LRequestFormerId,
                static row => row.LAuthorId));
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

    private static LDraft LEngineAuthorChange(LDraft draft, string name)
    {
        LAuthor held = draft.LDraftAuthorHeld ?? throw new LRefusal(LRefusal.LRefusalLink);
        return draft with { LDraftAuthorHeld = held with { LAuthorName = name ?? string.Empty } };
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
