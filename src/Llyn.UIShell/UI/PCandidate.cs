using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const int PCandidateLimit = 8;
    private const double PCandidateShade = 10;
    private const double PCandidateGap = 6;

    private readonly ObservableCollection<PCandidateItem> _pCandidateItem = [];

    private PCard? _pCandidateCard;

    internal void PCandidateHandle(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PCandidateItem item })
        {
            PCandidateHide();
            return;
        }

        PCandidateSelect(item);
        e.Handled = true;
    }

    private bool PCandidateHandle(Key key)
    {
        if (key == Key.Escape)
        {
            PCandidateHide();
            return true;
        }

        if (key == Key.Down || key == Key.Up)
        {
            int count = _pCandidateItem.Count;
            if (count == 0)
            {
                return false;
            }

            int step = key == Key.Down ? 1 : count - 1;
            int chosen = PCandidateList.SelectedIndex < 0
                ? (key == Key.Down ? count - 1 : 0)
                : PCandidateList.SelectedIndex;
            PCandidateList.SelectedIndex = (chosen + step) % count;
            PCandidateList.ScrollIntoView(PCandidateList.SelectedItem);
            return true;
        }

        if (key == Key.Enter && PCandidateList.SelectedItem is PCandidateItem item)
        {
            PCandidateSelect(item);
            return true;
        }

        return false;
    }

    private void PCandidateSelect(PCandidateItem item)
    {
        PCard? card = _pCandidateCard;
        PCandidateHide();

        if (card is null)
        {
            return;
        }

        card.PCardContextCommit(item.PCandidateItemId, item.PCandidateItemTitle);
        card.PCardContextClear();
    }

    private void PCandidateShow(PCard card, string text)
    {
        string word = (text ?? string.Empty).Trim();
        if (word.Length == 0)
        {
            PCandidateHide();
            return;
        }

        IReadOnlyList<LCatalogSituation> found;
        try
        {
            found = _lEngine.LEngineSituationFind(word, LCatalogOrder.LCatalogOrderUsage);
        }
        catch (Exception)
        {
            PCandidateHide();
            return;
        }

        _pCandidateItem.Clear();
        foreach (LCatalogSituation row in found)
        {
            LSituation stored = row.LCatalogSituationStored;
            string title = stored.LSituationTitle.LStateValueShow().Trim();
            if (title.Length == 0 || card.PCardContextMatch(stored.LSituationId))
            {
                continue;
            }

            _pCandidateItem.Add(new PCandidateItem(
                stored.LSituationId, title, row.LCatalogSituationUsage, word));

            if (_pCandidateItem.Count == PCandidateLimit)
            {
                break;
            }
        }

        if (_pCandidateItem.Count == 0)
        {
            PCandidateHide();
            return;
        }

        TextBox? box = PCandidateBoxFind(card);

        _pCandidateCard = card;
        PCandidate.PlacementTarget = PCandidateFrameFind(box) ?? box ?? (UIElement)PContents;
        PCandidate.IsOpen = true;
        PCandidateList.SelectedIndex = -1;
    }

    private void PCandidateHide()
    {
        PCandidate.IsOpen = false;
        PCandidateList.SelectedIndex = -1;
        _pCandidateItem.Clear();
        _pCandidateCard = null;
    }

    private TextBox? PCandidateBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PContextCaret row } box &&
            PCardContextFind(row) == card
            ? box
            : null;
    }

    private static CustomPopupPlacement[] PCandidatePlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-PCandidateShade, target.Height + PCandidateGap - PCandidateShade);
        var pAbove = new Point(-PCandidateShade, PCandidateShade - PCandidateGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    private static FrameworkElement? PCandidateFrameFind(TextBox? box)
    {
        if (box is null)
        {
            return null;
        }

        box.ApplyTemplate();
        return box.Template?.FindName(PField.PFieldSurfaceName, box) as FrameworkElement;
    }
}
