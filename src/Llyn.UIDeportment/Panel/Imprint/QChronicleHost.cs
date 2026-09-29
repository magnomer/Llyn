using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal interface QChronicleHost
{
    void QChronicleUndoObserve();

    void QChronicleRedoObserve();
}

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
}
