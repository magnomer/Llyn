using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QContext
{
    private readonly FrameworkElement _qContextSurface;

    private readonly ObservableCollection<PCard> _qContextMeaning;

    private readonly ObservableCollection<PCard> _qContextCollocation;

    private CCard _cCard = null!;

    private QProffer _qContextProffer = null!;

    internal QContext(
        FrameworkElement surface, ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
    {
        _qContextSurface = surface;
        _qContextMeaning = meaning;
        _qContextCollocation = collocation;
        surface.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QContextTextObserve));
    }

    internal void QContextIntroduce(CCard card, QProffer proffer)
    {
        _cCard = card;
        _qContextProffer = proffer;
    }

    internal static void QContextShow(PCard card, IReadOnlyList<CSituationDraft> drafts)
    {
        List<PContext> chips = [];
        foreach (CSituationDraft draft in drafts)
        {
            chips.Add(new PContext(draft.CSituationDraftWording, draft.CSituationDraftId));
        }

        card.PCardContext.PCaretShow(chips);
    }

    internal void QContextApply(FrameworkElement container, object item, string? _)
    {
        if (item is PCaret<PContext> caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PContextEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PCaretHint);
                entry.Text = caret.PCaretText;
                entry.PreviewKeyDown -= _qContextProffer.QProfferKeyRefine;
                entry.PreviewKeyDown -= _qContextProffer.QProfferKeyObserve;
                entry.PreviewKeyDown -= QContextCommitObserve;
                entry.PreviewKeyDown -= QContextEraseObserve;
                entry.PreviewKeyDown -= QContextCaretRefine;
                entry.PreviewKeyDown += _qContextProffer.QProfferKeyRefine;
                entry.PreviewKeyDown += _qContextProffer.QProfferKeyObserve;
                entry.PreviewKeyDown += QContextCommitObserve;
                entry.PreviewKeyDown += QContextEraseObserve;
                entry.PreviewKeyDown += QContextCaretRefine;
                entry.LostKeyboardFocus -= QContextBlurRefine;
                entry.LostKeyboardFocus -= QContextCloseObserve;
                entry.LostKeyboardFocus += QContextBlurRefine;
                entry.LostKeyboardFocus += QContextCloseObserve;
            }

            return;
        }

        if (item is not PContext chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PContextName") is TextBlock name)
        {
            QStateConverter.QStateTextRefine(name, TextBlock.TextProperty, chip.PContextText);
        }

        if (QLook.QLookPartFind<Button>(container, "PContextEraser") is Button eraser)
        {
            eraser.Click -= QContextChipObserve;
            eraser.Click += QContextChipObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PContextIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    internal void QContextFieldApply(ItemsControl list, PCard card)
    {
        list.ItemTemplateSelector ??= new PContextSelector
        {
            PContextSelectorChip = QContract.QContractSheetFind<DataTemplate>(
                _qContextSurface, "Theme.Context.Chip"),
            PContextSelectorCaret = QContract.QContractSheetFind<DataTemplate>(
                _qContextSurface, "Theme.Context.Entry"),
        };
        QLookItem.QLookItemAttach(list, QContextApply);
        QLookItem.QLookItemAttach(
            QBerth.QBerthBuild(
                list, card.PCardContext, nameof(PCaret<PContext>.PCaretAnchor)),
            QContextApply);
        if (QLook.QLookPartFind<Border>(list, "PContextFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QContextFocusRefine;
            frame.MouseLeftButtonDown += QContextFocusRefine;
        }
    }

    private void QContextTextObserve(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: PCaret<PContext> caret } box
            && QContextCardFind(caret) is PCard card)
        {
            _qContextProffer.QProfferSituationRefine(card, _cCard.CCardSituationAdd(
                card.PCardId, box.Text, card.PCardContext.PCaretPosition, false));
        }
    }

    internal void QContextChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext chip } && QContextCardFind(chip) is PCard card)
        {
            _cCard.CCardSituationRemove(card.PCardId, chip.PContextId);
        }
    }

    internal void QContextCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: PCaret<PContext> row }
            || QContextCardFind(row) is not PCard card)
        {
            return;
        }

        _cCard.CCardSituationAdd(
            card.PCardId, card.PCardContext.PCaretText, card.PCardContext.PCaretPosition, true);
        e.Handled = true;
        _qContextProffer.QProfferShutRefine();
        card.PCardContext.PCaretClear();
    }

    internal void QContextEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PCaret<PContext> row } box
            || QContextCardFind(row) is not PCard card)
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
                if (card.PCardContext.PCaretFind(step) is PContext chip)
                {
                    _cCard.CCardSituationRemove(card.PCardId, chip.PContextId);
                }
            });
    }

    internal void QContextCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PCaret<PContext> row } box
            || QContextCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardContext.PCaretMove,
            () => QField.QFieldCaretApply(box, row, 0));
    }

    internal void QContextBlurRefine(object sender, RoutedEventArgs e)
    {
        _qContextProffer.QProfferShutRefine();
    }

    internal void QContextCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PCaret<PContext> row }
            && QContextCardFind(row) is PCard card)
        {
            _cCard.CCardSituationAdd(
                card.PCardId, card.PCardContext.PCaretText, card.PCardContext.PCaretPosition, true);
            card.PCardContext.PCaretClear();
        }
    }

    internal void QContextFocusRefine(object sender, MouseButtonEventArgs e)
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

    internal PCard? QContextCardFind(object row)
    {
        foreach (PCard card in _qContextMeaning)
        {
            if ((row is PContext chip && card.PCardContext.PCaretRow.Contains(chip))
                || ReferenceEquals(card.PCardContext, row))
            {
                return card;
            }
        }

        foreach (PCard card in _qContextCollocation)
        {
            if ((row is PContext chip && card.PCardContext.PCaretRow.Contains(chip))
                || ReferenceEquals(card.PCardContext, row))
            {
                return card;
            }
        }

        return null;
    }
}
