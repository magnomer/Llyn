using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PImage> PCardImage { get; }

    internal void PCardImageShow(IReadOnlyList<LStateValue> locations)
    {
        PCardImage.Clear();
        foreach (LStateValue location in locations)
        {
            PCardImage.Add(new PImage(location));
        }
    }

    internal IReadOnlyList<LStateValue> PCardImageRead()
    {
        List<LStateValue> locations = [];
        foreach (PImage row in PCardImage)
        {
            LStateValue location = row.PImageLocationRead();
            if (location.LStateValueEmpty)
            {
                continue;
            }

            locations.Add(location);
        }

        return locations;
    }

    internal void PCardImageAdd()
    {
        PCardImage.Add(new PImage());
    }

    internal void PCardImageRemove(PImage row)
    {
        PCardImage.Remove(row);
    }
}
