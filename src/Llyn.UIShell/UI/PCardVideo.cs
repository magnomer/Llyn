using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PVideo> PCardVideo { get; }

    internal void PCardVideoShow(IReadOnlyList<LStateValue> locations)
    {
        PCardVideo.Clear();
        foreach (LStateValue location in locations)
        {
            PCardVideo.Add(new PVideo(location));
        }
    }

    internal IReadOnlyList<LStateValue> PCardVideoRead()
    {
        List<LStateValue> locations = [];
        foreach (PVideo row in PCardVideo)
        {
            LStateValue location = row.PVideoLocationRead();
            if (location.LStateValueEmpty)
            {
                continue;
            }

            locations.Add(location);
        }

        return locations;
    }

    internal void PCardVideoAdd()
    {
        PCardVideo.Add(new PVideo());
    }

    internal void PCardVideoRemove(PVideo row)
    {
        PCardVideo.Remove(row);
    }
}
