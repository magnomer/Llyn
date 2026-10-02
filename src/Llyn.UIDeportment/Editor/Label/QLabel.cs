using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLabel
{
    private readonly FrameworkElement _qLabelSurface;

    private readonly ObservableCollection<PCard> _qLabelMeaning;

    private readonly ObservableCollection<PCard> _qLabelCollocation;

    private QSlate _qLabelSlate = null!;

    private CEditor _cEditor = null!;

    internal QLabel(
        FrameworkElement surface, ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
    {
        _qLabelSurface = surface;
        _qLabelMeaning = meaning;
        _qLabelCollocation = collocation;
    }

    internal void QLabelIntroduce(CEditor editor, QSlate slate)
    {
        _cEditor = editor;
        _qLabelSlate = slate;
    }

    internal void QLabelApply(FrameworkElement container, object item, string? _)
    {
        if (item is PLabelCaret caret)
        {
            if (QLook.QLookPartFind<TextBox>(container, "PLabelEntry") is TextBox entry)
            {
                entry.SetValue(QField.QFieldHintProperty, caret.PLabelCaretHint);
                entry.Text = caret.PLabelCaretText;
                entry.PreviewKeyDown -= _qLabelSlate.QSlateKeyRefine;
                entry.PreviewKeyDown -= _qLabelSlate.QSlateKeyObserve;
                entry.PreviewKeyDown -= QLabelCommitObserve;
                entry.PreviewKeyDown -= QLabelEraseObserve;
                entry.PreviewKeyDown -= QLabelCaretRefine;
                entry.PreviewKeyDown += _qLabelSlate.QSlateKeyRefine;
                entry.PreviewKeyDown += _qLabelSlate.QSlateKeyObserve;
                entry.PreviewKeyDown += QLabelCommitObserve;
                entry.PreviewKeyDown += QLabelEraseObserve;
                entry.PreviewKeyDown += QLabelCaretRefine;
                entry.LostKeyboardFocus -= QLabelBlurRefine;
                entry.LostKeyboardFocus -= QLabelCloseObserve;
                entry.LostKeyboardFocus += QLabelBlurRefine;
                entry.LostKeyboardFocus += QLabelCloseObserve;
            }

            return;
        }

        if (item is not PLabelChip chip)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLabelName") is TextBlock name)
        {
            name.Text = chip.PLabelChipName;
        }

        if (QLook.QLookPartFind<Button>(container, "PLabelEraser") is Button eraser)
        {
            eraser.Click -= QLabelChipObserve;
            eraser.Click += QLabelChipObserve;
            if (QLook.QLookPartFind<QIconImage>(eraser, "PLabelIcon") is QIconImage icon)
            {
                icon.QIconSource = QIcon.QIconResolve("close", 12);
            }
        }
    }

    internal void QLabelFieldApply(ItemsControl list, PCard card)
    {
        list.ItemTemplateSelector ??= new PLabelSelector
        {
            PLabelSelectorChip = QContract.QContractSheetFind<DataTemplate>(
                _qLabelSurface, "Theme.Label.Chip"),
            PLabelSelectorCaret = QContract.QContractSheetFind<DataTemplate>(
                _qLabelSurface, "Theme.Label.Entry"),
        };
        QLookItem.QLookItemAttach(list, QLabelApply);
        QLookItem.QLookItemAttach(
            QBerth.QBerthBuild(list, card.PCardLabelCaret, nameof(PLabelCaret.PLabelCaretAnchor)),
            QLabelApply);
        if (QLook.QLookPartFind<Border>(list, "PLabelFrame") is Border frame)
        {
            frame.MouseLeftButtonDown -= QLabelFocusRefine;
            frame.MouseLeftButtonDown += QLabelFocusRefine;
        }
    }

    internal void QLabelTextObserve(PLabelCaret caret, string text)
    {
        if (QLabelCardFind(caret) is PCard card)
        {
            _qLabelSlate.QSlateRefine(card, _cEditor.CEditorCard.CCardTagAdd(
                card.PCardId, text, card.PCardLabelPosition, false));
        }
    }

    internal void QLabelChipObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLabelChip chip } && QLabelCardFind(chip) is PCard card)
        {
            _cEditor.CEditorCard.CCardTagRemove(card.PCardId, chip.PLabelChipId);
        }
    }

    internal void QLabelCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not FrameworkElement { DataContext: PLabelCaret row }
            || QLabelCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorCard.CCardTagAdd(
            card.PCardId, card.PCardLabelText, card.PCardLabelPosition, true);
        e.Handled = true;
        _qLabelSlate.QSlateShutRefine();
        card.PCardLabelClear();
    }

    internal void QLabelEraseObserve(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLabelCaret row } box || QLabelCardFind(row) is not PCard card)
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
                if (card.PCardLabelFind(step) is PLabelChip chip)
                {
                    _cEditor.CEditorCard.CCardTagRemove(card.PCardId, chip.PLabelChipId);
                }
            });
    }

    internal void QLabelCaretRefine(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PLabelCaret row } box || QLabelCardFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = QCaret.QCaretStepApply(
            e.Key.ToString(),
            box.Text.Length,
            box.SelectionLength,
            card.PCardLabelMove,
            () => QField.QFieldCaretApply(box, row, 0));
    }

    internal void QLabelBlurRefine(object sender, RoutedEventArgs e)
    {
        _qLabelSlate.QSlateShutRefine();
    }

    internal void QLabelCloseObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: PLabelCaret row } && QLabelCardFind(row) is PCard card)
        {
            _cEditor.CEditorCard.CCardTagAdd(
                card.PCardId, card.PCardLabelText, card.PCardLabelPosition, true);
            card.PCardLabelClear();
        }
    }

    internal void QLabelFocusRefine(object sender, MouseButtonEventArgs e)
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

    internal PCard? QLabelCardFind(object row)
    {
        foreach (PCard card in _qLabelMeaning)
        {
            if ((row is PLabelChip chip && card.PCardLabel.Contains(chip))
                || ReferenceEquals(card.PCardLabelCaret, row))
            {
                return card;
            }
        }

        foreach (PCard card in _qLabelCollocation)
        {
            if ((row is PLabelChip chip && card.PCardLabel.Contains(chip))
                || ReferenceEquals(card.PCardLabelCaret, row))
            {
                return card;
            }
        }

        return null;
    }
}
