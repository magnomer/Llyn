using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LRegisterChip
{
    private readonly LRegisterVault _lRegisterChipVault;
    private readonly LIdentity _lRegisterChipIdentity;

    public LRegisterChip(LRegisterVault registers, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(registers);
        ArgumentNullException.ThrowIfNull(identity);
        _lRegisterChipVault = registers;
        _lRegisterChipIdentity = identity;
    }

    public LEntryDraft? LRegisterChipApply(LEntryDraft content, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(content);

        return request switch
        {
            LRequestRegisterAddition sent => LRegisterChipAdd(content, sent),
            LRequestRegisterPick sent => LRegisterChipInsert(content, sent),
            LRequestRegisterRemoval sent => LRegisterChipRemove(content, sent),
            LRequestRegisterShift sent => LRegisterChipMove(content, sent),
            LRequestRegisterName sent => LRegisterChipChange(content, sent),
            _ => null,
        };
    }

    public LEntryDraft LRegisterChipAdd(LEntryDraft content, LRequestRegisterAddition request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LStateValue name = LStateValue.LStateValueRead(request.LRequestValue);
        LRegister? stored = LRegisterChipResolve(_lRegisterChipVault, name.LStateValueShow());
        return LRegisterChipApply(
            content,
            request.LRequestCardId,
            registers =>
            {
                if (name.LStateValueShown is string shown && LDraftClerkList.LDraftHeldCheck(
                        registers, static row => row.LRegisterDraftName.LStateValueShow(), shown))
                {
                    return registers;
                }

                return stored is not null
                    ? LDraftClerkList.LDraftListInsert(
                        registers,
                        new LRegisterDraft(stored.LRegisterName, stored.LRegisterId),
                        stored.LRegisterId,
                        request.LRequestPosition,
                        static row => row.LRegisterDraftId)
                    : LDraftClerkList.LDraftListAdd(
                        registers,
                        new LRegisterDraft(name, _lRegisterChipIdentity.LIdentityCreate()),
                        request.LRequestPosition);
            });
    }

    public LEntryDraft LRegisterChipInsert(LEntryDraft content, LRequestRegisterPick request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.LRequestRegisterId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LRegister stored = _lRegisterChipVault.LRegisterRead(request.LRequestRegisterId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        LRegisterDraft register = new(stored.LRegisterName, stored.LRegisterId);

        return LRegisterChipApply(
            content,
            request.LRequestCardId,
            registers => LDraftClerkList.LDraftListInsert(
                registers, register, stored.LRegisterId, request.LRequestPosition, static row => row.LRegisterDraftId));
    }

    public static LEntryDraft LRegisterChipRemove(LEntryDraft content, LRequestRegisterRemoval request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LRegisterChipApply(
            content,
            request.LRequestCardId,
            registers => LDraftClerkList.LDraftListRemove(
                registers, request.LRequestRegisterId, static row => row.LRegisterDraftId));
    }

    public static LEntryDraft LRegisterChipMove(LEntryDraft content, LRequestRegisterShift request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return LRegisterChipApply(
            content,
            request.LRequestCardId,
            registers => LDraftClerkList.LDraftListMove(
                registers, request.LRequestRegisterId, request.LRequestPosition, static row => row.LRegisterDraftId));
    }

    public static LEntryDraft LRegisterChipChange(LEntryDraft content, LRequestRegisterName request)
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

    public static LRegister? LRegisterChipResolve(LRegisterVault registers, string name)
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

    private static LEntryDraft LRegisterChipApply(
        LEntryDraft content,
        long cardId,
        Func<IReadOnlyList<LRegisterDraft>, IReadOnlyList<LRegisterDraft>> change)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftRegister = change(card.LCardDraftRegister) });
    }
}
