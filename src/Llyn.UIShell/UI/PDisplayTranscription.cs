using System.Collections.ObjectModel;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private readonly ObservableCollection<PTranscriptionItem> _pDisplayTranscription = [];

    private void PDisplayTranscriptionShow(LEntryDraft draft)
    {
        _pDisplayTranscription.Clear();
        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (!spelled.LTranscriptionDraftEmpty)
            {
                _pDisplayTranscription.Add(PTranscriptionItem.PTranscriptionItemCreate(_pDisplayHost, spelled));
            }
        }
    }
}
