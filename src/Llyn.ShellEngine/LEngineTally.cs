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
            Dictionary<string, List<string>> divisions = new(StringComparer.Ordinal);
            List<string> order = [];
            Dictionary<string, IReadOnlyList<LReflex>> readings = new(StringComparer.Ordinal);
            foreach (LFanqieRow row in rows)
            {
                if (!divisions.TryGetValue(row.LFanqieRowDivision, out List<string>? characters))
                {
                    characters = [];
                    divisions[row.LFanqieRowDivision] = characters;
                    order.Add(row.LFanqieRowDivision);
                }

                if (row.LFanqieRowCharacter.Length > 0 && !characters.Contains(row.LFanqieRowCharacter))
                {
                    characters.Add(row.LFanqieRowCharacter);
                    LEngineTallyLoad(diwei.LDiweiLanguage, row.LFanqieRowCharacter, readings);
                }
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
                    LTallyLine.LTallyLineScan(diwei.LDiweiKind, divisions[division], readings, ranking)));
            }

            return tallies;
        }
    }

    private void LEngineTallyLoad(
        string language, string character, Dictionary<string, IReadOnlyList<LReflex>> readings)
    {
        if (readings.ContainsKey(character))
        {
            return;
        }

        List<LReflex> reflexes = [];
        LReflexArchive archive = new(_lEngineDatabase);
        foreach (LEntry entry in new LEntryArchive(_lEngineDatabase).LEntryHeadwordFind(language, character))
        {
            reflexes.AddRange(archive.LReflexRead(entry.LEntryId));
        }

        readings[character] = reflexes;
    }
}
