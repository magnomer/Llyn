using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Llyn.Core;
using Llyn.Infrastructure;

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
            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            List<LMarkupEntry> entries = new(ids.Count);
            foreach (long id in ids)
            {
                entries.Add(LEngineMarkupCreate(id));
            }

            text = LMarkup.LMarkupFormat(entries);
        }

        File.WriteAllText(path, text, new UTF8Encoding(false));
    }

    private LMarkupEntry LEngineMarkupCreate(long id)
    {
        return new LMarkupLoader(_lEngineDatabase).LMarkupLoad(id)
            ?? throw new LRefusal(LRefusal.LRefusalEntry);
    }
}
