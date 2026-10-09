using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LMarkupFacade
{
    private readonly LEngineHearth _lMarkupFacadeHearth;
    private readonly object _lMarkupFacadeGate;

    public LMarkupFacade(LEngineHearth hearth)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        _lMarkupFacadeHearth = hearth;

        _lMarkupFacadeGate = _lMarkupFacadeHearth.LEngineGate;
    }

    private LEngineStaff LMarkupFacadeStaff => _lMarkupFacadeHearth.LEngineStaffHeld;

    public LMarkupCargo LEngineMarkupRead(string path)
    {
        return LMarkupFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffMarkup.LMarkupClerkRead(path);
    }

    public IReadOnlyList<IReadOnlyList<LMarkupTarget>> LEngineMarkupFind(IReadOnlyList<LMarkupEntry> entries)
    {
        lock (_lMarkupFacadeGate)
        {
            return LMarkupFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffIntake.LMarkupTargetFind(entries);
        }
    }

    public async Task<IReadOnlyList<LMarkupOmission>?> LEngineMarkupStart(
        string path,
        Func<
            IReadOnlyList<LMarkupEntry>,
            IReadOnlyList<IReadOnlyList<LMarkupTarget>>,
            IReadOnlyList<LMarkupIntake>?> declare)
    {
        ArgumentNullException.ThrowIfNull(declare);

        LMarkupCargo cargo = await Task.Run(() => LEngineMarkupRead(path)).ConfigureAwait(true);
        IReadOnlyList<IReadOnlyList<LMarkupTarget>> targets =
            await Task.Run(() => LEngineMarkupFind(cargo.LMarkupCargoEntry)).ConfigureAwait(true);
        if (declare(cargo.LMarkupCargoEntry, targets) is not IReadOnlyList<LMarkupIntake> intakes)
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
            _lMarkupFacadeHearth.LEngineRevision++;
            return LMarkupFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffIntake.LMarkupClerkImport(cargo, intakes);
        }
    }
}
