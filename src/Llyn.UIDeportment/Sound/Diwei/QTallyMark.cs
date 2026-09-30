using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTallyMark
{
    private QTallyMark(CTallyMark mark)
    {
        QTallyMarkText = mark.CTallyMarkText;
        QTallyMarkCount = mark.CTallyMarkCount.ToString(CultureInfo.InvariantCulture);
        QTallyMarkCharacters = mark.CTallyMarkCharacters;
    }

    public string QTallyMarkText { get; }

    public string QTallyMarkCount { get; }

    public IReadOnlyList<string> QTallyMarkCharacters { get; }

    internal static void QTallyMarkRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QTallyMark mark
            || QLook.QLookPartFind<ToggleButton>(container, "PTallyDropper") is not ToggleButton dropper)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(dropper, "PTallyPart") is TextBlock part)
        {
            part.Text = mark.QTallyMarkText;
        }

        if (QLook.QLookPartFind<TextBlock>(dropper, "PTallyCount") is TextBlock count)
        {
            count.Text = mark.QTallyMarkCount;
        }

        dropper.Checked -= QTallyDropRefine;
        dropper.Checked += QTallyDropRefine;
        dropper.Unchecked -= QTallyDropRefine;
        dropper.Unchecked += QTallyDropRefine;
        if (QLook.QLookPartFind<Popup>(container, "PTallyPopup") is Popup popup)
        {
            popup.PlacementTarget = dropper;
            popup.Closed -= QTallyCloseRefine;
            popup.Closed += QTallyCloseRefine;
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PTallyCharacters") is ItemsControl characters)
        {
            characters.ItemsSource = mark.QTallyMarkCharacters;
        }
    }

    private static void QTallyDropRefine(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton { Parent: Panel host } dropper)
        {
            return;
        }

        foreach (UIElement child in host.Children)
        {
            if (child is Popup popup)
            {
                popup.IsOpen = QLook.QLookCheckedRead(dropper.IsChecked);
            }
        }
    }

    private static void QTallyCloseRefine(object? sender, EventArgs e)
    {
        if (sender is Popup { PlacementTarget: ToggleButton dropper })
        {
            dropper.IsChecked = false;
        }
    }

    internal static IReadOnlyList<QTallyMark> QTallyMarkBuild(IReadOnlyList<CTallyMark> marks)
    {
        ArgumentNullException.ThrowIfNull(marks);

        return QSplice.QSpliceBuild(marks, static mark => new QTallyMark(mark));
    }
}
