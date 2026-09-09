using System;
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

    internal void PWindowEntryShow(string id)
    {
        if (!PLibrary.PLibraryLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationLibrary, new RoutedEventArgs());
        PLibrary.PIndexEntryShow(id);
    }

    internal void PWindowSituationShow(string id)
    {
        if (!PRepertoire.PRepertoireLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationRepertoire, new RoutedEventArgs());
        PRepertoire.PAtlasSituationShow(id);
    }

    internal void PWindowTagShow(string text)
    {
        if (!PTaxonomy.PTaxonomyLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationTaxonomy, new RoutedEventArgs());
        PTaxonomy.PDirectoryTagShow(text);
    }

    internal void PWindowRegisterShow(string id)
    {
        if (!PTenor.PTenorLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationTenor, new RoutedEventArgs());
        PTenor.PGamutRegisterShow(id);
    }

    internal void PWindowExampleShow(string id)
    {
        if (!PCorpus.PCorpusLeaveConfirm())
        {
            return;
        }

        PNavigationHandle(PNavigationCorpus, new RoutedEventArgs());
        PCorpus.PAnthologyExampleShow(id);
    }
}
