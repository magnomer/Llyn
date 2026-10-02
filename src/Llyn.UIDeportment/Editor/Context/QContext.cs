using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QContext
{
    private readonly FrameworkElement _qContextSurface;

    private readonly ObservableCollection<PCard> _qContextMeaning;

    private readonly ObservableCollection<PCard> _qContextCollocation;

    private CEditor _cEditor = null!;

    private QProffer _qContextProffer = null!;

    internal QContext(
        FrameworkElement surface, ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
    {
        _qContextSurface = surface;
        _qContextMeaning = meaning;
        _qContextCollocation = collocation;
    }

    internal void QContextIntroduce(CEditor editor, QProffer proffer)
    {
        _cEditor = editor;
        _qContextProffer = proffer;
    }

    internal void QContextApply(FrameworkElement container, object item, string? _)
    {
        if (item is PContextCaret caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PContextEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PContextCaretHint);
                entry.Text = caret.PContextCaretText;
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
            QBerth.QBerthBuild(list, card.PCardContextCaret, nameof(PContextCaret.PContextCaretAnchor)),
            QContextApply);
        if (QLook.QLookPartFind<Border>(list, "PContextFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QContextFocusRefine;
            frame.MouseLeftButtonDown += QContextFocusRefine;
        }
    }

    internal void QContextTextObserve(PContextCaret caret, string text)
    {
        if (QContextCardFind(caret) is PCard card)
        {
            _qContextProffer.QProfferSituationRefine(card, _cEditor.CEditorCard.CCardSituationAdd(
                card.PCardId, text, card.PCardContextPosition, false));
        }
    }

    internal void QContextChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContext chip } && QContextCardFind(chip) is PCard card)
        {
            _cEditor.CEditorCard.CCardSituationRemove(card.PCardId, chip.PContextId);
        }
    }

    internal void QContextCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: PContextCaret row }
            || QContextCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorCard.CCardSituationAdd(
            card.PCardId, card.PCardContextText, card.PCardContextPosition, true);
        e.Handled = true;
        _qContextProffer.QProfferShutRefine();
        card.PCardContextClear();
    }

    internal void QContextEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PContextCaret row } box || QContextCardFind(row) is not PCard card)
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
                if (card.PCardContextFind(step) is PContext chip)
                {
                    _cEditor.CEditorCard.CCardSituationRemove(card.PCardId, chip.PContextId);
                }
            });
    }

    internal void QContextCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PContextCaret row } box || QContextCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardContextMove,
            () => QField.QFieldCaretApply(box, row, 0));
    }

    internal void QContextBlurRefine(object sender, RoutedEventArgs e)
    {
        _qContextProffer.QProfferShutRefine();
    }

    internal void QContextCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PContextCaret row } && QContextCardFind(row) is PCard card)
        {
            _cEditor.CEditorCard.CCardSituationAdd(
                card.PCardId, card.PCardContextText, card.PCardContextPosition, true);
            card.PCardContextClear();
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
            if ((row is PContext chip && card.PCardContext.Contains(chip))
                || ReferenceEquals(card.PCardContextCaret, row))
            {
                return card;
            }
        }

        foreach (PCard card in _qContextCollocation)
        {
            if ((row is PContext chip && card.PCardContext.Contains(chip))
                || ReferenceEquals(card.PCardContextCaret, row))
            {
                return card;
            }
        }

        return null;
    }
}
