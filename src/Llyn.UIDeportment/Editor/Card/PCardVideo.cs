using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    public ObservableCollection<QVideoItem> PCardVideo { get; } = [];

    internal void PCardVideoShow(IReadOnlyList<CVideoDraft> rows)
    {
        PCardRowShow(
            PCardVideo,
            rows,
            static row => row.QVideoItemId,
            static draft => draft.CVideoDraftId,
            static draft => new QVideoItem(draft),
            (row, draft) =>
            {
                row.QVideoItemShow(draft);
                return row;
            });
    }
}
