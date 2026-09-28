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

    public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        lock (_lMarkupFacadeGate)
        {
            return LMarkupFacadeStaff.LEngineStaffEntry.LEntryHeadwordFind(
                entry.LMarkupEntryHeadword, entry.LMarkupEntryLanguage);
        }
    }

    public async Task<IReadOnlyList<LMarkupOmission>?> LEngineMarkupStart(
        string path, Func<IReadOnlyList<LMarkupEntry>, IReadOnlyList<LMarkupIntake>?> declare)
    {
        ArgumentNullException.ThrowIfNull(declare);

        LMarkupCargo cargo = await Task.Run(() => LEngineMarkupRead(path)).ConfigureAwait(true);
        if (declare(cargo.LMarkupCargoEntry) is not IReadOnlyList<LMarkupIntake> intakes)
        {
            return null;
        }

        LMarkupOutcome outcome = await Task.Run(() => LEngineMarkupImport(cargo, intakes)).ConfigureAwait(false);
        return outcome.LMarkupOutcomeOmission;
    }

    public LMarkupOutcome LEngineMarkupImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        lock (_lMarkupFacadeGate)
        {
            return LMarkupFacadeStaff.LEngineStaffIntake.LMarkupClerkImport(cargo, intakes);
        }
    }
}
