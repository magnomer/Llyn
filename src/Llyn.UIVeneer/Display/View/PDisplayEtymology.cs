using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayEtymologyShow(LEntryDraft draft)
    {
        LEtymologyDraft etymology = draft.LEntryDraftEtymology;
        Dictionary<long, LTranslationTarget> named = [];
        try
        {
            foreach (LTranslationTarget target in _lDisplay.LDisplayTargetRead(draft))
            {
                named[target.LTranslationTargetId] = target;
            }
        }
        catch (Exception)
        {
            named.Clear();
        }

        List<PEtymologyChip> chips = [];
        foreach (long id in etymology.LEtymologyDraftEtymons)
        {
            if (named.TryGetValue(id, out LTranslationTarget? target))
            {
                chips.Add(new PEtymologyChip(
                    id, target.LTranslationTargetHeadword, target.LTranslationTargetLanguage, false));
            }
        }

        PDisplayEtymology.PEtymologyLanguage = draft.LEntryDraftLanguage;
        PDisplayEtymology.PEtymologyText = etymology.LEtymologyDraftText;
        PDisplayEtymology.PEtymologySourceShow(chips);
        PDisplayEtymologySection.Visibility = draft.LEntryDraftDerived
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void PDisplayEtymologyHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is PEtymologyChip chip)
        {
            _pDisplayHost.PWindowEntryShow(chip.PEtymologyChipId);
        }
    }
}
