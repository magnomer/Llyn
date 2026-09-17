using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIShell;

internal interface PChronicleHost
{
    void PChronicleUndo();

    void PChronicleRedo();

    void PChronicleUpdate();
}

internal static class PChronicle
{
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
