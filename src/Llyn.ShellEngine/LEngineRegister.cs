using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal IReadOnlyList<LRegister> LEngineRegisterRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LRegisterVault registers = _lEngineRegisters;
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

            LEngineRegisterPrepare(language);

            string written = query.Trim();
            List<LRegister> found = [];
            foreach (LRegister register in _lEngineRegisters.LRegisterRead())
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
    }

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);

            LRegisterVault archive = _lEngineRegisters;
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

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        List<LCatalogRegister> rows = [];
        bool kept = false;
        foreach (LCatalogRegister row in LEngineRegisterFind(vista.LVistaQuery, vista.LVistaOrder))
        {
            bool chosen = vista.LVistaMatch(row.LCatalogRegisterStored.LRegisterId);
            kept |= chosen;
            rows.Add(row with { LCatalogRegisterChosen = chosen });
        }

        if (!kept)
        {
            vista.LVistaSelect(null);
        }

        return rows;
    }

    public LRegister LEngineRegisterCreate(string name)
    {
        LRegister created;
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            created = LDraftClerkChip.LRegisterResolve(_lEngineRegisters, name)
                ?? _lEngineRegisters.LRegisterCreate(new LRegister(
                    0,
                    LStateValue.LStateValueRead(name.Trim())));
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, created.LRegisterId);
        return created;
    }

    internal void LEngineRegisterChange(long registerId, string renamed)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registerId);
            ArgumentNullException.ThrowIfNull(renamed);

            _lEngineRegisters.LRegisterNameUpdate(
                registerId, LStateValue.LStateValueRead(renamed.Trim()));
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, registerId);
    }

    internal void LEngineRegisterDelete(long registerId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registerId);
            _lEngineRegisters.LRegisterDelete(registerId, true);
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, registerId);
    }

    private void LEngineRegisterPrepare(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return;
        }

        _lEngineRegisters.LRegisterDefaultCreate(
            _lEngineRegisters.LRegisterLoad(language));
    }

    private void LEngineRegisterSync(
        long ownerId,
        IReadOnlyList<LRegisterDraft> drafts,
        string language,
        bool collocation,
        Dictionary<long, long> identity)
    {
        LEngineRegisterPrepare(language);

        LRegisterVault registers = _lEngineRegisters;

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
        LRegisterVault registers, LRegisterDraft draft, Dictionary<long, long> identity)
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

        LRegister? found = LDraftClerkChip.LRegisterResolve(registers, draft.LRegisterDraftName.LStateValueShow());
        if (found is not null)
        {
            LEngineIdentityRecord(identity, draft.LRegisterDraftId, found.LRegisterId);
            return found.LRegisterId;
        }

        long created = registers.LRegisterCreate(new LRegister(0, draft.LRegisterDraftName)).LRegisterId;
        LEngineIdentityRecord(identity, draft.LRegisterDraftId, created);
        return created;
    }
}
