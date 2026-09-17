using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIShell;

public partial class PWindow
{
    private static PChronicleHost? PChronicleHostRead()
    {
        DependencyObject? current = Keyboard.FocusedElement as DependencyObject;
        while (current is not null)
        {
            if (current is PChronicleHost host)
            {
                return host;
            }

            current = current is Visual
                ? VisualTreeHelper.GetParent(current)
                : LogicalTreeHelper.GetParent(current);
        }

        return null;
    }

    private void PChronicleKeyHandle(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers is not (ModifierKeys.Control or (ModifierKeys.Control | ModifierKeys.Shift)))
        {
            return;
        }

        bool shift = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
        bool undo = e.Key == Key.Z && !shift;
        bool redo = e.Key == Key.Y && !shift || e.Key == Key.Z && shift;
        if (!undo && !redo)
        {
            return;
        }

        if (PChronicleHostRead() is not PChronicleHost host)
        {
            return;
        }

        if (undo)
        {
            host.PChronicleUndo();
        }
        else
        {
            host.PChronicleRedo();
        }

        e.Handled = true;
    }
}
