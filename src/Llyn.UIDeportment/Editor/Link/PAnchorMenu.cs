using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Conduct;


namespace Llyn.UIDeportment;

public partial class PEditor
{
    private const string PAnchorMenuStyle = "Theme.Choice.Filter";

    private const string PAnchorMenuBrush = "Theme.Accent";

    private const string PAnchorMenuMark = " ≈";

    private Popup PAnchor => (Popup)FindName(nameof(PAnchor));

    private TextBlock PAnchorEmpty => (TextBlock)FindName(nameof(PAnchorEmpty));

    private StackPanel PAnchorList => (StackPanel)FindName(nameof(PAnchorList));

    private void PAnchorAttach()
    {
        PAnchor.Closed += PAnchorClosedObserve;
        CommandManager.AddPreviewExecutedHandler(PEditorSound, PAnchorShutRefine);
    }

    private void PAnchorShutRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == PReflexCommand.PReflexCommandAnchor)
        {
            PAnchor.IsOpen = false;
        }
    }

    internal void PReflexAnchorObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not QReflexItem row || e.OriginalSource is not UIElement anchor)
        {
            return;
        }

        PAnchorRefine(_qEditor.QEditorAnchor.CSoundingAnchorOpen(row.QReflexItemId), anchor);
    }

    private void PAnchorRefine(CAnchor menu, UIElement anchor)
    {
        PAnchorList.Children.Clear();
        PAnchorEmpty.Visibility = QLook.QLookVisibleRead(menu.CAnchorEmpty);
        foreach (CAnchorRow item in menu.CAnchorRows)
        {
            CheckBox box = new()
            {
                Style = (Style)PAnchorList.FindResource(PAnchorMenuStyle),
                Tag = item.CAnchorRowId,
                IsChecked = item.CAnchorRowHeld,
                Content = PAnchorLabelBuild(item),
            };
            box.Click += PAnchorTickObserve;
            PAnchorList.Children.Add(box);
        }

        PAnchor.PlacementTarget = anchor;
        PAnchor.IsOpen = true;
    }

    private TextBlock PAnchorLabelBuild(CAnchorRow item)
    {
        TextBlock label = new() { VerticalAlignment = VerticalAlignment.Center };
        label.Inlines.Add(new Run(item.CAnchorRowSummary));
        if (item.CAnchorRowEstimated)
        {
            label.Inlines.Add(new Run(PAnchorMenuMark)
            {
                Foreground = (Brush)PAnchorList.FindResource(PAnchorMenuBrush),
                FontWeight = FontWeights.SemiBold,
            });
        }

        return label;
    }

    private void PAnchorTickObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox { Tag: long fanqieId } box)
        {
            return;
        }

        _qEditor.QEditorAnchor.CSoundingAnchorSet(fanqieId, QLook.QLookCheckedRead(box.IsChecked));
    }

    private void PAnchorClosedObserve(object? sender, EventArgs e)
    {
        _qEditor.QEditorAnchor.CSoundingAnchorClose();
    }
}
