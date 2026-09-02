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

        if (PList.IsVisible && selectedButton != PNavigationList && !PWindowDiscardConfirm(PList.PListChangeCheck()))
        {
            return;
        }

        if (PSound.IsVisible && selectedButton != PNavigationSound && !PWindowDiscardConfirm(PSound.PSoundChangeCheck()))
        {
            return;
        }

        if (PTag.IsVisible && selectedButton != PNavigationTag && !PWindowDiscardConfirm(PTag.PTagChangeCheck()))
        {
            return;
        }

        if (PSituation.IsVisible && selectedButton != PNavigationSituation
            && !PWindowDiscardConfirm(PSituation.PSituationChangeCheck()))
        {
            return;
        }

        (Button Button, FrameworkElement Panel)[] tabs =
        [
            (PNavigationInput, PInput),
            (PNavigationList, PList),
            (PNavigationSound, PSound),
            (PNavigationTag, PTag),
            (PNavigationSituation, PSituation),
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
        PNavigationHandle(PNavigationList, new RoutedEventArgs());
        PList.PIndexEntryShow(id);
    }
}
