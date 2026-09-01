using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

/// <summary>
/// Which panel the window shows: the navigation buttons pick exactly one, and the chosen button
/// wears the selected style while every other panel is collapsed out of the layout.
/// </summary>
public partial class PWindow
{
    private void PNavigationHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
        {
            return;
        }

        // Leaving the list panel while it is being written in is leaving the editing state, so it is
        // asked about here: a tab that is switched away from keeps its editor, but the correction the
        // user typed would sit out of sight until they came back to it.
        if (PList.IsVisible && selectedButton != PNavigationList && !PWindowDiscardConfirm(PList.PListChangeCheck()))
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
}
