using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LTally> LEngineTallyRead(LDiwei diwei)
    {
        ArgumentNullException.ThrowIfNull(diwei);

        lock (_lEngineGate)
        {
            IReadOnlyList<LFanqieRow> rows = new LDiweiArchive(_lEngineDatabase).LDiweiFanqieRead(diwei.LDiweiId);
            IReadOnlyDictionary<long, IReadOnlyList<LReflex>> anchored =
                new LReflexArchive(_lEngineDatabase).LReflexAnchorScan(diwei.LDiweiId);
            Dictionary<string, List<string>> divisions = new(StringComparer.Ordinal);
            List<string> order = [];
            Dictionary<string, Dictionary<string, IReadOnlyList<LReflex>>> readings = new(StringComparer.Ordinal);
            foreach (LFanqieRow row in rows)
            {
                if (!divisions.TryGetValue(row.LFanqieRowDivision, out List<string>? characters))
                {
                    characters = [];
                    divisions[row.LFanqieRowDivision] = characters;
                    readings[row.LFanqieRowDivision] =
                        new Dictionary<string, IReadOnlyList<LReflex>>(StringComparer.Ordinal);
                    order.Add(row.LFanqieRowDivision);
                }

                if (row.LFanqieRowCharacter.Length > 0 && !characters.Contains(row.LFanqieRowCharacter))
                {
                    characters.Add(row.LFanqieRowCharacter);
                }

                LEngineTallyLoad(readings[row.LFanqieRowDivision], row, anchored);
            }

            List<string> ranking = [];
            foreach (LReflexRule rule in LEngineReflexRead(diwei.LDiweiLanguage))
            {
                if (!ranking.Contains(rule.LReflexRuleLanguage, StringComparer.OrdinalIgnoreCase))
                {
                    ranking.Add(rule.LReflexRuleLanguage);
                }
            }

            List<LTally> tallies = new(order.Count);
            foreach (string division in order)
            {
                tallies.Add(new LTally(
                    division,
                    LTallyLine.LTallyLineScan(diwei.LDiweiKind, divisions[division], readings[division], ranking)));
            }

            return tallies;
        }
    }

    private static void LEngineTallyLoad(
        Dictionary<string, IReadOnlyList<LReflex>> readings,
        LFanqieRow row,
        IReadOnlyDictionary<long, IReadOnlyList<LReflex>> anchored)
    {
        if (row.LFanqieRowCharacter.Length == 0
            || !anchored.TryGetValue(row.LFanqieRowId, out IReadOnlyList<LReflex>? reflexes))
        {
            return;
        }

        List<LReflex> held = readings.TryGetValue(row.LFanqieRowCharacter, out IReadOnlyList<LReflex>? kept)
            ? [.. kept]
            : [];
        foreach (LReflex reflex in reflexes)
        {
            if (!held.Any(found => found.LReflexId == reflex.LReflexId))
            {
                held.Add(reflex);
            }
        }

        readings[row.LFanqieRowCharacter] = held;
    }
}
