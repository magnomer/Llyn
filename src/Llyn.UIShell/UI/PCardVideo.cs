using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PVideo> PCardVideo { get; }

    internal void PCardVideoShow(IReadOnlyList<LVideoDraft> rows)
    {
        PCardVideo.Clear();
        foreach (LVideoDraft row in rows)
        {
            PCardVideo.Add(new PVideo(row));
        }
    }

    internal IReadOnlyList<LVideoDraft> PCardVideoRead()
    {
        List<LVideoDraft> rows = [];
        foreach (PVideo row in PCardVideo)
        {
            LVideoDraft written = row.PVideoDraftRead();
            if (written.LVideoDraftEmpty)
            {
                continue;
            }

            rows.Add(written);
        }

        return rows;
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
