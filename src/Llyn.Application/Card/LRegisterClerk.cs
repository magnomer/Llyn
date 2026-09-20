using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LRegisterClerk
{
    private readonly LRegisterVault _lRegisterClerkRegisters;

    public LRegisterClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lRegisterClerkRegisters = rig.LRigRegisters;
    }

    public IReadOnlyList<LRegister> LRegisterClerkRead(long ownerId, bool collocation)
    {
        return collocation
            ? _lRegisterClerkRegisters.LRegisterCollocationRead(ownerId)
            : _lRegisterClerkRegisters.LRegisterMeaningRead(ownerId);
    }

    public IReadOnlyList<LRegister> LRegisterClerkFind(string query, string language)
    {
        ArgumentNullException.ThrowIfNull(query);

        LRegisterClerkPrepare(language);

        string written = query.Trim();
        List<LRegister> found = [];
        foreach (LRegister register in _lRegisterClerkRegisters.LRegisterRead())
        {
            if (written.Length != 0
                && !LCatalog.LCatalogTextMatch(register.LRegisterName.LStateValueShow(), written))
            {
                continue;
            }

            found.Add(register);
        }

        return found;
    }

    public IReadOnlyList<LCatalogRegister> LRegisterClerkFind(string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(query);

        IReadOnlyDictionary<long, int> counts = _lRegisterClerkRegisters.LRegisterReferenceRead();

        string written = query.Trim();
        List<LCatalogRegister> found = [];
        foreach (LRegister register in _lRegisterClerkRegisters.LRegisterRead())
        {
            LCatalogRegister row = LCatalogRegister.LCatalogRegisterCreate(
                register, counts.TryGetValue(register.LRegisterId, out int usage) ? usage : 0);
            if (row.LCatalogRegisterMatch(written))
            {
                found.Add(row);
            }
        }

        return LCatalogRegister.LCatalogRegisterSort(found, order);
    }

    public LRegister LRegisterClerkCreate(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return LDraftClerkChip.LRegisterResolve(_lRegisterClerkRegisters, name)
            ?? _lRegisterClerkRegisters.LRegisterCreate(new LRegister(
                0,
                LStateValue.LStateValueRead(name.Trim())));
    }

    public void LRegisterClerkChange(long registerId, string renamed)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registerId);
        ArgumentNullException.ThrowIfNull(renamed);

        _lRegisterClerkRegisters.LRegisterNameUpdate(
            registerId, LStateValue.LStateValueRead(renamed.Trim()));
    }

    public void LRegisterClerkDelete(long registerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registerId);
        _lRegisterClerkRegisters.LRegisterDelete(registerId, true);
    }

    public void LRegisterClerkPrepare(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return;
        }

        _lRegisterClerkRegisters.LRegisterDefaultCreate(
            _lRegisterClerkRegisters.LRegisterLoad(language));
    }

    public void LRegisterClerkSync(
        long ownerId,
        IReadOnlyList<LRegisterDraft> drafts,
        string language,
        bool collocation,
        Dictionary<long, long> identity)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(identity);

        LRegisterClerkPrepare(language);

        LRegisterVault registers = _lRegisterClerkRegisters;

        LCardClerkField.LCardFieldSync(
            LRegisterClerkRead(drafts),
            collocation ? registers.LRegisterCollocationRead(ownerId) : registers.LRegisterMeaningRead(ownerId),
            row => row.LRegisterId,
            written => LRegisterClerkResolve(written, identity),
            rowId =>
            {
                if (collocation)
                {
                    registers.LRegisterCollocationDetach(ownerId, rowId);
                    return;
                }

                registers.LRegisterMeaningDetach(ownerId, rowId);
            },
            (rowId, position) =>
            {
                if (collocation)
                {
                    registers.LRegisterCollocationAttach(ownerId, rowId, position);
                    return;
                }

                registers.LRegisterMeaningAttach(ownerId, rowId, position);
            });
    }

    public static bool LRegisterClerkMatch(
        IReadOnlyList<LRegisterDraft> one, IReadOnlyList<LRegisterDraft> other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LRegisterDraftName != other[index].LRegisterDraftName
                || one[index].LRegisterDraftId != other[index].LRegisterDraftId)
            {
                return false;
            }
        }

        return true;
    }

    private static IEnumerable<LRegisterDraft> LRegisterClerkRead(IReadOnlyList<LRegisterDraft> drafts)
    {
        foreach (LRegisterDraft draft in drafts)
        {
            if (!draft.LRegisterDraftName.LStateValueEmpty)
            {
                yield return draft;
            }
        }
    }

    private long LRegisterClerkResolve(LRegisterDraft draft, Dictionary<long, long> identity)
    {
        LRegisterVault registers = _lRegisterClerkRegisters;
        if (draft.LRegisterDraftId > 0)
        {
            LRegister stored = registers.LRegisterRead(draft.LRegisterDraftId)
                ?? throw new LRefusal(LRefusal.LRefusalLink);
            if (!stored.LRegisterBuiltin && stored.LRegisterName != draft.LRegisterDraftName)
            {
                registers.LRegisterNameUpdate(stored.LRegisterId, draft.LRegisterDraftName);
            }

            return stored.LRegisterId;
        }

        LRegister? found = LDraftClerkChip.LRegisterResolve(registers, draft.LRegisterDraftName.LStateValueShow());
        if (found is not null)
        {
            LIdentity.LIdentityRecord(identity, draft.LRegisterDraftId, found.LRegisterId);
            return found.LRegisterId;
        }

        long created = registers.LRegisterCreate(new LRegister(0, draft.LRegisterDraftName)).LRegisterId;
        LIdentity.LIdentityRecord(identity, draft.LRegisterDraftId, created);
        return created;
    }
}
