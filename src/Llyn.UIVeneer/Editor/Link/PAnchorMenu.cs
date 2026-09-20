using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private const string PAnchorMenuStyle = "Theme.Choice.Filter";

    private const string PAnchorMenuBrush = "Theme.Accent";

    private const string PAnchorMenuMark = " ≈";

    private PReflexItem? _pAnchorRow;

    internal void PReflexAnchorHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not PReflexItem row || e.OriginalSource is not UIElement anchor)
        {
            return;
        }

        PAnchor.IsOpen = false;
        _pAnchorRow = row;
        PAnchorBuild(row);
        PAnchor.PlacementTarget = anchor;
        PAnchor.IsOpen = true;
    }

    private void PAnchorBuild(PReflexItem row)
    {
        PAnchorList.Children.Clear();
        IReadOnlyList<string> classes = LAnatomyTone.LAnatomyToneScan(
            _pEditorHost.PWindowDeportment.LWindowToneRead(_lEditor.LEditorLanguage),
            row.PReflexItemLanguage,
            row.PReflexItemTone);
        IReadOnlyList<PAnchorItem> items = PAnchorItem.PAnchorItemScan(
            _lEditor.LEditorAnchorRead(), row.PReflexItemAnchors, classes);
        PAnchorEmpty.Visibility = items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        foreach (PAnchorItem item in items)
        {
            CheckBox box = new()
            {
                Style = (Style)PAnchorList.FindResource(PAnchorMenuStyle),
                Tag = item.PAnchorItemId,
                IsChecked = item.PAnchorItemAnchored,
                Content = PAnchorLabelBuild(item),
            };
            box.Click += PAnchorTickHandle;
            PAnchorList.Children.Add(box);
        }
    }

    private TextBlock PAnchorLabelBuild(PAnchorItem item)
    {
        TextBlock label = new() { VerticalAlignment = VerticalAlignment.Center };
        label.Inlines.Add(new Run(item.PAnchorItemLabel));
        if (item.PAnchorItemEstimated)
        {
            label.Inlines.Add(new Run(PAnchorMenuMark)
            {
                Foreground = (Brush)PAnchorList.FindResource(PAnchorMenuBrush),
                FontWeight = FontWeights.SemiBold,
            });
        }

        return label;
    }

    private void PAnchorTickHandle(object sender, RoutedEventArgs e)
    {
        if (_pAnchorRow is not PReflexItem row || sender is not CheckBox { Tag: long fanqieId } box)
        {
            return;
        }

        bool anchored = box.IsChecked == true;
        row.PReflexItemAnchors = _pEditorHost.PWindowDeportment.LWindowAnchorToggle(
            row.PReflexItemAnchors, fanqieId, anchored);
        PReflexAnchorShow();
        PEditorRequestSend(new LRequestReflexAnchor(PEditorDraft, row.PReflexItemId, fanqieId, anchored));
    }

    private void PAnchorClosedHandle(object? sender, EventArgs e)
    {
        _pAnchorRow = null;
    }
}
