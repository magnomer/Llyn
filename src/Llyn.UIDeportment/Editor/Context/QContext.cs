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

    private readonly ObservableCollection<QPlacard> _qContextMeaning;

    private readonly ObservableCollection<QPlacard> _qContextCollocation;

    private CCard _cCard = null!;

    private CCaret _cCaret = null!;

    private QProffer _qContextProffer = null!;

    internal QContext(
        FrameworkElement surface, ObservableCollection<QPlacard> meaning, ObservableCollection<QPlacard> collocation)
    {
        _qContextSurface = surface;
        _qContextMeaning = meaning;
        _qContextCollocation = collocation;
        surface.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QContextTextObserve));
    }

    internal void QContextIntroduce(CCard card, CCaret caret, QProffer proffer)
    {
        _cCard = card;
        _cCaret = caret;
        _qContextProffer = proffer;
    }

    internal void QContextShow(QPlacard card, CCardDraft draft, long? anchor)
    {
        List<QContextChip> chips = [];
        foreach (CSituationDraft situation in draft.CCardDraftSituation)
        {
            chips.Add(new QContextChip(situation.CSituationDraftWording, situation.CSituationDraftId));
        }

        card.QPlacardContext.QInletRowShow(chips);
        QContextAnchorShow(card, anchor);
    }

    private static void QContextAnchorShow(QPlacard card, long? anchor)
    {
        QContextChip? paired = null;
        foreach (QContextChip chip in card.QPlacardContext.QInletRow)
        {
            if (chip.QContextChipId == anchor)
            {
                paired = chip;
                break;
            }
        }

        card.QPlacardContext.QInletAnchorShow(paired);
    }

    internal void QContextApply(FrameworkElement container, object item, string? _)
    {
        if (item is QInlet<QContextChip> caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PContextEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.QInletHint);
                entry.Text = caret.QInletText;
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

        if (item is not QContextChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PContextName") is TextBlock name)
        {
            QStateConverter.QStateTextRefine(name, TextBlock.TextProperty, chip.QContextChipText);
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

    internal void QContextFieldApply(ItemsControl list, QPlacard card)
    {
        list.ItemTemplateSelector ??= new QContextSelector
        {
            QContextSelectorChip = QContract.QContractSheetFind<DataTemplate>(
                _qContextSurface, "Theme.Context.Chip"),
            QContextSelectorCaret = QContract.QContractSheetFind<DataTemplate>(
                _qContextSurface, "Theme.Context.Entry"),
        };
        QLookItem.QLookItemAttach(list, QContextApply);
        QLookItem.QLookItemAttach(
            QBerth.QBerthBuild(
                list, card.QPlacardContext, nameof(QInlet<QContextChip>.QInletAnchor)),
            QContextApply);
        if (QLook.QLookPartFind<Border>(list, "PContextFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QContextFocusRefine;
            frame.MouseLeftButtonDown += QContextFocusRefine;
        }
    }

    private void QContextTextObserve(object sender, TextChangedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: QInlet<QContextChip> caret } box
            && QContextCardFind(caret) is QPlacard card)
        {
            _qContextProffer.QProfferSituationRefine(
                card, _cCaret.CCaretSituationAdd(card.QPlacardCollocated, card.QPlacardId, box.Text));
        }
    }

    internal void QContextChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QContextChip chip } && QContextCardFind(chip) is QPlacard card)
        {
            _cCard.CCardSituationRemove(card.QPlacardCollocated, card.QPlacardId, chip.QContextChipId);
        }
    }

    internal void QContextCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: QInlet<QContextChip> row } box
            || QContextCardFind(row) is not QPlacard card)
        {
            return;
        }

        _cCaret.CCaretSituationSettle(card.QPlacardCollocated, card.QPlacardId, box.Text);
        e.Handled = true;
        _qContextProffer.QProfferShutRefine();
        card.QPlacardContext.QInletClear();
    }

    internal void QContextEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: QInlet<QContextChip> row } box
            || QContextCardFind(row) is not QPlacard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretEdgeApply(
            e.Key.ToString(),
            box.CaretIndex,
            box.Text.Length,
            box.SelectionLength,
            step => _cCaret.CCaretSituation.CCaretTrailDelete(card.QPlacardCollocated, card.QPlacardId, step));
    }

    internal void QContextCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: QInlet<QContextChip> row } box
            || QContextCardFind(row) is not QPlacard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            step =>
            {
                (bool moved, long? anchor) =
                    _cCaret.CCaretSituation.CCaretTrailMove(card.QPlacardCollocated, card.QPlacardId, step);
                QContextAnchorShow(card, anchor);
                return moved;
            },
            () => QField.QFieldCaretApply(box, row, 0));
    }

    internal void QContextBlurRefine(object sender, RoutedEventArgs e)
    {
        _qContextProffer.QProfferShutRefine();
    }

    internal void QContextCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox { DataContext: QInlet<QContextChip> row } box
            && QContextCardFind(row) is QPlacard card)
        {
            _cCaret.CCaretSituationSettle(card.QPlacardCollocated, card.QPlacardId, box.Text);
            card.QPlacardContext.QInletClear();
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

    internal QPlacard? QContextCardFind(object row)
    {
        foreach (QPlacard card in _qContextMeaning)
        {
            if ((row is QContextChip chip && card.QPlacardContext.QInletRow.Contains(chip))
                || ReferenceEquals(card.QPlacardContext, row))
            {
                return card;
            }
        }

        foreach (QPlacard card in _qContextCollocation)
        {
            if ((row is QContextChip chip && card.QPlacardContext.QInletRow.Contains(chip))
                || ReferenceEquals(card.QPlacardContext, row))
            {
                return card;
            }
        }

        return null;
    }
}
