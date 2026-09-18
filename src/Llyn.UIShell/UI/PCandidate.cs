using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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

    private PSentence? _pCandidateSentence;

    private bool _pCandidateRegister;

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
        PSentence? sentence = _pCandidateSentence;
        bool register = _pCandidateRegister;
        PCandidateHide();

        if (sentence is not null)
        {
            if (card is not null)
            {
                PSentenceCitationSend(card, sentence, item.PCandidateItemId);
            }

            return;
        }

        if (card is null)
        {
            return;
        }

        if (register)
        {
            if (!card.PCardRegisterMatch(item.PCandidateItemId))
            {
                PRegisterSend(card, item.PCandidateItemId, item.PCandidateItemTitle);
            }

            card.PCardRegisterClear();
            return;
        }

        if (!card.PCardContextMatch(item.PCandidateItemId))
        {
            PContextSend(card, item.PCandidateItemId, item.PCandidateItemTitle);
        }

        card.PCardContextClear();
    }

    private void PCandidateRegisterShow(PCard card, string text)
    {
        string word = (text ?? string.Empty).Trim();
        if (word.Length == 0)
        {
            PCandidateHide();
            return;
        }

        IReadOnlyList<LRegister> found;
        try
        {
            found = _lEngine.LEngineRegisterFind(word, PSpeakerLanguageRead());
        }
        catch (Exception)
        {
            PCandidateHide();
            return;
        }

        _pCandidateItem.Clear();
        foreach (LRegister row in found)
        {
            string name = row.LRegisterName.LStateValueShow().Trim();
            if (name.Length == 0 || card.PCardRegisterMatch(row.LRegisterId))
            {
                continue;
            }

            _pCandidateItem.Add(new PCandidateItem(row.LRegisterId, name, 0, word));

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

        TextBox? box = PCandidateRegisterFind(card);

        _pCandidateCard = card;
        _pCandidateRegister = true;
        PCandidate.PlacementTarget = PCandidateFrameFind(box) ?? box ?? (UIElement)PContents;
        PCandidate.IsOpen = true;
        PCandidateList.SelectedIndex = -1;
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
        _pCandidateRegister = false;
        PCandidate.PlacementTarget = PCandidateFrameFind(box) ?? box ?? (UIElement)PContents;
        PCandidate.IsOpen = true;
        PCandidateList.SelectedIndex = -1;
    }

    private void PCandidateCitationShow(PCard card, PSentence row, TextBox box)
    {
        string word = box.Text.Trim();
        string cited = PSentence.PSentenceCitationFind(_pEditorCitation, row.PSentenceCitation);
        if (word.Length == 0 || string.Equals(word, cited, StringComparison.Ordinal))
        {
            PCandidateHide();
            return;
        }

        IReadOnlyList<LCatalogReference> found;
        try
        {
            found = _lEngine.LEngineReferenceFind(word, LCatalogOrder.LCatalogOrderUsage);
        }
        catch (Exception)
        {
            PCandidateHide();
            return;
        }

        _pCandidateItem.Clear();
        foreach (LCatalogReference reference in found)
        {
            _pCandidateItem.Add(new PCandidateItem(
                reference.LCatalogReferenceStored.LReferenceId,
                reference.LCatalogReferenceByline,
                reference.LCatalogReferenceUsage,
                word));

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

        _pCandidateCard = card;
        _pCandidateSentence = row;
        _pCandidateRegister = false;
        PCandidate.PlacementTarget = PCandidateFrameFind(box) ?? (UIElement)box;
        PCandidate.IsOpen = true;
        PCandidateList.SelectedIndex = -1;
    }

    private void PCandidateHide()
    {
        PCandidate.IsOpen = false;
        PCandidateList.SelectedIndex = -1;
        _pCandidateItem.Clear();
        _pCandidateCard = null;
        _pCandidateSentence = null;
        _pCandidateRegister = false;
    }

    private TextBox? PCandidateRegisterFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PRegisterCaret row } box &&
            PCardRegisterFind(row) == card
            ? box
            : null;
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
