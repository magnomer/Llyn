using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LRegister> LEngineRegisterRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LRegisterArchive registers = new(_lEngineDatabase);
            return LEngineOwnerCheck(owner)
                ? registers.LRegisterCollocationRead(ownerId)
                : registers.LRegisterMeaningRead(ownerId);
        }
    }

    public IReadOnlyList<LRegister> LEngineRegisterFind(string query, string language)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);

            LEngineRegisterCreate(language);

            string written = query.Trim();
            List<LRegister> found = [];
            foreach (LRegister register in new LRegisterArchive(_lEngineDatabase).LRegisterRead())
            {
                if (register.LRegisterBuiltin
                    && !string.IsNullOrWhiteSpace(language)
                    && !string.Equals(register.LRegisterLanguage, language, StringComparison.Ordinal))
                {
                    continue;
                }

                if (written.Length != 0
                    && !LCatalog.LCatalogTextMatch(register.LRegisterName.LStateValueShow(), written))
                {
                    continue;
                }

                found.Add(register);
            }

            return found;
        }
    }

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);

            LRegisterArchive archive = new(_lEngineDatabase);
            IReadOnlyDictionary<long, int> counts = archive.LRegisterReferenceRead();

            string written = query.Trim();
            List<LCatalogRegister> found = [];
            foreach (LRegister register in archive.LRegisterRead())
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
    }

    public void LEngineRegisterChange(long registerId, string renamed)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registerId);
            ArgumentNullException.ThrowIfNull(renamed);

            new LRegisterArchive(_lEngineDatabase).LRegisterNameUpdate(
                registerId, LStateValue.LStateValueRead(renamed.Trim()));
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, registerId);
    }

    public void LEngineRegisterDelete(long registerId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registerId);
            new LRegisterArchive(_lEngineDatabase).LRegisterDelete(registerId, true);
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, registerId);
    }

    private void LEngineRegisterCreate(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return;
        }

        new LRegisterArchive(_lEngineDatabase).LRegisterDefaultCreate(
            LRegisterLoader.LRegisterLoaderLoad(language));
    }

    private void LEngineRegisterSync(
        long ownerId,
        IReadOnlyList<LRegisterDraft> drafts,
        string language,
        bool collocation,
        Dictionary<long, long> identity)
    {
        LEngineRegisterCreate(language);

        LRegisterArchive registers = new(_lEngineDatabase);

        LEngineFieldSync(
            LEngineRegisterRead(drafts),
            collocation ? registers.LRegisterCollocationRead(ownerId) : registers.LRegisterMeaningRead(ownerId),
            row => row.LRegisterId,
            written => LEngineRegisterResolve(registers, written, identity),
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

    private static bool LEngineRegisterMatch(
        IReadOnlyList<LRegisterDraft> one, IReadOnlyList<LRegisterDraft> other)
    {
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

    private static IEnumerable<LRegisterDraft> LEngineRegisterRead(IReadOnlyList<LRegisterDraft> drafts)
    {
        foreach (LRegisterDraft draft in drafts)
        {
            if (!draft.LRegisterDraftName.LStateValueEmpty)
            {
                yield return draft;
            }
        }
    }

    private long LEngineRegisterResolve(
        LRegisterArchive registers, LRegisterDraft draft, Dictionary<long, long> identity)
    {
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

        LRegister? found = LEngineRegisterResolve(
            draft.LRegisterDraftName.LStateValueShow(), draft.LRegisterDraftLanguage);
        if (found is not null)
        {
            LEngineIdentityRecord(identity, draft.LRegisterDraftId, found.LRegisterId);
            return found.LRegisterId;
        }

        long created = registers.LRegisterCreate(new LRegister(
            0,
            draft.LRegisterDraftName,
            draft.LRegisterDraftLanguage)).LRegisterId;
        LEngineIdentityRecord(identity, draft.LRegisterDraftId, created);
        return created;
    }

    private LRegister? LEngineRegisterResolve(string name, string language)
    {
        string written = LCatalog.LCatalogTextNormalize(name);
        if (written.Length == 0)
        {
            return null;
        }

        foreach (LRegister register in LEngineRegisterFind(string.Empty, language))
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
}
