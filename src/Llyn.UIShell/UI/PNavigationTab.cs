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
