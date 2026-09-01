using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIShell;

public partial class PEditor
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
