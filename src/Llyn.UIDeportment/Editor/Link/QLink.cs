using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLink
{
    private readonly FrameworkElement _qLinkSurface;

    private readonly ObservableCollection<PCard> _qLinkMeaning;

    private readonly ObservableCollection<PCard> _qLinkCollocation;

    private readonly QProspect _qLinkProspect;

    private CCard _cCard = null!;

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
        surface.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QLinkTextObserve));
    }

    internal void QLinkIntroduce(CCard card)
    {
        _cCard = card;
    }

    internal static void QLinkShow(PCard card, IReadOnlyList<CTranslationTarget> targets)
    {
        List<QLinkChip> chips = [];
        foreach (CTranslationTarget target in targets)
        {
            chips.Add(new QLinkChip(
                target.CTranslationTargetId, target.CTranslationTargetHeadword, target.CTranslationTargetLanguage));
        }

        card.PCardLink.PCaretShow(chips);
    }

    private void QLinkApply(FrameworkElement container, object item, string? _)
    {
        if (item is PCaret<QLinkChip> caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PLinkEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PCaretHint);
                entry.Text = caret.PCaretText;
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
            headword.Text = chip.QLinkChipHeadword;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLinkLanguage") is TextBlock language)
        {
            language.Text = chip.QLinkChipLanguage;
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
            QBerth.QBerthBuild(list, card.PCardLink, nameof(PCaret<QLinkChip>.PCaretAnchor)),
            QLinkApply);
        if (QLook.QLookPartFind<Border>(list, "PLinkFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QLinkFocusRefine;
            frame.MouseLeftButtonDown += QLinkFocusRefine;
        }
    }

    private void QLinkTextObserve(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: PCaret<QLinkChip> caret } box
            && QLinkCardFind(caret) is PCard card)
        {
            QLinkRefine(card, _cCard.CCardTranslationAdd(
                card.PCardId, box.Text, card.PCardLink.PCaretPosition));
        }
    }

    private void QLinkRefine(PCard card, CProspect prospect)
    {
        card.PCardLink.PCaretRefine(prospect.CProspectText);
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
            _cCard.CCardTranslationRemove(card.PCardId, chip.QLinkChipId);
        }
    }

    private void QLinkCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PCaret<QLinkChip> row }
            || QLinkCardFind(row) is not PCard card)
        {
            return;
        }

        CProspect prospect = _cCard.CCardTranslationResolve(
            card.PCardId, row.PCaretText, row.PCaretPosition, true);
        e.Handled = true;
        QLinkRefine(card, prospect);
    }

    private void QLinkEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PCaret<QLinkChip> row } box
            || QLinkCardFind(row) is not PCard card)
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
                if (card.PCardLink.PCaretFind(step) is QLinkChip chip)
                {
                    _cCard.CCardTranslationRemove(card.PCardId, chip.QLinkChipId);
                }
            });
    }

    private void QLinkCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PCaret<QLinkChip> row } box
            || QLinkCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardLink.PCaretMove,
            () => QField.QFieldCaretApply(box, row, 0));
    }

    private void QLinkBlurRefine(object sender, RoutedEventArgs e)
    {
        _qLinkProspect.QProspectShutRefine();
    }

    private void QLinkCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCaret<QLinkChip> row }
            && QLinkCardFind(row) is PCard card)
        {
            QLinkRefine(card, _cCard.CCardTranslationResolve(
                card.PCardId, row.PCaretText, row.PCaretPosition, false));
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
            foreach (QLinkChip chip in card.PCardLink.PCaretRow)
            {
                chip.QLinkChipRefine();
            }
        }

        foreach (PCard card in _qLinkCollocation)
        {
            foreach (QLinkChip chip in card.PCardLink.PCaretRow)
            {
                chip.QLinkChipRefine();
            }
        }
    }

    internal TextBox? QLinkBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PCaret<QLinkChip> row } box &&
            QLinkCardFind(row) == card
            ? box
            : null;
    }

    internal PCard? QLinkCardFind(object row)
    {
        foreach (PCard card in _qLinkMeaning)
        {
            if ((row is QLinkChip chip && card.PCardLink.PCaretRow.Contains(chip))
                || ReferenceEquals(card.PCardLink, row))
            {
                return card;
            }
        }

        foreach (PCard card in _qLinkCollocation)
        {
            if ((row is QLinkChip chip && card.PCardLink.PCaretRow.Contains(chip))
                || ReferenceEquals(card.PCardLink, row))
            {
                return card;
            }
        }

        return null;
    }
}
