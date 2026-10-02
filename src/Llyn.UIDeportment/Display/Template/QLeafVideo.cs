using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed record QLeafVideo(QVideoItem QLeafVideoRow, bool QLeafVideoEmpty)
{
    internal static IReadOnlyList<QLeafVideo> QLeafVideoCreate(IReadOnlyList<CVideoDraft> drafts)
    {
        List<QLeafVideo> rows = new(drafts.Count);
        foreach (CVideoDraft draft in drafts)
        {
            rows.Add(new QLeafVideo(new QVideoItem(draft), draft.CVideoDraftEmpty));
        }

        return rows;
    }
}
