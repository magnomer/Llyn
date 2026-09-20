using System;
using System.Collections.Generic;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    private void PNavigationHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not PTab selectedButton)
        {
            return;
        }

        if (PLibrary.IsVisible && selectedButton != PNavigationLibrary
            && !PWindowDiscardConfirm(PLibrary.PLibraryChangeCheck(), PLibrary.PLibraryDraftFinish))
        {
            return;
        }

        if (PPhonology.IsVisible && selectedButton != PNavigationPhonology
            && !PWindowDiscardConfirm(PPhonology.PPhonologyChangeCheck(), PPhonology.PPhonologyDraftFinish))
        {
            return;
        }

        if (PYunjing.IsVisible && selectedButton != PNavigationYunjing
            && !PWindowDiscardConfirm(PYunjing.PYunjingChangeCheck(), PYunjing.PYunjingDraftFinish))
        {
            return;
        }

        if (PTaxonomy.IsVisible && selectedButton != PNavigationTaxonomy
            && !PWindowDiscardConfirm(PTaxonomy.PTaxonomyChangeCheck(), PTaxonomy.PTaxonomyDraftFinish))
        {
            return;
        }

        if (PTenor.IsVisible && selectedButton != PNavigationTenor
            && !PWindowDiscardConfirm(PTenor.PTenorChangeCheck(), PTenor.PTenorDraftFinish))
        {
            return;
        }

        if (PRepertoire.IsVisible && selectedButton != PNavigationRepertoire
            && !PWindowDiscardConfirm(PRepertoire.PRepertoireChangeCheck(), PRepertoire.PRepertoireDraftFinish))
        {
            return;
        }

        if (PCorpus.IsVisible && selectedButton != PNavigationCorpus
            && !PWindowDiscardConfirm(PCorpus.PCorpusChangeCheck(), PCorpus.PCorpusDraftFinish))
        {
            return;
        }

        if (PReference.IsVisible && selectedButton != PNavigationSource
            && !PWindowDiscardConfirm(PReference.PReferenceChangeCheck(), PReference.PReferenceDraftFinish))
        {
            return;
        }

        if (PGuild.IsVisible && selectedButton != PNavigationGuild
            && !PWindowDiscardConfirm(PGuild.PGuildChangeCheck(), PGuild.PGuildDraftFinish))
        {
            return;
        }

        if (PFavorite.IsVisible && selectedButton != PNavigationFavorite
            && !PWindowDiscardConfirm(PFavorite.PFavoriteChangeCheck(), PFavorite.PFavoriteDraftFinish))
        {
            return;
        }

        foreach ((string mode, PTab button, FrameworkElement panel, Action<bool>? scribe) in PNavigationTabRead())
        {
            bool isSelected = button == selectedButton;
            button.PTabChosen = isSelected;
            panel.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;

            if (isSelected)
            {
                _lWindow.LWindowModeSave(mode);
            }
        }
    }

    internal void PNavigationRestore()
    {
        foreach ((string mode, PTab button, FrameworkElement panel, Action<bool>? scribe) in PNavigationTabRead())
        {
            if (!_lWindow.LWindowModeMatch(mode))
            {
                continue;
            }

            PNavigationHandle(button, new RoutedEventArgs());
            scribe?.Invoke(_lWindow.LWindowPostureRead().LPostureStateSplit);
            return;
        }
    }

    private (string PNavigationMode, PTab PNavigationButton, FrameworkElement PNavigationPanel,
        Action<bool>? PNavigationScribe)[] PNavigationTabRead()
    {
        return
        [
            ("Input", PNavigationInput, PInput, null),
            ("Library", PNavigationLibrary, PLibrary, PLibrary.PLibraryScribeRestore),
            ("Phonology", PNavigationPhonology, PPhonology, PPhonology.PPhonologyScribeRestore),
            ("Yunjing", PNavigationYunjing, PYunjing, PYunjing.PYunjingScribeRestore),
            ("Taxonomy", PNavigationTaxonomy, PTaxonomy, PTaxonomy.PTaxonomyScribeRestore),
            ("Tenor", PNavigationTenor, PTenor, PTenor.PTenorScribeRestore),
            ("Repertoire", PNavigationRepertoire, PRepertoire, PRepertoire.PRepertoireScribeRestore),
            ("Corpus", PNavigationCorpus, PCorpus, PCorpus.PCorpusScribeRestore),
            ("Reference", PNavigationSource, PReference, PReference.PReferenceScribeRestore),
            ("Guild", PNavigationGuild, PGuild, PGuild.PGuildScribeRestore),
            ("Favorite", PNavigationFavorite, PFavorite, PFavorite.PFavoriteScribeRestore),
            ("Duplex", PNavigationDuplex, PDuplex, null),
            ("Settings", PNavigationSettings, PSettings, null)
        ];
    }

    internal bool PWindowEntryShow(long id)
    {
        PMentionMenuHide();
        if (!PLibrary.PLibraryLeaveConfirm())
        {
            return false;
        }

        PVoyageRecord();
        PNavigationHandle(PNavigationLibrary, new RoutedEventArgs());
        PLibrary.PIndexEntryShow(id);
        return true;
    }

    internal void PWindowMentionHandle(PMention anchor, LMentionResult result)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(result);

        PMentionMenuHide();

        if (result.LMentionResultStored is LMention stored)
        {
            if (!stored.LMentionLinked)
            {
                return;
            }

            PWindowEntryShow(stored.LMentionEntryId);
            if (stored.LMentionSensed)
            {
                PLibrary.PDisplay.PDisplayCardScroll(stored.LMentionSenseId);
            }

            return;
        }

        if (result.LMentionResultSingle)
        {
            PWindowEntryShow(result.LMentionResultFirst);
            return;
        }

        if (result.LMentionResultMany)
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
            meanings = _lWindow.LWindowMeaningRead(entryId);
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

    internal bool PWindowSituationShow(long id)
    {
        if (!PRepertoire.PRepertoireLeaveConfirm())
        {
            return false;
        }

        PVoyageRecord();
        PNavigationHandle(PNavigationRepertoire, new RoutedEventArgs());
        PRepertoire.PAtlasSituationShow(id);
        return true;
    }

    internal bool PWindowTagShow(long id)
    {
        if (!PTaxonomy.PTaxonomyLeaveConfirm())
        {
            return false;
        }

        PVoyageRecord();
        PNavigationHandle(PNavigationTaxonomy, new RoutedEventArgs());
        PTaxonomy.PDirectoryTagShow(id);
        return true;
    }

    internal bool PWindowExampleShow(long id)
    {
        if (!PCorpus.PCorpusLeaveConfirm())
        {
            return false;
        }

        PVoyageRecord();
        PNavigationHandle(PNavigationCorpus, new RoutedEventArgs());
        PCorpus.PAnthologyExampleShow(id);
        return true;
    }

    internal bool PWindowRegisterShow(long id)
    {
        if (!PTenor.PTenorLeaveConfirm())
        {
            return false;
        }

        PVoyageRecord();
        PNavigationHandle(PNavigationTenor, new RoutedEventArgs());
        PTenor.PGamutRegisterShow(id);
        return true;
    }
}
