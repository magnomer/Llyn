using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal static class QChronicle
{
    private static readonly ConditionalWeakTable<DependencyObject, QChronicleHost> QChronicleHold = [];

    internal static void QChronicleIntroduce(DependencyObject surface, QChronicleHost host)
    {
        QChronicleHold.AddOrUpdate(surface, host);
    }

    internal static QChronicleHost? QChronicleRead(DependencyObject element)
    {
        return QChronicleHold.TryGetValue(element, out QChronicleHost? host) ? host : null;
    }

    internal static void QChronicleCaretRefine(Action step)
    {
        TextBox? focused = Keyboard.FocusedElement as TextBox;
        step();
        if (focused is not null && ReferenceEquals(Keyboard.FocusedElement, focused))
        {
            focused.CaretIndex = focused.Text.Length;
        }
    }

    private static QChronicleHost? QChronicleFocusRead()
    {
        DependencyObject? current = Keyboard.FocusedElement as DependencyObject;
        while (current is not null)
        {
            if (current is QChronicleHost host)
            {
                return host;
            }

            if (QChronicleRead(current) is QChronicleHost driver)
            {
                return driver;
            }

            current = current is Visual
                ? VisualTreeHelper.GetParent(current)
                : LogicalTreeHelper.GetParent(current);
        }

        return null;
    }

    internal static void QChronicleKeyObserve(object sender, KeyEventArgs e)
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

        if (QChronicleFocusRead() is not QChronicleHost host)
        {
            return;
        }

        if (undo)
        {
            host.QChronicleUndoObserve();
        }
        else
        {
            host.QChronicleRedoObserve();
        }

        e.Handled = true;
    }
}
