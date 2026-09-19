using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed partial class PCard
{
    public ObservableCollection<PImage> PCardImage { get; } = [];

    internal void PCardImageShow(IReadOnlyList<LImageDraft> rows)
    {
        PCardRowShow(
            PCardImage,
            rows,
            static row => row.PImageId,
            static draft => draft.LImageDraftId,
            PCardImageCreate,
            (row, draft) =>
            {
                row.PImageShow(draft);
                return row;
            });
    }

    private static PImage PCardImageCreate(LImageDraft draft)
    {
        return new PImage(draft);
    }
}
