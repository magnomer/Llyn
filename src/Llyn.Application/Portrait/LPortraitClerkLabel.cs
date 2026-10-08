using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Application;

public static class LPortraitClerkLabel
{
    public static IReadOnlyList<string> LPortraitKindRead()
    {
        return Enum.GetValues<LReferenceKind>().Select(LReference.LReferenceKindResolve).ToList();
    }

    public static LPortraitLabel LPortraitLabelCreate(IReadOnlyList<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);
        ArgumentOutOfRangeException.ThrowIfNotEqual(words.Count, 26);

        return new LPortraitLabel(
            words[0], words[1], words[2], words[3], words[4], words[5], words[6], words[7], words[8], words[9],
            words[10], words[11], words[12], words[13], words[14], words[15], words[16], words[17], words[18],
            words[19], words[20], words[21],
            new Dictionary<LUnit, string>
            {
                [LUnit.LUnitContent] = words[22],
                [LUnit.LUnitFunction] = words[23],
                [LUnit.LUnitMorpheme] = words[24],
                [LUnit.LUnitWord] = words[25],
            });
    }

    public static LPortraitLegend LPortraitLegendCreate(
        IReadOnlyList<string> words, IReadOnlyDictionary<string, string> kinds)
    {
        ArgumentNullException.ThrowIfNull(words);
        ArgumentNullException.ThrowIfNull(kinds);
        ArgumentOutOfRangeException.ThrowIfNotEqual(words.Count, 13);

        Dictionary<LReferenceKind, string> worded = [];
        foreach (LReferenceKind kind in Enum.GetValues<LReferenceKind>())
        {
            worded[kind] = kinds.GetValueOrDefault(
                LReference.LReferenceKindResolve(kind), LReference.LReferenceKindFormat(kind));
        }

        return new LPortraitLegend(
            words[0], words[1], words[2], words[3], words[4], words[5], words[6], words[7], words[8], words[9],
            words[10], words[11], words[12], worded);
    }
}
