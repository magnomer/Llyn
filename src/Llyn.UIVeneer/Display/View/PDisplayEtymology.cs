using System.Collections.Generic;
using System.Windows.Input;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayEtymologyShow(LEntryDraft draft)
    {
        PDisplayEtymology.PEtymologyLanguage = draft.LEntryDraftLanguage;
        PDisplayEtymology.PEtymologyText = draft.LEntryDraftEtymology.LEtymologyDraftText;
        PDisplayEtymologyApply(draft, _lLectern.LLecternEtymonRead(draft));
        PDisplayEtymologySection.Visibility = PLook.PLookVisibleRead(draft.LEntryDraftDerived);
    }

    private void PDisplayEtymologyApply(LEntryDraft draft, IReadOnlyList<LTranslationTarget> etymons)
    {
        PDisplayEtymology.PEtymologySourceShow(etymons);
        PDisplayEtymology.Visibility = PLook.PLookVisibleRead(
            LLectern.LLecternEtymologyCheck(draft.LEntryDraftEtymology.LEtymologyDraftText, etymons.Count));
    }

    private void PDisplayEtymologyHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _pDisplayHost.PWindowEntryShow(((PEtymon)e.Parameter).PEtymonId);
    }
}
