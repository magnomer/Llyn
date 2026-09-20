using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LMarkupCargo LEngineMarkupRead(string path)
    {
        return _lEngineMarkupClerk.LMarkupClerkRead(path);
    }

    public Task<LMarkupCargo> LEngineMarkupStart(string path)
    {
        return Task.Run(() => LEngineMarkupRead(path));
    }

    public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryHeadwordFind(entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage);
        }
    }

    public Task<LMarkupOutcome> LEngineMarkupStart(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        return Task.Run(() => LEngineMarkupImport(cargo, intakes));
    }

    public LMarkupOutcome LEngineMarkupImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        lock (_lEngineGate)
        {
            return _lEngineMarkupIntake.LMarkupClerkImport(cargo, intakes);
        }
    }

    internal void LEngineMarkupExport(IReadOnlyList<long> ids, string path)
    {
        lock (_lEngineGate)
        {
            _lEngineMarkupClerk.LMarkupClerkExport(ids, path);
        }
    }
}
