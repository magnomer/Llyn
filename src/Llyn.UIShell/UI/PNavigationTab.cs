using System.Windows;
using System.Windows.Controls;

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

        (Button Button, FrameworkElement Panel)[] tabs =
        [
            (PNavigationInput, PInput),
            (PNavigationLibrary, PLibrary),
            (PNavigationPhonology, PPhonology),
            (PNavigationTaxonomy, PTaxonomy),
            (PNavigationRepertoire, PRepertoire),
            (PNavigationCorpus, PCorpus),
            (PNavigationFavorite, PFavorite),
            (PNavigationDuplex, PDuplex),
            (PNavigationSettings, PSettings)
        ];

        foreach ((Button button, FrameworkElement panel) in tabs)
        {
            bool isSelected = button == selectedButton;
            button.Style = (Style)FindResource(
                isSelected ? "Theme.Navigation.Selected" : "Theme.Navigation.Button");
            panel.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    internal void PWindowEntryShow(string id)
    {
        PNavigationHandle(PNavigationLibrary, new RoutedEventArgs());
        PLibrary.PIndexEntryShow(id);
    }
}
