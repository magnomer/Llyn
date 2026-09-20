using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal void LEngineMarkupExport(IReadOnlyList<long> ids, string path)
    {
        ArgumentNullException.ThrowIfNull(ids);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string text;
        lock (_lEngineGate)
        {
            using LVaultSession session = _lEngineVault.LVaultSessionStart();

            List<LMarkupEntry> entries = new(ids.Count);
            foreach (long id in ids)
            {
                entries.Add(LEngineMarkupCreate(id));
            }

            text = LMarkup.LMarkupFormat(entries);
        }

        _lEngineMarkupVault.LMarkupSave(path, text);
    }

    private LMarkupEntry LEngineMarkupCreate(long id)
    {
        LMarkupClerk clerk = new LMarkupClerk(
            _lEngineEntries,
            _lEngineSpeeches,
            _lEngineMorphologies,
            _lEngineMeanings,
            _lEngineReferences,
            _lEngineAuthors);
        return clerk.LMarkupLoad(id)
            ?? throw new LRefusal(LRefusal.LRefusalEntry);
    }
}
