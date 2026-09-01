using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

/// <summary>
/// Which region of the input editor is open: the tab strip picks one of meaning, collocation, or
/// note, and collapses the other two. The same selection shape as the navigation strip, over the
/// input panel's own contents rather than over the window's panels.
/// </summary>
public partial class PInput
{
    private void PStackHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
        {
            return;
        }

        (Button Button, FrameworkElement Contents)[] tabs =
        [
            (PStackMeaning, PMeaning),
            (PStackCollocation, PCollocation),
            (PStackNote, PNote)
        ];

        foreach ((Button button, FrameworkElement contents) in tabs)
        {
            bool isSelected = button == selectedButton;
            button.Style = (Style)FindResource(
                isSelected ? "Theme.Input.Tab.Selected" : "Theme.Input.Tab");
            contents.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
