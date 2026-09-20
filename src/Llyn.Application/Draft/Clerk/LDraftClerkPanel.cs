using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkPanel
{
    private readonly LAuthorVault _lDraftClerkAuthors;
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkPanel(LAuthorVault authors, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(authors);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkAuthors = authors;
        _lDraftClerkIdentity = identity;
    }

    public static LDraft LExampleChange(LDraft draft, Func<LExample, LExample> change)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(change);

        LExample held = draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample);
        return draft with { LDraftExample = change(held) };
    }

    public static LDraft LReferenceChange(LDraft draft, Func<LReference, LReference> change)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(change);

        LReference held = draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference);
        return draft with { LDraftReference = change(held) };
    }

    public static LReference LReferenceBodyApply(LReference held, LRequestReferenceBody sent)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(sent);

        return held with
        {
            LReferenceTitle = LStateValue.LStateValueRead(sent.LRequestTitle),
            LReferenceYear = LStateValue.LStateValueRead(sent.LRequestYear),
            LReferenceKind = sent.LRequestKind,
            LReferenceNote = LStateValue.LStateValueRead(sent.LRequestNote),
            LReferenceUrl = LStateValue.LStateValueRead(sent.LRequestUrl),
            LReferenceAuthorState = LStateMark.LStateMarkRead(sent.LRequestAuthorState),
        };
    }

    public static LExample LExampleBodyApply(LExample held, LRequestExampleBody sent)
    {
        ArgumentNullException.ThrowIfNull(sent);

        return LDraftClerkMention.LMentionUpdate(held, LStateValue.LStateValueRead(sent.LRequestText)) with
        {
            LExampleLanguage = sent.LRequestLanguage ?? string.Empty,
            LExampleSource = LStateAnchor.LStateAnchorRead(sent.LRequestReferenceId),
        };
    }

    public static LSituation LSituationBodyApply(LSituation held, LRequestSituationBody sent)
    {
        ArgumentNullException.ThrowIfNull(held);
        ArgumentNullException.ThrowIfNull(sent);

        return held with
        {
            LSituationTitle = LStateValue.LStateValueRead(sent.LRequestTitle),
            LSituationDescription = LStateValue.LStateValueRead(sent.LRequestDescription),
            LSituationKind = LStateValue.LStateValueRead(sent.LRequestKind),
        };
    }

    public LDraft LAuthorAdd(LDraft draft, LRequestAuthorAddition request)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.LRequestText);

        string name = request.LRequestText.Trim();
        LAuthor author = LAuthorMatch(draft.LDraftAuthor, name)
            ?? LAuthorMatch(_lDraftClerkAuthors.LAuthorAllRead(), name)
            ?? new LAuthor(_lDraftClerkIdentity.LIdentityCreate(), name);
        return LAuthorApply(
            draft,
            authors => LDraftClerkList.LDraftListChange(
                authors,
                author,
                author.LAuthorId,
                request.LRequestPosition,
                request.LRequestFormerId,
                static row => row.LAuthorId));
    }

    private static LAuthor? LAuthorMatch(IReadOnlyList<LAuthor> authors, string name)
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

    public LDraft LAuthorInsert(LDraft draft, LRequestAuthorPick request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestAuthorId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LAuthor stored = _lDraftClerkAuthors.LAuthorRead(request.LRequestAuthorId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return LAuthorApply(
            draft,
            authors => LDraftClerkList.LDraftListChange(
                authors,
                stored,
                stored.LAuthorId,
                request.LRequestPosition,
                request.LRequestFormerId,
                static row => row.LAuthorId));
    }

    public static LDraft LAuthorRemove(LDraft draft, long authorId)
    {
        return LAuthorApply(
            draft, authors => LDraftClerkList.LDraftListRemove(authors, authorId, static row => row.LAuthorId), false);
    }

    public static LDraft LAuthorMove(LDraft draft, LRequestAuthorShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LAuthorApply(
            draft,
            authors => LDraftClerkList.LDraftListMove(
                authors, request.LRequestAuthorId, request.LRequestPosition, static row => row.LAuthorId),
            false);
    }

    public static LDraft LAuthorChange(LDraft draft, string? name)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LAuthor held = draft.LDraftAuthorHeld ?? throw new LRefusal(LRefusal.LRefusalLink);
        return draft with { LDraftAuthorHeld = held with { LAuthorName = name ?? string.Empty } };
    }

    private static LDraft LAuthorApply(
        LDraft draft, Func<IReadOnlyList<LAuthor>, IReadOnlyList<LAuthor>> change, bool credited = true)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LReference held = draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference);
        IReadOnlyList<LAuthor> authors = change(draft.LDraftAuthor);

        LReference reference = credited && authors.Count > 0
            ? held with { LReferenceAuthorState = LStateMark.LStateMarkSpecified }
            : held;

        return draft with { LDraftAuthor = authors, LDraftReference = reference };
    }
}
