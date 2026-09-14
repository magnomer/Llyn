using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LEntryDraft LEngineSituationAdd(LEntryDraft content, LRequestSituationAddition request)
    {
        LStateValue title = LEngineValueRead(request.LRequestValue);
        LSituation? stored = LEngineSituationResolve(new LSituationArchive(_lEngineDatabase), title);
        if (stored is not null)
        {
            LSituationDraft found = LEngineSituationRead(stored);
            return LEngineSituationApply(
                content,
                request.LRequestCardId,
                situations => LEngineListInsert(
                    situations, found, stored.LSituationId, request.LRequestPosition, static row => row.LSituationDraftId));
        }

        LSituationDraft situation = new(
            title,
            LEngineIdentityCreate(),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

        return LEngineSituationApply(
            content,
            request.LRequestCardId,
            situations => LEngineListAdd(situations, situation, request.LRequestPosition));
    }

    private LEntryDraft LEngineSituationInsert(LEntryDraft content, LRequestSituationPick request)
    {
        if (request.LRequestSituationId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalSituation);
        }

        LSituation stored = new LSituationArchive(_lEngineDatabase).LSituationRead(request.LRequestSituationId)
            ?? throw new LRefusal(LRefusal.LRefusalSituation);

        LSituationDraft situation = LEngineSituationRead(stored);

        return LEngineSituationApply(
            content,
            request.LRequestCardId,
            situations => LEngineListInsert(
                situations, situation, stored.LSituationId, request.LRequestPosition, static row => row.LSituationDraftId));
    }

    private static LEntryDraft LEngineSituationRemove(LEntryDraft content, LRequestSituationRemoval request)
    {
        return LEngineSituationApply(
            content,
            request.LRequestCardId,
            situations => LEngineListRemove(situations, request.LRequestSituationId, static row => row.LSituationDraftId));
    }

    private static LEntryDraft LEngineSituationMove(LEntryDraft content, LRequestSituationShift request)
    {
        return LEngineSituationApply(
            content,
            request.LRequestCardId,
            situations => LEngineListMove(
                situations, request.LRequestSituationId, request.LRequestPosition, static row => row.LSituationDraftId));
    }

    private static LDraft LEngineSituationChange(LDraft draft, long id, Func<LSituation, LSituation> change)
    {
        if (id == 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        if (draft.LDraftSituation is LSituation held && held.LSituationId == id)
        {
            return draft with { LDraftSituation = change(held) };
        }

        LEntryDraft content = LEngineRowChange(draft.LDraftContent, card =>
        {
            IReadOnlyList<LSituationDraft>? situations = LEngineListChange(
                card.LCardDraftSituation,
                id,
                static row => row.LSituationDraftId,
                situation => LEngineSituationRead(change(LEngineSituationRead(situation))));

            return situations is null ? null : card with { LCardDraftSituation = situations };
        });

        return draft with { LDraftContent = content };
    }

    private static LSituationDraft LEngineSituationRead(LSituation stored)
    {
        return new LSituationDraft(
            stored.LSituationTitle, stored.LSituationId, stored.LSituationDescription, stored.LSituationKind);
    }

    private static LSituation LEngineSituationRead(LSituationDraft draft)
    {
        return new LSituation(
            draft.LSituationDraftId,
            draft.LSituationDraftTitle,
            draft.LSituationDraftDescription,
            draft.LSituationDraftKind);
    }

    private static LEntryDraft LEngineSituationApply(
        LEntryDraft content,
        long cardId,
        Func<IReadOnlyList<LSituationDraft>, IReadOnlyList<LSituationDraft>> change)
    {
        return LEngineCardChange(
            content, cardId, card => card with { LCardDraftSituation = change(card.LCardDraftSituation) });
    }

    private LEntryDraft LEngineRegisterAdd(LEntryDraft content, LRequestRegisterAddition request)
    {
        LStateValue name = LEngineValueRead(request.LRequestValue);
        LRegister? stored = LEngineRegisterResolve(name.LStateValueShow());
        if (stored is not null)
        {
            LRegisterDraft found = new(stored.LRegisterName, stored.LRegisterId);
            return LEngineRegisterApply(
                content,
                request.LRequestCardId,
                registers => LEngineListInsert(
                    registers, found, stored.LRegisterId, request.LRequestPosition, static row => row.LRegisterDraftId));
        }

        LRegisterDraft register = new(name, LEngineIdentityCreate());

        return LEngineRegisterApply(
            content,
            request.LRequestCardId,
            registers => LEngineListAdd(registers, register, request.LRequestPosition));
    }

    private LEntryDraft LEngineRegisterInsert(LEntryDraft content, LRequestRegisterPick request)
    {
        if (request.LRequestRegisterId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LRegister stored = new LRegisterArchive(_lEngineDatabase).LRegisterRead(request.LRequestRegisterId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        LRegisterDraft register = new(stored.LRegisterName, stored.LRegisterId);

        return LEngineRegisterApply(
            content,
            request.LRequestCardId,
            registers => LEngineListInsert(
                registers, register, stored.LRegisterId, request.LRequestPosition, static row => row.LRegisterDraftId));
    }

    private static LEntryDraft LEngineRegisterRemove(LEntryDraft content, LRequestRegisterRemoval request)
    {
        return LEngineRegisterApply(
            content,
            request.LRequestCardId,
            registers => LEngineListRemove(registers, request.LRequestRegisterId, static row => row.LRegisterDraftId));
    }

    private static LEntryDraft LEngineRegisterMove(LEntryDraft content, LRequestRegisterShift request)
    {
        return LEngineRegisterApply(
            content,
            request.LRequestCardId,
            registers => LEngineListMove(
                registers, request.LRequestRegisterId, request.LRequestPosition, static row => row.LRegisterDraftId));
    }

    private static LEntryDraft LEngineRegisterChange(LEntryDraft content, LRequestRegisterName request)
    {
        LStateValue value = LEngineValueRead(request.LRequestValue);

        return LEngineRowChange(content, card =>
        {
            IReadOnlyList<LRegisterDraft>? registers = LEngineListChange(
                card.LCardDraftRegister,
                request.LRequestRegisterId,
                static row => row.LRegisterDraftId,
                register => register with { LRegisterDraftName = value });

            return registers is null ? null : card with { LCardDraftRegister = registers };
        });
    }

    private static LEntryDraft LEngineRegisterApply(
        LEntryDraft content,
        long cardId,
        Func<IReadOnlyList<LRegisterDraft>, IReadOnlyList<LRegisterDraft>> change)
    {
        return LEngineCardChange(
            content, cardId, card => card with { LCardDraftRegister = change(card.LCardDraftRegister) });
    }

    private LEntryDraft LEngineTagAdd(LEntryDraft content, LRequestTagAddition request)
    {
        LTagDraft tag = new(LEngineIdentityCreate(), request.LRequestText);

        return LEngineTagApply(
            content,
            request.LRequestCardId,
            tags => LEngineListAdd(tags, tag, request.LRequestPosition));
    }

    private LEntryDraft LEngineTagInsert(LEntryDraft content, LRequestTagPick request)
    {
        if (request.LRequestTagId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LTag stored = new LTagArchive(_lEngineDatabase).LTagRead(request.LRequestTagId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        LTagDraft tag = new(stored.LTagId, stored.LTagText);

        return LEngineTagApply(
            content,
            request.LRequestCardId,
            tags => LEngineListInsert(tags, tag, stored.LTagId, request.LRequestPosition, static row => row.LTagDraftId));
    }

    private static LEntryDraft LEngineTagRemove(LEntryDraft content, LRequestTagRemoval request)
    {
        return LEngineTagApply(
            content,
            request.LRequestCardId,
            tags => LEngineListRemove(tags, request.LRequestTagId, static row => row.LTagDraftId));
    }

    private static LEntryDraft LEngineTagMove(LEntryDraft content, LRequestTagShift request)
    {
        return LEngineTagApply(
            content,
            request.LRequestCardId,
            tags => LEngineListMove(tags, request.LRequestTagId, request.LRequestPosition, static row => row.LTagDraftId));
    }

    private static LEntryDraft LEngineTagChange(LEntryDraft content, LRequestTagText request)
    {
        string text = request.LRequestText ?? string.Empty;

        return LEngineRowChange(content, card =>
        {
            IReadOnlyList<LTagDraft>? tags = LEngineListChange(
                card.LCardDraftTag,
                request.LRequestTagId,
                static row => row.LTagDraftId,
                tag => tag with { LTagDraftText = text });

            return tags is null ? null : card with { LCardDraftTag = tags };
        });
    }

    private static LEntryDraft LEngineTagApply(
        LEntryDraft content, long cardId, Func<IReadOnlyList<LTagDraft>, IReadOnlyList<LTagDraft>> change)
    {
        return LEngineCardChange(content, cardId, card => card with { LCardDraftTag = change(card.LCardDraftTag) });
    }

    private static LEntryDraft LEngineTranslationInsert(LEntryDraft content, LRequestTranslationPick request)
    {
        if (request.LRequestEntryId == 0)
        {
            throw new LRefusal(LRefusal.LRefusalTarget);
        }

        return LEngineTranslationApply(
            content,
            request.LRequestCardId,
            translations => LEngineListInsert(
                translations,
                request.LRequestEntryId,
                request.LRequestEntryId,
                request.LRequestPosition,
                static row => row));
    }

    private static LEntryDraft LEngineTranslationRemove(LEntryDraft content, LRequestTranslationRemoval request)
    {
        return LEngineTranslationApply(
            content,
            request.LRequestCardId,
            translations => LEngineListRemove(translations, request.LRequestEntryId, static row => row));
    }

    private static LEntryDraft LEngineTranslationMove(LEntryDraft content, LRequestTranslationShift request)
    {
        return LEngineTranslationApply(
            content,
            request.LRequestCardId,
            translations => LEngineListMove(
                translations, request.LRequestEntryId, request.LRequestPosition, static row => row));
    }

    private static LEntryDraft LEngineTranslationApply(
        LEntryDraft content, long cardId, Func<IReadOnlyList<long>, IReadOnlyList<long>> change)
    {
        return LEngineCardChange(
            content, cardId, card => card with { LCardDraftTranslation = change(card.LCardDraftTranslation) });
    }
}
