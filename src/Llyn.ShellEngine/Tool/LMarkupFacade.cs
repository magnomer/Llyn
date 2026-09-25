using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LMarkupFacade
{
    private readonly LEngine _lMarkupFacadeEngine;
    private readonly object _lMarkupFacadeGate;

    public LMarkupFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lMarkupFacadeEngine = engine;
        _lMarkupFacadeGate = engine.LEngineGate;
    }

    private LEngineStaff LMarkupFacadeStaff => _lMarkupFacadeEngine.LEngineStaffHeld;

    public LMarkupCargo LEngineMarkupRead(string path)
    {
        return LMarkupFacadeStaff.LEngineStaffMarkup.LMarkupClerkRead(path);
    }

    public Task<LMarkupCargo> LEngineMarkupStart(string path)
    {
        return Task.Run(() => LEngineMarkupRead(path));
    }

    public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_lMarkupFacadeGate)
        {
            return LMarkupFacadeStaff.LEngineStaffEntry.LEntryHeadwordFind(
                entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage);
        }
    }

    public Task<LMarkupOutcome> LEngineMarkupStart(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        return Task.Run(() => LEngineMarkupImport(cargo, intakes));
    }

    public LMarkupOutcome LEngineMarkupImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        lock (_lMarkupFacadeGate)
        {
            return LMarkupFacadeStaff.LEngineStaffIntake.LMarkupClerkImport(cargo, intakes);
        }
    }
}
