using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed partial class PCard
{
    public ObservableCollection<PImage> PCardImage { get; }

    internal void PCardImageShow(IReadOnlyList<LImageDraft> rows)
    {
        PCardImage.Clear();
        foreach (LImageDraft row in rows)
        {
            PCardImage.Add(new PImage(row));
        }
    }

    internal IReadOnlyList<LImageDraft> PCardImageRead()
    {
        List<LImageDraft> rows = [];
        foreach (PImage row in PCardImage)
        {
            LImageDraft written = row.PImageDraftRead();
            if (written.LImageDraftEmpty)
            {
                continue;
            }

            rows.Add(written);
        }

        return rows;
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
