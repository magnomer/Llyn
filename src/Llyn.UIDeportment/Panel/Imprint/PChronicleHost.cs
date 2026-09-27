using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal interface PChronicleHost
{
    void PChronicleUndo();

    void PChronicleRedo();

    void PChronicleUpdate();
}

internal static class PChronicle
{
    private static readonly ConditionalWeakTable<DependencyObject, PChronicleHost> PChronicleHold = [];

    internal static void PChronicleAttach(DependencyObject surface, PChronicleHost host)
    {
        PChronicleHold.AddOrUpdate(surface, host);
    }

    internal static PChronicleHost? PChronicleRead(DependencyObject element)
    {
        return PChronicleHold.TryGetValue(element, out PChronicleHost? host) ? host : null;
    }

    internal static void PChronicleRun(Action step)
    {
        TextBox? focused = Keyboard.FocusedElement as TextBox;
        step();
        if (focused is not null && ReferenceEquals(Keyboard.FocusedElement, focused))
        {
            focused.CaretIndex = focused.Text.Length;
        }
    }
}
