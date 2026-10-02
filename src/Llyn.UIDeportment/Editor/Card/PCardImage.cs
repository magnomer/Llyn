using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    public ObservableCollection<QImageItem> PCardImage { get; } = [];

    internal void PCardImageShow(IReadOnlyList<CImageDraft> rows)
    {
        PCardRowShow(
            PCardImage,
            rows,
            static row => row.QImageItemId,
            static draft => draft.CImageDraftId,
            static draft => new QImageItem(draft),
            (row, draft) =>
            {
                row.QImageItemShow(draft);
                return row;
            });
    }
}
