using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkChip
{
    private readonly LSituationVault _lDraftClerkSituations;
    private readonly LRegisterVault _lDraftClerkRegisters;
    private readonly LTagVault _lDraftClerkTags;
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkChip(
        LSituationVault situations, LRegisterVault registers, LTagVault tags, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(situations);
        ArgumentNullException.ThrowIfNull(registers);
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkSituations = situations;
        _lDraftClerkRegisters = registers;
        _lDraftClerkTags = tags;
        _lDraftClerkIdentity = identity;
    }

    public LEntryDraft LSituationAdd(LEntryDraft content, LRequestSituationAddition request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LStateValue title = LStateValue.LStateValueRead(request.LRequestValue);
        LSituation? stored = LSituationResolve(_lDraftClerkSituations, title);
        if (stored is not null)
        {
            LSituationDraft found = LSituationRead(stored);
            return LSituationApply(
                content,
                request.LRequestCardId,
                situations => LDraftClerkList.LDraftListInsert(
                    situations,
                    found,
                    stored.LSituationId,
                    request.LRequestPosition,
                    static row => row.LSituationDraftId));
        }

        LSituationDraft situation = new(
            title,
            _lDraftClerkIdentity.LIdentityCreate(),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

        return LSituationApply(
            content,
            request.LRequestCardId,
            situations => LDraftClerkList.LDraftListAdd(situations, situation, request.LRequestPosition));
    }

    public LEntryDraft LSituationInsert(LEntryDraft content, LRequestSituationPick request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestSituationId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalSituation);
        }

        LSituation stored = _lDraftClerkSituations.LSituationRead(request.LRequestSituationId)
            ?? throw new LRefusal(LRefusal.LRefusalSituation);

        LSituationDraft situation = LSituationRead(stored);

        return LSituationApply(
            content,
            request.LRequestCardId,
            situations => LDraftClerkList.LDraftListInsert(
                situations,
                situation,
                stored.LSituationId,
                request.LRequestPosition,
                static row => row.LSituationDraftId));
    }

    public static LEntryDraft LSituationRemove(LEntryDraft content, LRequestSituationRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LSituationApply(
            content,
            request.LRequestCardId,
            situations => LDraftClerkList.LDraftListRemove(
                situations, request.LRequestSituationId, static row => row.LSituationDraftId));
    }

    public static LEntryDraft LSituationMove(LEntryDraft content, LRequestSituationShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LSituationApply(
            content,
            request.LRequestCardId,
            situations => LDraftClerkList.LDraftListMove(
                situations,
                request.LRequestSituationId,
                request.LRequestPosition,
                static row => row.LSituationDraftId));
    }

    public static LDraft LSituationChange(LDraft draft, long id, Func<LSituation, LSituation> change)
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
                situation => LSituationRead(change(LSituationRead(situation))));

            return situations is null ? null : card with { LCardDraftSituation = situations };
        });

        return draft with { LDraftContent = content };
    }

    public static LSituation? LSituationResolve(LSituationVault situations, LStateValue title)
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

    private static LSituationDraft LSituationRead(LSituation stored)
    {
        return new LSituationDraft(
            stored.LSituationTitle, stored.LSituationId, stored.LSituationDescription, stored.LSituationKind);
    }

    private static LSituation LSituationRead(LSituationDraft draft)
    {
        return new LSituation(
            draft.LSituationDraftId,
            draft.LSituationDraftTitle,
            draft.LSituationDraftDescription,
            draft.LSituationDraftKind);
    }

    private static LEntryDraft LSituationApply(
        LEntryDraft content,
        long cardId,
        Func<IReadOnlyList<LSituationDraft>, IReadOnlyList<LSituationDraft>> change)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftSituation = change(card.LCardDraftSituation) });
    }

    public LEntryDraft LRegisterAdd(LEntryDraft content, LRequestRegisterAddition request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LStateValue name = LStateValue.LStateValueRead(request.LRequestValue);
        LRegister? stored = LRegisterResolve(_lDraftClerkRegisters, name.LStateValueShow());
        if (stored is not null)
        {
            LRegisterDraft found = new(stored.LRegisterName, stored.LRegisterId);
            return LRegisterApply(
                content,
                request.LRequestCardId,
                registers => LDraftClerkList.LDraftListInsert(
                    registers,
                    found,
                    stored.LRegisterId,
                    request.LRequestPosition,
                    static row => row.LRegisterDraftId));
        }

        LRegisterDraft register = new(name, _lDraftClerkIdentity.LIdentityCreate());

        return LRegisterApply(
            content,
            request.LRequestCardId,
            registers => LDraftClerkList.LDraftListAdd(registers, register, request.LRequestPosition));
    }

    public LEntryDraft LRegisterInsert(LEntryDraft content, LRequestRegisterPick request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestRegisterId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LRegister stored = _lDraftClerkRegisters.LRegisterRead(request.LRequestRegisterId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        LRegisterDraft register = new(stored.LRegisterName, stored.LRegisterId);

        return LRegisterApply(
            content,
            request.LRequestCardId,
            registers => LDraftClerkList.LDraftListInsert(
                registers, register, stored.LRegisterId, request.LRequestPosition, static row => row.LRegisterDraftId));
    }

    public static LEntryDraft LRegisterRemove(LEntryDraft content, LRequestRegisterRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LRegisterApply(
            content,
            request.LRequestCardId,
            registers => LDraftClerkList.LDraftListRemove(
                registers, request.LRequestRegisterId, static row => row.LRegisterDraftId));
    }

    public static LEntryDraft LRegisterMove(LEntryDraft content, LRequestRegisterShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LRegisterApply(
            content,
            request.LRequestCardId,
            registers => LDraftClerkList.LDraftListMove(
                registers, request.LRequestRegisterId, request.LRequestPosition, static row => row.LRegisterDraftId));
    }

    public static LEntryDraft LRegisterChange(LEntryDraft content, LRequestRegisterName request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LStateValue value = LStateValue.LStateValueRead(request.LRequestValue);

        return LDraftClerkList.LDraftRowChange(content, card =>
        {
            IReadOnlyList<LRegisterDraft>? registers = LDraftClerkList.LDraftListChange(
                card.LCardDraftRegister,
                request.LRequestRegisterId,
                static row => row.LRegisterDraftId,
                register => register with { LRegisterDraftName = value });

            return registers is null ? null : card with { LCardDraftRegister = registers };
        });
    }

    public static LRegister? LRegisterResolve(LRegisterVault registers, string name)
    {
        ArgumentNullException.ThrowIfNull(registers);

        string written = LCatalog.LCatalogTextNormalize(name);
        if (written.Length == 0)
        {
            return null;
        }

        foreach (LRegister register in registers.LRegisterRead())
        {
            if (string.Equals(
                    LCatalog.LCatalogTextNormalize(register.LRegisterName.LStateValueShow()),
                    written,
                    StringComparison.Ordinal))
            {
                return register;
            }
        }

        return null;
    }

    private static LEntryDraft LRegisterApply(
        LEntryDraft content,
        long cardId,
        Func<IReadOnlyList<LRegisterDraft>, IReadOnlyList<LRegisterDraft>> change)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftRegister = change(card.LCardDraftRegister) });
    }

    public LEntryDraft LTagAdd(LEntryDraft content, LRequestTagAddition request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LTagDraft tag = new(_lDraftClerkIdentity.LIdentityCreate(), request.LRequestText);

        return LTagApply(
            content,
            request.LRequestCardId,
            tags => LDraftClerkList.LDraftListAdd(tags, tag, request.LRequestPosition));
    }

    public LEntryDraft LTagInsert(LEntryDraft content, LRequestTagPick request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestTagId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LTag stored = _lDraftClerkTags.LTagRead(request.LRequestTagId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        LTagDraft tag = new(stored.LTagId, stored.LTagText);

        return LTagApply(
            content,
            request.LRequestCardId,
            tags => LDraftClerkList.LDraftListInsert(
                tags, tag, stored.LTagId, request.LRequestPosition, static row => row.LTagDraftId));
    }

    public static LEntryDraft LTagRemove(LEntryDraft content, LRequestTagRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LTagApply(
            content,
            request.LRequestCardId,
            tags => LDraftClerkList.LDraftListRemove(tags, request.LRequestTagId, static row => row.LTagDraftId));
    }

    public static LEntryDraft LTagMove(LEntryDraft content, LRequestTagShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LTagApply(
            content,
            request.LRequestCardId,
            tags => LDraftClerkList.LDraftListMove(
                tags, request.LRequestTagId, request.LRequestPosition, static row => row.LTagDraftId));
    }

    public static LEntryDraft LTagChange(LEntryDraft content, LRequestTagText request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string text = request.LRequestText ?? string.Empty;

        return LDraftClerkList.LDraftRowChange(content, card =>
        {
            IReadOnlyList<LTagDraft>? tags = LDraftClerkList.LDraftListChange(
                card.LCardDraftTag,
                request.LRequestTagId,
                static row => row.LTagDraftId,
                tag => tag with { LTagDraftText = text });

            return tags is null ? null : card with { LCardDraftTag = tags };
        });
    }

    private static LEntryDraft LTagApply(
        LEntryDraft content, long cardId, Func<IReadOnlyList<LTagDraft>, IReadOnlyList<LTagDraft>> change)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftTag = change(card.LCardDraftTag) });
    }

    public static LEntryDraft LTranslationInsert(LEntryDraft content, LRequestTranslationPick request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestEntryId == 0)
        {
            throw new LRefusal(LRefusal.LRefusalTarget);
        }

        return LTranslationApply(
            content,
            request.LRequestCardId,
            translations => LDraftClerkList.LDraftListInsert(
                translations,
                request.LRequestEntryId,
                request.LRequestEntryId,
                request.LRequestPosition,
                static row => row));
    }

    public static LEntryDraft LTranslationRemove(LEntryDraft content, LRequestTranslationRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LTranslationApply(
            content,
            request.LRequestCardId,
            translations => LDraftClerkList.LDraftListRemove(translations, request.LRequestEntryId, static row => row));
    }

    public static LEntryDraft LTranslationMove(LEntryDraft content, LRequestTranslationShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LTranslationApply(
            content,
            request.LRequestCardId,
            translations => LDraftClerkList.LDraftListMove(
                translations, request.LRequestEntryId, request.LRequestPosition, static row => row));
    }

    private static LEntryDraft LTranslationApply(
        LEntryDraft content, long cardId, Func<IReadOnlyList<long>, IReadOnlyList<long>> change)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftTranslation = change(card.LCardDraftTranslation) });
    }
}
