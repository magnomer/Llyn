using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    public ObservableCollection<PImage> PCardImage { get; } = [];

    internal void PCardImageShow(IReadOnlyList<CImageDraft> rows)
    {
        PCardRowShow(
            PCardImage,
            rows,
            static row => row.PImageId,
            static draft => draft.CImageDraftId,
            PCardImageCreate,
            (row, draft) =>
            {
                row.PImageShow(draft);
                return row;
            });
    }

    private PImage PCardImageCreate(CImageDraft draft)
    {
        return new PImage(_pCardAtelier, draft);
    }
}
