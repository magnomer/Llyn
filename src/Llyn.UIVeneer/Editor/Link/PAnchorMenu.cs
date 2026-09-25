using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Application;
using Llyn.Core;

using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private const string PAnchorMenuStyle = "Theme.Choice.Filter";

    private const string PAnchorMenuBrush = "Theme.Accent";

    private const string PAnchorMenuMark = " ≈";

    private LReflexItem? _pAnchorRow;

    internal void PReflexAnchorHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not LReflexItem row || e.OriginalSource is not UIElement anchor)
        {
            return;
        }

        PAnchor.IsOpen = false;
        _pAnchorRow = row;
        PAnchorBuild(row);
        PAnchor.PlacementTarget = anchor;
        PAnchor.IsOpen = true;
    }

    private void PAnchorBuild(LReflexItem row)
    {
        PAnchorList.Children.Clear();
        IReadOnlyList<PAnchorItem> items = PAnchorItem.PAnchorItemScan(
            _pEditorHost.PWindowDeportment.LWindowAnchorScan(
                _lEditor.LEditorAnchorRead(),
                row.LReflexItemAnchors,
                _lEditor.LEditorLanguage,
                row.LReflexItemLanguage,
                row.LReflexItemTone));
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
        if (_pAnchorRow is not LReflexItem row || sender is not CheckBox { Tag: long fanqieId } box)
        {
            return;
        }

        bool anchored = box.IsChecked == true;
        PEditorRequestSend(new LRequestReflexAnchor(PEditorDraft, row.LReflexItemId, fanqieId, anchored));
    }

    private void PAnchorClosedHandle(object? sender, EventArgs e)
    {
        _pAnchorRow = null;
    }
}
