using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const string PAnchorMenuStyle = "Theme.Choice.Filter";

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
        IReadOnlyList<PAnchorItem> items = PAnchorItem.PAnchorItemScan(_pReflexFanqie, row.PReflexItemAnchors);
        PAnchorEmpty.Visibility = items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        foreach (PAnchorItem item in items)
        {
            CheckBox box = new()
            {
                Style = (Style)PAnchorList.FindResource(PAnchorMenuStyle),
                Tag = item.PAnchorItemId,
                IsChecked = item.PAnchorItemAnchored,
                Content = new TextBlock { Text = item.PAnchorItemLabel, VerticalAlignment = VerticalAlignment.Center },
            };
            box.Click += PAnchorTickHandle;
            PAnchorList.Children.Add(box);
        }
    }

    private void PAnchorTickHandle(object sender, RoutedEventArgs e)
    {
        if (_pAnchorRow is not PReflexItem row || sender is not CheckBox { Tag: long fanqieId } box)
        {
            return;
        }

        bool anchored = box.IsChecked == true;
        row.PReflexItemAnchors = LAnchor.LAnchorToggle(row.PReflexItemAnchors, fanqieId, anchored);
        PReflexAnchorShow();
        PEditorRequestSend(new LRequestReflexAnchor(_pEditorDraft, row.PReflexItemId, fanqieId, anchored));
    }

    private void PAnchorClosedHandle(object? sender, EventArgs e)
    {
        _pAnchorRow = null;
    }
}
