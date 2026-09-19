using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed partial class PCard
{
    public ObservableCollection<PVideo> PCardVideo { get; } = [];

    internal void PCardVideoShow(IReadOnlyList<LVideoDraft> rows)
    {
        PCardRowShow(
            PCardVideo,
            rows,
            static row => row.PVideoId,
            static draft => draft.LVideoDraftId,
            PCardVideoCreate,
            (row, draft) =>
            {
                row.PVideoShow(draft);
                return row;
            });
    }

    private PVideo PCardVideoCreate(LVideoDraft draft)
    {
        return new PVideo(_pCardEngine, draft);
    }
}
