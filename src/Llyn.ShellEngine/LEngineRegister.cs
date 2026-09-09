using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LRegister> LEngineRegisterRead(string ownerId, LOwner owner)
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
            IReadOnlyDictionary<string, int> counts = archive.LRegisterReferenceRead();

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

    public void LEngineRegisterChange(string registerId, string renamed)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(registerId);
            ArgumentNullException.ThrowIfNull(renamed);

            new LRegisterArchive(_lEngineDatabase).LRegisterNameUpdate(
                registerId, LStateValue.LStateValueRead(renamed.Trim()));
        }

        LEngineBulletinRaise(LSubject.LSubjectRegister, registerId);
    }

    public void LEngineRegisterDelete(string registerId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(registerId);
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
        string ownerId, IReadOnlyList<LRegisterDraft> drafts, string language, bool collocation)
    {
        LEngineRegisterCreate(language);

        LRegisterArchive registers = new(_lEngineDatabase);

        IReadOnlyList<LRegister> attached = collocation
            ? registers.LRegisterCollocationRead(ownerId)
            : registers.LRegisterMeaningRead(ownerId);

        List<string> targets = [];
        HashSet<string> kept = new(StringComparer.Ordinal);
        foreach (LRegisterDraft draft in LEngineRegisterRead(drafts))
        {
            string id = LEngineRegisterResolve(registers, draft);
            if (!kept.Add(id))
            {
                continue;
            }

            targets.Add(id);
        }

        foreach (LRegister row in attached)
        {
            if (kept.Contains(row.LRegisterId))
            {
                continue;
            }

            if (collocation)
            {
                registers.LRegisterCollocationDetach(ownerId, row.LRegisterId);
                continue;
            }

            registers.LRegisterMeaningDetach(ownerId, row.LRegisterId);
        }

        for (int position = 0; position < targets.Count; position++)
        {
            if (collocation)
            {
                registers.LRegisterCollocationAttach(ownerId, targets[position], position);
                continue;
            }

            registers.LRegisterMeaningAttach(ownerId, targets[position], position);
        }
    }

    private void LEngineRegisterAttach(
        string ownerId, IReadOnlyList<LRegisterDraft> drafts, string language, bool collocation)
    {
        LEngineRegisterCreate(language);

        LRegisterArchive registers = new(_lEngineDatabase);
        int position = 0;
        HashSet<string> attached = new(StringComparer.Ordinal);
        foreach (LRegisterDraft draft in LEngineRegisterRead(drafts))
        {
            string registerId = LEngineRegisterResolve(registers, draft);
            if (!attached.Add(registerId))
            {
                continue;
            }

            if (collocation)
            {
                registers.LRegisterCollocationAttach(ownerId, registerId, position);
            }
            else
            {
                registers.LRegisterMeaningAttach(ownerId, registerId, position);
            }

            position++;
        }
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
            if (one[index].LRegisterDraftText != other[index].LRegisterDraftText
                || !string.Equals(
                    one[index].LRegisterDraftId, other[index].LRegisterDraftId, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineRegisterCheck(IReadOnlyList<LRegisterDraft> drafts)
    {
        foreach (LRegisterDraft draft in drafts)
        {
            if (!draft.LRegisterDraftText.LStateValueEmpty)
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<LRegisterDraft> LEngineRegisterRead(IReadOnlyList<LRegisterDraft> drafts)
    {
        foreach (LRegisterDraft draft in drafts)
        {
            if (!draft.LRegisterDraftText.LStateValueEmpty)
            {
                yield return draft;
            }
        }
    }

    private static string LEngineRegisterResolve(LRegisterArchive registers, LRegisterDraft draft)
    {
        if (!string.IsNullOrWhiteSpace(draft.LRegisterDraftId))
        {
            LRegister? stored = registers.LRegisterRead(draft.LRegisterDraftId);
            if (stored is not null)
            {
                if (!stored.LRegisterBuiltin && stored.LRegisterName != draft.LRegisterDraftText)
                {
                    registers.LRegisterNameUpdate(stored.LRegisterId, draft.LRegisterDraftText);
                }

                return stored.LRegisterId;
            }
        }

        string written = draft.LRegisterDraftText.LStateValueShow().Trim();
        foreach (LRegister row in registers.LRegisterRead())
        {
            if (string.Equals(
                    row.LRegisterName.LStateValueShow().Trim(),
                    written,
                    StringComparison.CurrentCultureIgnoreCase))
            {
                return row.LRegisterId;
            }
        }

        return registers.LRegisterCreate(new LRegister(
            string.Empty,
            draft.LRegisterDraftText,
            string.Empty,
            false)).LRegisterId;
    }
}
