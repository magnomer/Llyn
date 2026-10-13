using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LSituationChip
{
    private readonly LSituationVault _lSituationChipVault;
    private readonly LIdentity _lSituationChipIdentity;

    public LSituationChip(LSituationVault situations, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(situations);
        ArgumentNullException.ThrowIfNull(identity);
        _lSituationChipVault = situations;
        _lSituationChipIdentity = identity;
    }

    public static LDraft? LSituationChipApply(LDraft draft, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return request switch
        {
            LRequestSituationTitle sent => LSituationChipChange(
                draft,
                sent.LRequestSituationId,
                situation => situation with { LSituationTitle = LStateValue.LStateValueRead(sent.LRequestValue) }),
            LRequestSituationDescription sent => LSituationChipChange(
                draft,
                sent.LRequestSituationId,
                situation => situation with
                {
                    LSituationDescription = LStateValue.LStateValueRead(sent.LRequestValue),
                }),
            LRequestSituationKind sent => LSituationChipChange(
                draft,
                sent.LRequestSituationId,
                situation => situation with { LSituationKind = LStateValue.LStateValueRead(sent.LRequestValue) }),
            _ => null,
        };
    }

    public LEntryDraft? LSituationChipApply(LEntryDraft content, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(content);

        return request switch
        {
            LRequestSituationAddition sent => LSituationChipAdd(content, sent),
            LRequestSituationPick sent => LSituationChipInsert(content, sent),
            LRequestSituationRemoval sent => LSituationChipRemove(content, sent),
            LRequestSituationShift sent => LSituationChipMove(content, sent),
            _ => null,
        };
    }

    public LEntryDraft LSituationChipAdd(LEntryDraft content, LRequestSituationAddition request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LStateValue title = LStateValue.LStateValueRead(request.LRequestValue);
        LSituation? stored = LSituationChipResolve(_lSituationChipVault, title);
        return LSituationChipApply(
            content,
            request.LRequestCardId,
            situations =>
            {
                if (title.LStateValueShown is string shown && LDraftClerkList.LDraftHeldCheck(
                        situations, static row => row.LSituationDraftTitle.LStateValueShow(), shown))
                {
                    return situations;
                }

                return stored is not null
                    ? LDraftClerkList.LDraftListInsert(
                        situations,
                        LSituationChipRead(stored),
                        stored.LSituationId,
                        request.LRequestPosition,
                        static row => row.LSituationDraftId)
                    : LDraftClerkList.LDraftListAdd(
                        situations,
                        new LSituationDraft(
                            title,
                            _lSituationChipIdentity.LIdentityCreate(),
                            LStateValue.LStateValueUnspecified,
                            LStateValue.LStateValueUnspecified),
                        request.LRequestPosition);
            });
    }

    public LEntryDraft LSituationChipInsert(LEntryDraft content, LRequestSituationPick request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestSituationId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalSituation);
        }

        LSituation stored = _lSituationChipVault.LSituationRead(request.LRequestSituationId)
            ?? throw new LRefusal(LRefusal.LRefusalSituation);

        LSituationDraft situation = LSituationChipRead(stored);

        return LSituationChipApply(
            content,
            request.LRequestCardId,
            situations => LDraftClerkList.LDraftListInsert(
                situations,
                situation,
                stored.LSituationId,
                request.LRequestPosition,
                static row => row.LSituationDraftId));
    }

    public static LEntryDraft LSituationChipRemove(LEntryDraft content, LRequestSituationRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LSituationChipApply(
            content,
            request.LRequestCardId,
            situations => LDraftClerkList.LDraftListRemove(
                situations, request.LRequestSituationId, static row => row.LSituationDraftId));
    }

    public static LEntryDraft LSituationChipMove(LEntryDraft content, LRequestSituationShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LSituationChipApply(
            content,
            request.LRequestCardId,
            situations => LDraftClerkList.LDraftListMove(
                situations,
                request.LRequestSituationId,
                request.LRequestPosition,
                static row => row.LSituationDraftId));
    }

    public static LDraft LSituationChipChange(LDraft draft, long id, Func<LSituation, LSituation> change)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(change);

        if (id == 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        if (draft.LDraftSituation is LSituation held && held.LSituationId == id)
        {
            return draft with { LDraftSituation = change(held) };
        }

        LEntryDraft content = LDraftClerkList.LDraftRowChange(draft.LDraftContent, card =>
        {
            IReadOnlyList<LSituationDraft>? situations = LDraftClerkList.LDraftListChange(
                card.LCardDraftSituation,
                id,
                static row => row.LSituationDraftId,
                situation => LSituationChipRead(change(LSituationChipRead(situation))));

            return situations is null ? null : card with { LCardDraftSituation = situations };
        });

        return draft with { LDraftContent = content };
    }

    public static LSituation? LSituationChipResolve(LSituationVault situations, LStateValue title)
    {
        ArgumentNullException.ThrowIfNull(situations);
        ArgumentNullException.ThrowIfNull(title);

        string written = LCatalog.LCatalogTextNormalize(title.LStateValueShow());
        if (written.Length == 0)
        {
            return null;
        }

        LSituation? found = null;
        foreach (LSituation situation in situations.LSituationRead())
        {
            if (!string.Equals(
                    LCatalog.LCatalogTextNormalize(situation.LSituationTitle.LStateValueShow()),
                    written,
                    StringComparison.Ordinal))
            {
                continue;
            }

            if (found is not null)
            {
                return null;
            }

            found = situation;
        }

        return found;
    }

    private static LSituationDraft LSituationChipRead(LSituation stored)
    {
        return new LSituationDraft(
            stored.LSituationTitle, stored.LSituationId, stored.LSituationDescription, stored.LSituationKind);
    }

    private static LSituation LSituationChipRead(LSituationDraft draft)
    {
        return new LSituation(
            draft.LSituationDraftId,
            draft.LSituationDraftTitle,
            draft.LSituationDraftDescription,
            draft.LSituationDraftKind);
    }

    private static LEntryDraft LSituationChipApply(
        LEntryDraft content,
        long cardId,
        Func<IReadOnlyList<LSituationDraft>, IReadOnlyList<LSituationDraft>> change)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftSituation = change(card.LCardDraftSituation) });
    }
}
