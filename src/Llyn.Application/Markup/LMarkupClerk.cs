using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMarkupClerk
{
    private readonly LVault _lMarkupClerkVault;
    private readonly LMarkupVault _lMarkupClerkMarkup;
    private readonly LLanguageVault _lMarkupClerkLanguages;
    private readonly LMarkupClerkEntry _lMarkupClerkEntry;

    public LMarkupClerk(LRig rig, LMarkupClerkEntry entry)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(entry);
        _lMarkupClerkVault = rig.LRigVault;
        _lMarkupClerkMarkup = rig.LRigMarkup;
        _lMarkupClerkLanguages = rig.LRigLanguages;
        _lMarkupClerkEntry = entry;
    }

    public LMarkupCargo LMarkupClerkRead(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        IReadOnlyList<LMarkupEntry> parsed = LMarkup.LMarkupParse(
            _lMarkupClerkMarkup.LMarkupRead(path), out IReadOnlyList<LMarkupOmission> skipped);

        List<LMarkupOmission> omissions = [.. skipped];
        List<LMarkupEntry> entries = new(parsed.Count);
        foreach (LMarkupEntry entry in parsed)
        {
            string language = entry.LMarkupEntryLanguage;
            if (language.Length == 0 || _lMarkupClerkLanguages.LLanguageNameValidate(language))
            {
                entries.Add(entry);
                continue;
            }

            omissions.Add(new LMarkupOmission(entry.LMarkupEntryLine, $"language \"{language}\""));
            entries.Add(entry with { LMarkupEntryLanguage = string.Empty });
        }

        string[] names = LEntryClerkTwin.LTwinRead(
            entries,
            entry => entry.LMarkupEntryHeadword,
            entry => entry.LMarkupEntryLanguage,
            entry => entry.LMarkupEntryLine);
        for (int index = 0; index < entries.Count; index++)
        {
            entries[index] = entries[index] with { LMarkupEntryName = names[index] };
        }

        return new LMarkupCargo(entries, omissions);
    }

    public void LMarkupClerkExport(IReadOnlyList<long> ids, string path)
    {
        ArgumentNullException.ThrowIfNull(ids);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        LMarkupNode root;
        using (LVaultSession session = _lMarkupClerkVault.LVaultSessionStart())
        {
            List<LMarkupEntry> entries = new(ids.Count);
            foreach (long id in ids)
            {
                entries.Add(_lMarkupClerkEntry.LMarkupLoad(id) ?? throw new LRefusal(LRefusal.LRefusalEntry));
            }

            root = LMarkup.LMarkupFormat(entries);
        }

        _lMarkupClerkMarkup.LMarkupSave(path, root);
    }
}
