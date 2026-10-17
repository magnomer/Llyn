using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Llyn.UIDemeanor;

internal readonly record struct BBlueprintTrack(int BBlueprintTrackCells, int BBlueprintTrackWeight)
{
    internal static IReadOnlyList<BBlueprintTrack> BBlueprintTrackParse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        List<BBlueprintTrack> tracks = [];
        foreach (string entry in value.Split(','))
        {
            string text = entry.Trim();
            bool share = text.EndsWith('*');
            string number = share ? text[..^1] : text;
            int count = 1;
            if ((!share || number.Length > 0)
                && (!int.TryParse(number, NumberStyles.None, CultureInfo.InvariantCulture, out count)
                    || (share && count == 0)))
            {
                throw new InvalidDataException($"The blueprint tracks '{value}' are not sizes or weights.");
            }

            tracks.Add(share ? new BBlueprintTrack(0, count) : new BBlueprintTrack(count, 0));
        }

        return tracks;
    }

    internal static int[] BBlueprintTrackDivide(IReadOnlyList<BBlueprintTrack> tracks, int length)
    {
        ArgumentNullException.ThrowIfNull(tracks);

        int[] sizes = new int[tracks.Count];
        int rest = Math.Max(length, 0);
        int weights = 0;
        int last = -1;
        for (int index = 0; index < tracks.Count; index++)
        {
            if (tracks[index].BBlueprintTrackWeight == 0)
            {
                sizes[index] = Math.Min(tracks[index].BBlueprintTrackCells, rest);
                rest -= sizes[index];
            }
            else
            {
                weights += tracks[index].BBlueprintTrackWeight;
                last = index;
            }
        }

        int shared = rest;
        for (int index = 0; index < tracks.Count; index++)
        {
            int weight = tracks[index].BBlueprintTrackWeight;
            if (weight == 0)
            {
                continue;
            }

            sizes[index] = index == last ? rest : shared * weight / weights;
            rest -= sizes[index];
        }

        return sizes;
    }
}
