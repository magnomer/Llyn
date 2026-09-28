using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private static QChronicleHost? PWindowChronicleRead()
    {
        DependencyObject? current = Keyboard.FocusedElement as DependencyObject;
        while (current is not null)
        {
            if (current is QChronicleHost host)
            {
                return host;
            }

            if (QChronicle.QChronicleRead(current) is QChronicleHost driver)
            {
                return driver;
            }

            current = current is Visual
                ? VisualTreeHelper.GetParent(current)
                : LogicalTreeHelper.GetParent(current);
        }

        return null;
    }

    private void PChronicleKeyObserve(object sender, KeyEventArgs e)
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

        if (PWindowChronicleRead() is not QChronicleHost host)
        {
            return;
        }

        if (undo)
        {
            host.QChronicleUndo();
        }
        else
        {
            host.QChronicleRedo();
        }

        e.Handled = true;
    }
}
