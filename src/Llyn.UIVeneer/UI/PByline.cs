using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PImprint
{
    private const int PBylineLimit = 8;
    private const double PBylineShade = 10;
    private const double PBylineGap = 6;

    private readonly ObservableCollection<PBylineItem> _pBylineItem = [];

    private TextBox? _pBylineBox;

    internal void PBylineHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PBylineItem item })
        {
            PBylineHide();
            return;
        }

        PBylineSelect(item);
        e.Handled = true;
    }

    private bool PBylineHandle(Key key)
    {
        if (key == Key.Escape)
        {
            PBylineHide();
            return true;
        }

        if (key == Key.Down || key == Key.Up)
        {
            int count = _pBylineItem.Count;
            if (count == 0)
            {
                return false;
            }

            int step = key == Key.Down ? 1 : count - 1;
            int chosen = PBylineList.SelectedIndex < 0
                ? (key == Key.Down ? count - 1 : 0)
                : PBylineList.SelectedIndex;
            PBylineList.SelectedIndex = (chosen + step) % count;
            PBylineList.ScrollIntoView(PBylineList.SelectedItem);
            return true;
        }

        if (key == Key.Enter && PBylineList.SelectedItem is PBylineItem item)
        {
            PBylineSelect(item);
            return true;
        }

        return false;
    }

    private void PBylineSelect(PBylineItem item)
    {
        TextBox? box = _pBylineBox;
        PBylineHide();

        if (box?.DataContext is PAuthorItem row)
        {
            PAuthorAttach(row, box, item.PBylineItemId);
        }
    }

    private void PBylineShow(TextBox box, string text)
    {
        string word = (text ?? string.Empty).Trim();
        if (word.Length == 0)
        {
            PBylineHide();
            return;
        }

        IReadOnlyList<LAuthor> found;
        try
        {
            found = _lEngine.LEngineAuthorFind(word);
        }
        catch (Exception)
        {
            PBylineHide();
            return;
        }

        _pBylineItem.Clear();
        foreach (LAuthor author in found)
        {
            if (!author.LAuthorNamed)
            {
                continue;
            }

            string name = author.LAuthorName.Trim();
            if (PAuthorCreditFind(author.LAuthorId) is not null)
            {
                continue;
            }

            _pBylineItem.Add(new PBylineItem(author.LAuthorId, name, word));

            if (_pBylineItem.Count == PBylineLimit)
            {
                break;
            }
        }

        if (_pBylineItem.Count == 0)
        {
            PBylineHide();
            return;
        }

        _pBylineBox = box;
        PByline.PlacementTarget = PBylineFrameFind(box) ?? box;
        PByline.IsOpen = true;
        PBylineList.SelectedIndex = -1;
    }

    private void PBylineHide()
    {
        PByline.IsOpen = false;
        PBylineList.SelectedIndex = -1;
        _pBylineItem.Clear();
        _pBylineBox = null;
    }

    private static CustomPopupPlacement[] PBylinePlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-PBylineShade, target.Height + PBylineGap - PBylineShade);
        var pAbove = new Point(-PBylineShade, PBylineShade - PBylineGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    private static FrameworkElement? PBylineFrameFind(TextBox box)
    {
        box.ApplyTemplate();
        return box.Template?.FindName(PField.PFieldSurfaceName, box) as FrameworkElement;
    }
}
