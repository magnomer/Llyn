using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PSentence
{
    public ObservableCollection<PGloss> PSentenceGloss { get; } = [];

    internal event Action<PGloss, string>? PSentenceGlossNotice;

    private void PSentenceGlossShow(IReadOnlyList<CGlossDraft> drafts)
    {
        PCard.PCardRowShow(
            PSentenceGloss,
            drafts,
            static row => row.PGlossId,
            static draft => draft.CGlossDraftId,
            PSentenceGlossCreate,
            (row, draft) =>
            {
                row.PGlossShow(draft);
                return row;
            });
    }

    private PGloss PSentenceGlossCreate(CGlossDraft draft)
    {
        PGloss row = new(PSentenceLanguageCatalog, draft);
        row.PGlossPicked += (gloss, language) => PSentenceGlossNotice?.Invoke(gloss, language);
        return row;
    }
}
