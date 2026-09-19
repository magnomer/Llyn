using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LTallyLine(
    string LTallyLineLanguage,
    string LTallyLineKind,
    IReadOnlyList<LTallyMark> LTallyLineIpa,
    IReadOnlyList<LTallyMark> LTallyLineRespelling)
{
    public string LTallyLineLanguage { get; init; } = LTallyLineLanguage ?? string.Empty;

    public string LTallyLineKind { get; init; } = LTallyLineKind ?? string.Empty;

    public IReadOnlyList<LTallyMark> LTallyLineIpa { get; init; } = LTallyLineIpa ?? [];

    public IReadOnlyList<LTallyMark> LTallyLineRespelling { get; init; } = LTallyLineRespelling ?? [];

    public IReadOnlyList<LTallyMark> LTallyLineRead(bool respelled)
    {
        return respelled ? LTallyLineRespelling : LTallyLineIpa;
    }

    public static IReadOnlyList<LTallyLine> LTallyLineScan(
        string kind,
        IReadOnlyList<string> characters,
        IReadOnlyDictionary<string, IReadOnlyList<LReflex>> readings,
        IReadOnlyList<string> ranking)
    {
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(characters);
        ArgumentNullException.ThrowIfNull(readings);
        ArgumentNullException.ThrowIfNull(ranking);

        Dictionary<(string, string), Dictionary<string, List<string>>> ipa = [];
        Dictionary<(string, string), Dictionary<string, List<string>>> respelling = [];
        List<(string, string)> keys = [];
        foreach (string character in characters)
        {
            if (!readings.TryGetValue(character, out IReadOnlyList<LReflex>? reflexes))
            {
                continue;
            }

            foreach (LReflex reflex in reflexes)
            {
                (string, string) key = (reflex.LReflexLanguage.Trim(), reflex.LReflexKind.Trim());
                if (key.Item1.Length == 0)
                {
                    continue;
                }

                if (!keys.Contains(key))
                {
                    keys.Add(key);
                }

                LTallyLineAdd(ipa, key, reflex.LReflexAnatomy.LAnatomyIpaRead(kind), character);
                LTallyLineAdd(respelling, key, reflex.LReflexAnatomy.LAnatomyRespellingRead(kind), character);
            }
        }

        Func<string, int> rank = language =>
        {
            for (int index = 0; index < ranking.Count; index++)
            {
                if (string.Equals(ranking[index], language, StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }

            return ranking.Count;
        };

        List<(string, string)> ordered = [.. keys];
        ordered.Sort((left, right) =>
        {
            int order = rank(left.Item1).CompareTo(rank(right.Item1));
            if (order != 0)
            {
                return order;
            }

            order = string.Compare(left.Item1, right.Item1, StringComparison.OrdinalIgnoreCase);
            return order != 0 ? order : keys.IndexOf(left).CompareTo(keys.IndexOf(right));
        });

        List<LTallyLine> lines = new(ordered.Count);
        foreach ((string, string) line in ordered)
        {
            IReadOnlyList<LTallyMark> spoken = LTallyMark.LTallyMarkScan(ipa.GetValueOrDefault(line));
            IReadOnlyList<LTallyMark> spelled = LTallyMark.LTallyMarkScan(respelling.GetValueOrDefault(line));
            if (spoken.Count > 0 || spelled.Count > 0)
            {
                lines.Add(new LTallyLine(line.Item1, line.Item2, spoken, spelled));
            }
        }

        return lines;
    }

    private static void LTallyLineAdd(
        Dictionary<(string, string), Dictionary<string, List<string>>> marks,
        (string, string) key,
        string text,
        string character)
    {
        if (text.Length == 0)
        {
            return;
        }

        if (!marks.TryGetValue(key, out Dictionary<string, List<string>>? parts))
        {
            parts = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            marks[key] = parts;
        }

        if (!parts.TryGetValue(text, out List<string>? held))
        {
            held = [];
            parts[text] = held;
        }

        if (!held.Contains(character))
        {
            held.Add(character);
        }
    }
}
