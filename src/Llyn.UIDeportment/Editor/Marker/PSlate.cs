using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PSlateTemplate _pSlateTemplate;

    private Popup PSlate => (Popup)FindName(nameof(PSlate));

    private Border PSlateSheet => (Border)FindName(nameof(PSlateSheet));

    private ListBox PSlateList => (ListBox)FindName(nameof(PSlateList));

    private void PSlateAttach()
    {
        PSlateList.ItemsSource = _pSlateItem;
        PLookItem.PLookItemAttach(PSlateList, PSlateApply);
        PSlate.CustomPopupPlacementCallback = PSlatePlace;
    }

    private void PSlateApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PSlateItem row)
        {
            return;
        }

        if (PLook.PLookPartFind<Run>(container, "PSlateLead") is Run lead)
        {
            lead.Text = row.PSlateItemLead;
        }

        if (PLook.PLookPartFind<Run>(container, "PSlateMark") is Run mark)
        {
            mark.Text = row.PSlateItemMark;
        }

        if (PLook.PLookPartFind<Run>(container, "PSlateTail") is Run tail)
        {
            tail.Text = row.PSlateItemTail;
        }

        if (PLook.PLookPartFind<Grid>(container, "PSlateRow") is Grid surface)
        {
            surface.PreviewMouseLeftButtonDown -= _pSlateTemplate.PSlateHandle;
            surface.PreviewMouseLeftButtonDown += _pSlateTemplate.PSlateHandle;
        }
    }

    private const int PSlateLimit = 8;
    private const double PSlateShade = 10;
    private const double PSlateGap = 6;

    private readonly ObservableCollection<PSlateItem> _pSlateItem = [];

    private PCard? _pSlateCard;

    internal void PSlateHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSlateItem item })
        {
            PSlateHide();
            return;
        }

        PSlateSelect(item);
        e.Handled = true;
    }

    private bool PSlateHandle(Key key)
    {
        if (key == Key.Escape)
        {
            PSlateHide();
            return true;
        }

        if (key == Key.Down || key == Key.Up)
        {
            int count = _pSlateItem.Count;
            if (count == 0)
            {
                return false;
            }

            int step = key == Key.Down ? 1 : count - 1;
            int chosen = PSlateList.SelectedIndex < 0
                ? (key == Key.Down ? count - 1 : 0)
                : PSlateList.SelectedIndex;
            PSlateList.SelectedIndex = (chosen + step) % count;
            PSlateList.ScrollIntoView(PSlateList.SelectedItem);
            return true;
        }

        if (key == Key.Enter && PSlateList.SelectedItem is PSlateItem item)
        {
            PSlateSelect(item);
            return true;
        }

        return false;
    }

    private void PSlateSelect(PSlateItem item)
    {
        PCard? card = _pSlateCard;
        PSlateHide();

        if (card is null)
        {
            return;
        }

        if (!card.PCardLabelMatch(item.PSlateItemId) && !card.PCardLabelCheck(item.PSlateItemText))
        {
            PLabelSend(card, item.PSlateItemId);
        }

        card.PCardLabelClear();
    }

    private void PSlateShow(PCard card, string text)
    {
        string word = (text ?? string.Empty).Trim();
        if (word.Length == 0)
        {
            PSlateHide();
            return;
        }

        IReadOnlyList<LTag> found;
        try
        {
            found = _lEditor.LEditorCard.LCardTagFind(word);
        }
        catch (Exception)
        {
            PSlateHide();
            return;
        }

        _pSlateItem.Clear();
        foreach (LTag tag in found)
        {
            string written = tag.LTagText.Trim();
            if (written.Length == 0 || card.PCardLabelCheck(written))
            {
                continue;
            }

            _pSlateItem.Add(new PSlateItem(tag.LTagId, written, word));

            if (_pSlateItem.Count == PSlateLimit)
            {
                break;
            }
        }

        if (_pSlateItem.Count == 0)
        {
            PSlateHide();
            return;
        }

        TextBox? box = PSlateBoxFind(card);

        _pSlateCard = card;
        PSlate.PlacementTarget = PSlateFrameFind(box) ?? box ?? (UIElement)PContents;
        PSlateSheet.SetBinding(
            FrameworkElement.MinWidthProperty,
            new Binding(nameof(FrameworkElement.ActualWidth)) { Source = PSlate.PlacementTarget });
        PSlate.IsOpen = true;
        PSlateList.SelectedIndex = -1;
    }

    private void PSlateHide()
    {
        PSlate.IsOpen = false;
        PSlateList.SelectedIndex = -1;
        _pSlateItem.Clear();
        _pSlateCard = null;
    }

    private TextBox? PSlateBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PLabelCaret row } box &&
            PCardLabelFind(row) == card
            ? box
            : null;
    }

    private static CustomPopupPlacement[] PSlatePlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-PSlateShade, target.Height + PSlateGap - PSlateShade);
        var pAbove = new Point(-PSlateShade, PSlateShade - PSlateGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    private static FrameworkElement? PSlateFrameFind(TextBox? box)
    {
        if (box is null)
        {
            return null;
        }

        box.ApplyTemplate();
        return box.Template?.FindName(PField.PFieldSurfaceName, box) as FrameworkElement;
    }
}
