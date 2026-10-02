using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QRegister
{
    private readonly FrameworkElement _qRegisterSurface;

    private readonly ObservableCollection<PCard> _qRegisterMeaning;

    private readonly ObservableCollection<PCard> _qRegisterCollocation;

    private CEditor _cEditor = null!;

    private QProffer _qRegisterProffer = null!;

    internal QRegister(
        FrameworkElement surface, ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
    {
        _qRegisterSurface = surface;
        _qRegisterMeaning = meaning;
        _qRegisterCollocation = collocation;
    }

    internal void QRegisterIntroduce(CEditor editor, QProffer proffer)
    {
        _cEditor = editor;
        _qRegisterProffer = proffer;
    }

    internal void QRegisterApply(FrameworkElement container, object item, string? _)
    {
        if (item is PRegisterCaret caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PRegisterEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PRegisterCaretHint);
                entry.Text = caret.PRegisterCaretText;
                entry.PreviewKeyDown -= _qRegisterProffer.QProfferKeyRefine;
                entry.PreviewKeyDown -= _qRegisterProffer.QProfferKeyObserve;
                entry.PreviewKeyDown -= QRegisterCommitObserve;
                entry.PreviewKeyDown -= QRegisterEraseObserve;
                entry.PreviewKeyDown -= QRegisterCaretRefine;
                entry.PreviewKeyDown += _qRegisterProffer.QProfferKeyRefine;
                entry.PreviewKeyDown += _qRegisterProffer.QProfferKeyObserve;
                entry.PreviewKeyDown += QRegisterCommitObserve;
                entry.PreviewKeyDown += QRegisterEraseObserve;
                entry.PreviewKeyDown += QRegisterCaretRefine;
                entry.LostKeyboardFocus -= QRegisterBlurRefine;
                entry.LostKeyboardFocus -= QRegisterCloseObserve;
                entry.LostKeyboardFocus += QRegisterBlurRefine;
                entry.LostKeyboardFocus += QRegisterCloseObserve;
            }

            return;
        }

        if (item is not PRegister chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PRegisterName") is TextBlock name)
        {
            QStateConverter.QStateTextRefine(name, TextBlock.TextProperty, chip.PRegisterText);
        }

        if (QLook.QLookPartFind<Button>(container, "PRegisterEraser") is Button eraser)
        {
            eraser.Click -= QRegisterChipObserve;
            eraser.Click += QRegisterChipObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PRegisterIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    internal void QRegisterFieldApply(ItemsControl list, PCard card)
    {
        list.ItemTemplateSelector ??= new PRegisterSelector
        {
            PRegisterSelectorChip = QContract.QContractSheetFind<DataTemplate>(
                _qRegisterSurface, "Theme.Register.Chip"),
            PRegisterSelectorCaret = QContract.QContractSheetFind<DataTemplate>(
                _qRegisterSurface, "Theme.Register.Entry"),
        };
        QLookItem.QLookItemAttach(list, QRegisterApply);
        QLookItem.QLookItemAttach(
            QBerth.QBerthBuild(list, card.PCardRegisterCaret, nameof(PRegisterCaret.PRegisterCaretAnchor)),
            QRegisterApply);
        if (QLook.QLookPartFind<Border>(list, "PRegisterFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QRegisterFocusRefine;
            frame.MouseLeftButtonDown += QRegisterFocusRefine;
        }
    }

    internal void QRegisterTextObserve(PRegisterCaret caret, string text)
    {
        if (QRegisterCardFind(caret) is PCard card)
        {
            _qRegisterProffer.QProfferRegisterRefine(card, _cEditor.CEditorCard.CCardRegisterAdd(
                card.PCardId, text, card.PCardRegisterPosition, false));
        }
    }

    internal void QRegisterChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PRegister chip } && QRegisterCardFind(chip) is PCard card)
        {
            _cEditor.CEditorCard.CCardRegisterRemove(card.PCardId, chip.PRegisterId);
        }
    }

    internal void QRegisterCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PRegisterCaret row }
            || QRegisterCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = true;
        _qRegisterProffer.QProfferRegisterRefine(card, _cEditor.CEditorCard.CCardRegisterAdd(
            card.PCardId, card.PCardRegisterText, card.PCardRegisterPosition, true));
    }

    internal void QRegisterEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PRegisterCaret row } box || QRegisterCardFind(row) is not PCard card)
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
                if (card.PCardRegisterFind(step) is PRegister chip)
                {
                    _cEditor.CEditorCard.CCardRegisterRemove(card.PCardId, chip.PRegisterId);
                }
            });
    }

    internal void QRegisterCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PRegisterCaret row } box || QRegisterCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardRegisterMove,
            () => QField.QFieldCaretApply(box, row, 0));
    }

    internal void QRegisterBlurRefine(object sender, RoutedEventArgs e)
    {
        _qRegisterProffer.QProfferShutRefine();
    }

    internal void QRegisterCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PRegisterCaret row } && QRegisterCardFind(row) is PCard card)
        {
            _qRegisterProffer.QProfferRegisterRefine(card, _cEditor.CEditorCard.CCardRegisterAdd(
                card.PCardId, card.PCardRegisterText, card.PCardRegisterPosition, true));
        }
    }

    internal void QRegisterFocusRefine(object sender, MouseButtonEventArgs e)
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

    internal PCard? QRegisterCardFind(object row)
    {
        foreach (PCard card in _qRegisterMeaning)
        {
            if ((row is PRegister chip && card.PCardRegister.Contains(chip))
                || ReferenceEquals(card.PCardRegisterCaret, row))
            {
                return card;
            }
        }

        foreach (PCard card in _qRegisterCollocation)
        {
            if ((row is PRegister chip && card.PCardRegister.Contains(chip))
                || ReferenceEquals(card.PCardRegisterCaret, row))
            {
                return card;
            }
        }

        return null;
    }
}
