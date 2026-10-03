using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEngineMarkup
{
    internal static void TEngineMarkupExport(this LEngine engine, IReadOnlyList<long> ids, string path)
    {
        engine.LEngineStaffHeld.LEngineStaffMarkup.LMarkupClerkExport(ids, path);
    }

    internal static LMarkupCargo TEngineMarkupRead(this LEngine engine, string path) =>
        engine.LEngineMarkup.LEngineMarkupRead(path);

    internal static IReadOnlyList<LMarkupTarget> TEngineMarkupFind(this LEngine engine, LMarkupEntry entry) =>
        engine.LEngineMarkup.LEngineMarkupFind([entry])[0];

    internal static TMarkupOutcome TEngineMarkupImport(
        this LEngine engine, LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)
    {
        List<HashSet<long>> held = [];
        foreach (LMarkupEntry entry in cargo.LMarkupCargoEntry)
        {
            held.Add([.. engine.TEngineMarkupFind(entry).Select(static found => found.LMarkupTargetId)]);
        }

        LMarkupOutcome outcome = engine.LEngineMarkup.LEngineMarkupImport(cargo, intakes);

        HashSet<long> taken = [];
        List<LEntry> stored = [];
        for (int index = 0; index < cargo.LMarkupCargoEntry.Count; index++)
        {
            long target = intakes.Single(intake => intake.LMarkupIntakeIndex == index).LMarkupIntakeTarget;
            long id = target > 0
                ? target
                : engine.TEngineMarkupFind(cargo.LMarkupCargoEntry[index])
                    .Select(static found => found.LMarkupTargetId)
                    .Where(found => !held[index].Contains(found) && !taken.Contains(found))
                    .Min();
            taken.Add(id);
            stored.Add(engine.TEngineEntryRead(id)!);
        }

        return new TMarkupOutcome(stored, outcome.LMarkupOutcomeOmission);
    }
}
