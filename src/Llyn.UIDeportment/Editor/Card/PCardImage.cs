using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    public ObservableCollection<PImage> PCardImage { get; } = [];

    internal void PCardImageShow(IReadOnlyList<CImageDraft> rows, CAtelier atelier)
    {
        PCardRowShow(
            PCardImage,
            rows,
            static row => row.PImageId,
            static draft => draft.CImageDraftId,
            draft => new PImage(atelier, draft),
            (row, draft) =>
            {
                row.PImageShow(draft);
                return row;
            });
    }
}
