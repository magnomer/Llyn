using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PCard
{
    public ObservableCollection<PVideo> PCardVideo { get; } = [];

    internal void PCardVideoShow(IReadOnlyList<CVideoDraft> rows)
    {
        PCardRowShow(
            PCardVideo,
            rows,
            static row => row.PVideoId,
            static draft => draft.CVideoDraftId,
            PCardVideoCreate,
            (row, draft) =>
            {
                row.PVideoShow(draft);
                return row;
            });
    }

    private PVideo PCardVideoCreate(CVideoDraft draft)
    {
        return new PVideo(_pCardAtelier, draft);
    }
}
