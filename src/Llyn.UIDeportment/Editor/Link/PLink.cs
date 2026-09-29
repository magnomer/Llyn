using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly PLinkTemplate _pLinkTemplate;

    private void PLinkApply(FrameworkElement container, object item, string? _)
    {
        if (item is PLinkCaret caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PLinkEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PLinkCaretHint);
                entry.Text = caret.PLinkCaretText;
                entry.PreviewKeyDown -= PProspectKeyRefine;
                entry.PreviewKeyDown -= PProspectKeyObserve;
                entry.PreviewKeyDown -= PLinkCommitObserve;
                entry.PreviewKeyDown -= PLinkEraseObserve;
                entry.PreviewKeyDown -= PLinkCaretRefine;
                entry.PreviewKeyDown += PProspectKeyRefine;
                entry.PreviewKeyDown += PProspectKeyObserve;
                entry.PreviewKeyDown += PLinkCommitObserve;
                entry.PreviewKeyDown += PLinkEraseObserve;
                entry.PreviewKeyDown += PLinkCaretRefine;
                entry.LostKeyboardFocus -= PLinkBlurRefine;
                entry.LostKeyboardFocus -= PLinkCloseObserve;
                entry.LostKeyboardFocus += PLinkBlurRefine;
                entry.LostKeyboardFocus += PLinkCloseObserve;
            }

            return;
        }

        if (item is not LLinkChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<Image>(container, "PLinkFlag") is Image flag)
        {
            flag.Source = chip.LLinkChipFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLinkHeadword") is TextBlock headword)
        {
            headword.Text = chip.LLinkChipHeadword;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLinkLanguage") is TextBlock language)
        {
            language.Text = chip.LLinkChipLanguage;
        }

        if (QLook.QLookPartFind<Button>(container, "PLinkEraser") is Button eraser)
        {
            eraser.Click -= PLinkDropObserve;
            eraser.Click += PLinkDropObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PLinkIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("unlink", 12);
            }
        }
    }

    private void PLinkFieldApply(ItemsControl list)
    {
        list.ItemTemplateSelector ??= new PLinkSelector
        {
            PLinkSelectorChip = (DataTemplate)_pLinkTemplate["Theme.Link.Chip"],
            PLinkSelectorCaret = (DataTemplate)_pLinkTemplate["Theme.Link.Entry"],
        };
        QLookItem.QLookItemAttach(list, PLinkApply);
        if (QLook.QLookPartFind<Border>(list, "PLinkFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= PLinkFocusRefine;
            frame.MouseLeftButtonDown += PLinkFocusRefine;
        }
    }

    private void PLinkTextObserve(PLinkCaret caret, string text)
    {
        if (PCardLinkFind(caret) is PCard card)
        {
            PLinkRefine(card, _qEditor.QEditorArea.CEditorCard.CCardTranslationAdd(
                card.PCardId, text, card.PCardLinkPosition));
        }
    }

    private void PLinkRefine(PCard card, CProspect prospect)
    {
        card.PCardLinkRefine(prospect.CProspectText);
        if (prospect.CProspectShown)
        {
            PProspectTranslationRefine(card, prospect);
            return;
        }

        PProspectShutRefine();
    }

    private void PLinkDropObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: LLinkChip chip } && PCardLinkFind(chip) is PCard card)
        {
            _qEditor.QEditorArea.CEditorCard.CCardTranslationRemove(card.PCardId, chip.LLinkChipId);
        }
    }

    private void PLinkCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PLinkCaret row }
            || PCardLinkFind(row) is not PCard card)
        {
            return;
        }

        CProspect prospect = _qEditor.QEditorArea.CEditorCard.CCardTranslationResolve(
            card.PCardId, row.PLinkCaretText, card.PCardLinkPosition, true);
        e.Handled = true;
        PLinkRefine(card, prospect);
    }

    private void PLinkEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLinkCaret row } box || PCardLinkFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretEdgeApply(
            e.Key.ToString(),
            box.CaretIndex,
            box.Text.Length,
            box.SelectionLength,
            step =>
            {
                if (card.PCardLinkFind(step) is LLinkChip chip)
                {
                    _qEditor.QEditorArea.CEditorCard.CCardTranslationRemove(card.PCardId, chip.LLinkChipId);
                }
            });
    }

    private void PLinkCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLinkCaret row } box || PCardLinkFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardLinkMove,
            () => PEditorCaretApply(box, row, 0));
    }

    private void PLinkBlurRefine(object sender, RoutedEventArgs e)
    {
        PProspectShutRefine();
    }

    private void PLinkCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLinkCaret row } && PCardLinkFind(row) is PCard card)
        {
            PLinkRefine(card, _qEditor.QEditorArea.CEditorCard.CCardTranslationResolve(
                card.PCardId, row.PLinkCaretText, card.PCardLinkPosition, false));
        }
    }

    private void PLinkFocusRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject surface)
        {
            return;
        }

        TextBox? entry = PEditorCaretFind(surface);
        if (entry is null)
        {
            return;
        }

        entry.Focus();
        entry.CaretIndex = entry.Text.Length;
        e.Handled = true;
    }

    internal void PLinkFlagRefine()
    {
        foreach (PCard card in _pMeaningList)
        {
            card.PCardFlagUpdate();
        }

        foreach (PCard card in _pCollocationList)
        {
            card.PCardFlagUpdate();
        }
    }

    private TextBox? PLinkBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PLinkCaret row } box &&
            PCardLinkFind(row) == card
            ? box
            : null;
    }

    private PCard? PCardLinkFind(object row)
    {
        foreach (PCard card in _pMeaningList)
        {
            if (card.PCardLink.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _pCollocationList)
        {
            if (card.PCardLink.Contains(row))
            {
                return card;
            }
        }

        return null;
    }
}
