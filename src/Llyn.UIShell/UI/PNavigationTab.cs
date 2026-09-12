using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PWindow
{
    private void PNavigationHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
        {
            return;
        }

        if (PLibrary.IsVisible && selectedButton != PNavigationLibrary && !PWindowDiscardConfirm(PLibrary.PLibraryChangeCheck()))
        {
            return;
        }

        if (PPhonology.IsVisible && selectedButton != PNavigationPhonology && !PWindowDiscardConfirm(PPhonology.PPhonologyChangeCheck()))
        {
            return;
        }

        if (PTaxonomy.IsVisible && selectedButton != PNavigationTaxonomy && !PWindowDiscardConfirm(PTaxonomy.PTaxonomyChangeCheck()))
        {
            return;
        }

        if (PTenor.IsVisible && selectedButton != PNavigationTenor && !PWindowDiscardConfirm(PTenor.PTenorChangeCheck()))
        {
            return;
        }

        if (PRepertoire.IsVisible && selectedButton != PNavigationRepertoire
            && !PWindowDiscardConfirm(PRepertoire.PRepertoireChangeCheck()))
        {
            return;
        }

        if (PCorpus.IsVisible && selectedButton != PNavigationCorpus
            && !PWindowDiscardConfirm(PCorpus.PCorpusChangeCheck()))
        {
            return;
        }

        if (PReference.IsVisible && selectedButton != PNavigationSource
            && !PWindowDiscardConfirm(PReference.PReferenceChangeCheck()))
        {
            return;
        }

        if (PFavorite.IsVisible && selectedButton != PNavigationFavorite
            && !PWindowDiscardConfirm(PFavorite.PFavoriteChangeCheck()))
        {
            return;
        }

        foreach ((string mode, Button button, FrameworkElement panel, Action<bool>? scribe) in PNavigationTabRead())
        {
            bool isSelected = button == selectedButton;
            button.Style = (Style)FindResource(
                isSelected ? "Theme.Navigation.Selected" : "Theme.Navigation.Button");
            panel.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;

            if (isSelected)
            {
                _lEngine.LEngineModeSave(mode);
            }
        }
    }

    internal void PNavigationRestore(LWorkspaceState state)
    {
        foreach ((string mode, Button button, FrameworkElement panel, Action<bool>? scribe) in PNavigationTabRead())
        {
            if (!string.Equals(mode, state.LWorkspaceStateMode, StringComparison.Ordinal))
            {
                continue;
            }

            PNavigationHandle(button, new RoutedEventArgs());
            scribe?.Invoke(state.LWorkspaceStateSplit);
            return;
        }
    }

    private (string PNavigationMode, Button PNavigationButton, FrameworkElement PNavigationPanel,
        Action<bool>? PNavigationScribe)[] PNavigationTabRead()
    {
        return
        [
            ("Input", PNavigationInput, PInput, null),
            ("Library", PNavigationLibrary, PLibrary, PLibrary.PLibraryScribeRestore),
            ("Phonology", PNavigationPhonology, PPhonology, PPhonology.PPhonologyScribeRestore),
            ("Taxonomy", PNavigationTaxonomy, PTaxonomy, PTaxonomy.PTaxonomyScribeRestore),
            ("Tenor", PNavigationTenor, PTenor, PTenor.PTenorScribeRestore),
            ("Repertoire", PNavigationRepertoire, PRepertoire, PRepertoire.PRepertoireScribeRestore),
            ("Corpus", PNavigationCorpus, PCorpus, PCorpus.PCorpusScribeRestore),
            ("Reference", PNavigationSource, PReference, PReference.PReferenceScribeRestore),
            ("Favorite", PNavigationFavorite, PFavorite, PFavorite.PFavoriteScribeRestore),
            ("Duplex", PNavigationDuplex, PDuplex, null),
            ("Settings", PNavigationSettings, PSettings, null)
        ];
    }

    internal void PWindowEntryShow(long id)
    {
        PMentionMenuHide();
        if (!PLibrary.PLibraryLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationLibrary, new RoutedEventArgs());
        PLibrary.PIndexEntryShow(id);
    }

    internal void PWindowMentionHandle(PMention anchor, LMentionResult result)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(result);

        PMentionMenuHide();

        if (result.LMentionResultStored is LMention stored)
        {
            if (stored.LMentionEntryId == 0)
            {
                return;
            }

            PWindowEntryShow(stored.LMentionEntryId);
            if (stored.LMentionSenseId != 0)
            {
                PLibrary.PDisplay.PDisplayCardScroll(stored.LMentionSenseId);
            }

            return;
        }

        if (result.LMentionResultEntry.Count == 1)
        {
            PWindowEntryShow(result.LMentionResultEntry[0].LTranslationTargetId);
            return;
        }

        if (result.LMentionResultEntry.Count > 1)
        {
            PMentionMenuShow(anchor, anchor.PMentionPieceRead(result.LMentionResultOffset), result);
        }
    }

    internal void PWindowSenseShow(FrameworkElement anchor, Rect place, long entryId, Action<long> chosen)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(chosen);

        IReadOnlyList<LMeaning> meanings;
        try
        {
            meanings = _lEngine.LEngineMeaningRead(entryId, LOwner.LOwnerEntry);
        }
        catch (Exception exception)
        {
            PWindowFailureShow("Mention.FindFailed", exception);
            return;
        }

        PMentionMenuShow(anchor, place, entryId, meanings, chosen);
    }

    internal void PWindowProspectShow(
        FrameworkElement anchor, Rect place, string word, string language, Action<long> chosen)
    {
        PInput.PEditor.PProspectShow(anchor, place, word, language, chosen);
    }

    private void PMentionLeaveHandle(object? sender, EventArgs e)
    {
        PMentionMenuHide();
    }

    internal void PWindowSituationShow(long id)
    {
        if (!PRepertoire.PRepertoireLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationRepertoire, new RoutedEventArgs());
        PRepertoire.PAtlasSituationShow(id);
    }

    internal void PWindowTagShow(long id)
    {
        if (!PTaxonomy.PTaxonomyLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationTaxonomy, new RoutedEventArgs());
        PTaxonomy.PDirectoryTagShow(id);
    }

    internal void PWindowRegisterShow(long id)
    {
        if (!PTenor.PTenorLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationTenor, new RoutedEventArgs());
        PTenor.PGamutRegisterShow(id);
    }

    internal void PWindowExampleShow(long id)
    {
        if (!PCorpus.PCorpusLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationCorpus, new RoutedEventArgs());
        PCorpus.PAnthologyExampleShow(id);
    }
}
