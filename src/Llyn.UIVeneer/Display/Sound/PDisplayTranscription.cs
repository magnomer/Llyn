using System.Collections.Generic;
using System.Collections.ObjectModel;
using Llyn.Core;

using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private readonly ObservableCollection<LTranscriptionItem> _pDisplayTranscription = [];

    private void PDisplayTranscriptionShow(LEntryDraft draft)
    {
        _pDisplayTranscription.Clear();
        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (spelled.LTranscriptionDraftEmpty)
            {
                continue;
            }

            if (PDisplayGlyphCheck(spelled.LTranscriptionDraftScheme))
            {
                continue;
            }

            _pDisplayTranscription.Add(LTranscriptionItem.LTranscriptionItemCreate(spelled));
        }
    }
}
