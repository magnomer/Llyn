using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed record QLeafImage(QImageItem QLeafImageRow, bool QLeafImageEmpty)
{
    internal static IReadOnlyList<QLeafImage> QLeafImageCreate(IReadOnlyList<CImageDraft> drafts)
    {
        List<QLeafImage> rows = new(drafts.Count);
        foreach (CImageDraft draft in drafts)
        {
            rows.Add(new QLeafImage(new QImageItem(draft), draft.CImageDraftEmpty));
        }

        return rows;
    }
}
