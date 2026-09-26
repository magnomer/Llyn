using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PTallyMark
{
    private PTallyMark(LTallyMark mark)
    {
        PTallyMarkText = mark.LTallyMarkText;
        PTallyMarkCount = mark.LTallyMarkCount.ToString(CultureInfo.InvariantCulture);
        PTallyMarkCharacters = mark.LTallyMarkCharacters;
    }

    public string PTallyMarkText { get; }

    public string PTallyMarkCount { get; }

    public IReadOnlyList<string> PTallyMarkCharacters { get; }

    internal static void PTallyMarkApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PTallyMark mark
            || PLook.PLookPartFind<ToggleButton>(container, "PTallyDropper") is not ToggleButton dropper)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(dropper, "PTallyPart") is TextBlock part)
        {
            part.Text = mark.PTallyMarkText;
        }

        if (PLook.PLookPartFind<TextBlock>(dropper, "PTallyCount") is TextBlock count)
        {
            count.Text = mark.PTallyMarkCount;
        }

        dropper.Checked -= PTallyDropHandle;
        dropper.Checked += PTallyDropHandle;
        dropper.Unchecked -= PTallyDropHandle;
        dropper.Unchecked += PTallyDropHandle;
        if (PLook.PLookPartFind<Popup>(container, "PTallyPopup") is Popup popup)
        {
            popup.PlacementTarget = dropper;
            popup.Closed -= PTallyCloseHandle;
            popup.Closed += PTallyCloseHandle;
        }

        if (PLook.PLookPartFind<ItemsControl>(container, "PTallyCharacters") is ItemsControl characters)
        {
            characters.ItemsSource = mark.PTallyMarkCharacters;
        }
    }

    private static void PTallyDropHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton { Parent: Panel host } dropper)
        {
            return;
        }

        foreach (UIElement child in host.Children)
        {
            if (child is Popup popup)
            {
                popup.IsOpen = PLook.PLookCheckedRead(dropper.IsChecked);
            }
        }
    }

    private static void PTallyCloseHandle(object? sender, EventArgs e)
    {
        if (sender is Popup { PlacementTarget: ToggleButton dropper })
        {
            dropper.IsChecked = false;
        }
    }

    internal static IReadOnlyList<PTallyMark> PTallyMarkBuild(IReadOnlyList<LTallyMark> marks)
    {
        ArgumentNullException.ThrowIfNull(marks);

        List<PTallyMark> built = new(marks.Count);
        foreach (LTallyMark mark in marks)
        {
            built.Add(new PTallyMark(mark));
        }

        return built;
    }
}
