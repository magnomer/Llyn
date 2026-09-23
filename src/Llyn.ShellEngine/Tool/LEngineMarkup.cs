using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LMarkupCargo LEngineMarkupRead(string path)
    {
        return _lEngineStaff.LEngineStaffMarkup.LMarkupClerkRead(path);
    }

    public Task<LMarkupCargo> LEngineMarkupStart(string path)
    {
        return Task.Run(() => LEngineMarkupRead(path));
    }

    public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffEntry.LEntryHeadwordFind(
                entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage);
        }
    }

    public Task<LMarkupOutcome> LEngineMarkupStart(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        return Task.Run(() => LEngineMarkupImport(cargo, intakes));
    }

    public LMarkupOutcome LEngineMarkupImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffIntake.LMarkupClerkImport(cargo, intakes);
        }
    }

    internal void LEngineMarkupExport(IReadOnlyList<long> ids, string path)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffMarkup.LMarkupClerkExport(ids, path);
        }
    }
}
