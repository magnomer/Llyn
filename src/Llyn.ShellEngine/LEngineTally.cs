using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LTally> LEngineTallyRead(LDiwei diwei)
    {
        ArgumentNullException.ThrowIfNull(diwei);

        lock (_lEngineGate)
        {
            IReadOnlyList<LFanqieRow> rows = _lEngineDiweiVault.LDiweiFanqieRead(diwei.LDiweiId);
            IReadOnlyDictionary<long, IReadOnlyList<LReflex>> anchored =
                _lEngineReflexes.LReflexAnchorScan(diwei.LDiweiId);
            LHypothesis? hypothesis = LEngineHypothesisRead(diwei.LDiweiLanguage);
            Dictionary<string, List<string>> sections = new(StringComparer.Ordinal);
            List<string> order = [];
            Dictionary<string, Dictionary<string, IReadOnlyList<LReflex>>> readings = new(StringComparer.Ordinal);
            foreach (LFanqieRow row in rows)
            {
                string heading = LEngineHeadingRead(diwei.LDiweiKind, row, hypothesis);
                if (!sections.TryGetValue(heading, out List<string>? characters))
                {
                    characters = [];
                    sections[heading] = characters;
                    readings[heading] = new Dictionary<string, IReadOnlyList<LReflex>>(StringComparer.Ordinal);
                    order.Add(heading);
                }

                if (row.LFanqieRowCharacter.Length > 0 && !characters.Contains(row.LFanqieRowCharacter))
                {
                    characters.Add(row.LFanqieRowCharacter);
                }

                LEngineTallyLoad(readings[heading], row, anchored);
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
            foreach (string heading in order)
            {
                tallies.Add(new LTally(
                    heading,
                    LTallyLine.LTallyLineScan(diwei.LDiweiKind, sections[heading], readings[heading], ranking)));
            }

            return tallies;
        }
    }

    private static string LEngineHeadingRead(string kind, LFanqieRow row, LHypothesis? hypothesis)
    {
        if (kind != LDiwei.LDiweiRime)
        {
            return row.LFanqieRowDivision;
        }

        return hypothesis?.LHypothesisPlaceFind(row.LFanqieRowInitial)?.LHypothesisPlaceName ?? string.Empty;
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
