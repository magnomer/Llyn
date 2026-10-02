using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLink
{
    private readonly FrameworkElement _qLinkSurface;

    private readonly ObservableCollection<PCard> _qLinkMeaning;

    private readonly ObservableCollection<PCard> _qLinkCollocation;

    private readonly QProspect _qLinkProspect;

    private CEditor _cEditor = null!;

    internal QLink(
        FrameworkElement surface,
        ObservableCollection<PCard> meaning,
        ObservableCollection<PCard> collocation,
        QProspect prospect)
    {
        _qLinkSurface = surface;
        _qLinkMeaning = meaning;
        _qLinkCollocation = collocation;
        _qLinkProspect = prospect;
    }

    internal void QLinkIntroduce(CEditor editor)
    {
        _cEditor = editor;
    }

    private void QLinkApply(FrameworkElement container, object item, string? _)
    {
        if (item is PLinkCaret caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PLinkEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PLinkCaretHint);
                entry.Text = caret.PLinkCaretText;
                entry.PreviewKeyDown -= _qLinkProspect.QProspectKeyRefine;
                entry.PreviewKeyDown -= _qLinkProspect.QProspectKeyObserve;
                entry.PreviewKeyDown -= QLinkCommitObserve;
                entry.PreviewKeyDown -= QLinkEraseObserve;
                entry.PreviewKeyDown -= QLinkCaretRefine;
                entry.PreviewKeyDown += _qLinkProspect.QProspectKeyRefine;
                entry.PreviewKeyDown += _qLinkProspect.QProspectKeyObserve;
                entry.PreviewKeyDown += QLinkCommitObserve;
                entry.PreviewKeyDown += QLinkEraseObserve;
                entry.PreviewKeyDown += QLinkCaretRefine;
                entry.LostKeyboardFocus -= QLinkBlurRefine;
                entry.LostKeyboardFocus -= QLinkCloseObserve;
                entry.LostKeyboardFocus += QLinkBlurRefine;
                entry.LostKeyboardFocus += QLinkCloseObserve;
            }

            return;
        }

        if (item is not QLinkChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<Image>(container, "PLinkFlag") is Image flag)
        {
            flag.Source = chip.QLinkChipFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLinkHeadword") is TextBlock headword)
        {
            headword.Text = chip.QLinkChipTarget.CTranslationTargetHeadword;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLinkLanguage") is TextBlock language)
        {
            language.Text = chip.QLinkChipTarget.CTranslationTargetLanguage;
        }

        if (QLook.QLookPartFind<Button>(container, "PLinkEraser") is Button eraser)
        {
            eraser.Click -= QLinkDropObserve;
            eraser.Click += QLinkDropObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PLinkIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("unlink", 12);
            }
        }
    }

    internal void QLinkFieldApply(ItemsControl list, PCard card)
    {
        list.ItemTemplateSelector ??= new PLinkSelector
        {
            PLinkSelectorChip = QContract.QContractSheetFind<DataTemplate>(
                _qLinkSurface, "Theme.Link.Chip"),
            PLinkSelectorCaret = QContract.QContractSheetFind<DataTemplate>(
                _qLinkSurface, "Theme.Link.Entry"),
        };
        QLookItem.QLookItemAttach(list, QLinkApply);
        QLookItem.QLookItemAttach(
            QBerth.QBerthBuild(list, card.PCardLinkCaret, nameof(PLinkCaret.PLinkCaretAnchor)),
            QLinkApply);
        if (QLook.QLookPartFind<Border>(list, "PLinkFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QLinkFocusRefine;
            frame.MouseLeftButtonDown += QLinkFocusRefine;
        }
    }

    internal void QLinkTextObserve(PLinkCaret caret, string text)
    {
        if (QLinkCardFind(caret) is PCard card)
        {
            QLinkRefine(card, _cEditor.CEditorCard.CCardTranslationAdd(
                card.PCardId, text, card.PCardLinkPosition));
        }
    }

    private void QLinkRefine(PCard card, CProspect prospect)
    {
        card.PCardLinkRefine(prospect.CProspectText);
        if (prospect.CProspectShown)
        {
            _qLinkProspect.QProspectTranslationRefine(card, prospect);
            return;
        }

        _qLinkProspect.QProspectShutRefine();
    }

    private void QLinkDropObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QLinkChip chip } && QLinkCardFind(chip) is PCard card)
        {
            _cEditor.CEditorCard.CCardTranslationRemove(
                card.PCardId, chip.QLinkChipTarget.CTranslationTargetId);
        }
    }

    private void QLinkCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PLinkCaret row }
            || QLinkCardFind(row) is not PCard card)
        {
            return;
        }

        CProspect prospect = _cEditor.CEditorCard.CCardTranslationResolve(
            card.PCardId, row.PLinkCaretText, card.PCardLinkPosition, true);
        e.Handled = true;
        QLinkRefine(card, prospect);
    }

    private void QLinkEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLinkCaret row } box || QLinkCardFind(row) is not PCard card)
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
                if (card.PCardLinkFind(step) is QLinkChip chip)
                {
                    _cEditor.CEditorCard.CCardTranslationRemove(
                        card.PCardId, chip.QLinkChipTarget.CTranslationTargetId);
                }
            });
    }

    private void QLinkCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLinkCaret row } box || QLinkCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardLinkMove,
            () => QField.QFieldCaretApply(box, row, 0));
    }

    private void QLinkBlurRefine(object sender, RoutedEventArgs e)
    {
        _qLinkProspect.QProspectShutRefine();
    }

    private void QLinkCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLinkCaret row } && QLinkCardFind(row) is PCard card)
        {
            QLinkRefine(card, _cEditor.CEditorCard.CCardTranslationResolve(
                card.PCardId, row.PLinkCaretText, card.PCardLinkPosition, false));
        }
    }

    private void QLinkFocusRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DependencyObject surface)
        {
            return;
        }

        TextBox? entry = QField.QFieldCaretFind(surface);
        if (entry is null)
        {
            return;
        }

        entry.Focus();
        entry.CaretIndex = entry.Text.Length;
        e.Handled = true;
    }

    internal void QLinkFlagRefine()
    {
        foreach (PCard card in _qLinkMeaning)
        {
            card.PCardFlagUpdate();
        }

        foreach (PCard card in _qLinkCollocation)
        {
            card.PCardFlagUpdate();
        }
    }

    internal TextBox? QLinkBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PLinkCaret row } box &&
            QLinkCardFind(row) == card
            ? box
            : null;
    }

    internal PCard? QLinkCardFind(object row)
    {
        foreach (PCard card in _qLinkMeaning)
        {
            if ((row is QLinkChip chip && card.PCardLink.Contains(chip))
                || ReferenceEquals(card.PCardLinkCaret, row))
            {
                return card;
            }
        }

        foreach (PCard card in _qLinkCollocation)
        {
            if ((row is QLinkChip chip && card.PCardLink.Contains(chip))
                || ReferenceEquals(card.PCardLinkCaret, row))
            {
                return card;
            }
        }

        return null;
    }
}
