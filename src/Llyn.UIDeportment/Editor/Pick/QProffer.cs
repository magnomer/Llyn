using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QProffer
{
    private const double QProfferShade = 10;
    private const double QProfferGap = 6;

    private readonly FrameworkElement _qProfferSurface;

    private readonly QContext _qProfferContext;

    private readonly QRegister _qProfferRegister;

    private readonly QSentence _qProfferSentence;

    private readonly ObservableCollection<PProfferItem> _qProfferItem = [];

    private CEditor _cEditor = null!;

    internal QProffer(FrameworkElement surface, QContext context, QRegister register, QSentence sentence)
    {
        _qProfferSurface = surface;
        _qProfferSentence = sentence;
        _qProfferContext = context;
        _qProfferRegister = register;
        QProfferList.ItemsSource = _qProfferItem;
        QLookItem.QLookItemAttach(QProfferList, QProfferApply);
        QProfferPopup.CustomPopupPlacementCallback = QProfferPlace;
        QProfferPopup.Closed += QProfferCloseRefine;
    }

    private Popup QProfferPopup => QContract.QContractFind<Popup>(_qProfferSurface, "PProffer");

    private Border QProfferSheet => QContract.QContractFind<Border>(_qProfferSurface, "PProfferSheet");

    private ListBox QProfferList => QContract.QContractFind<ListBox>(_qProfferSurface, "PProfferList");

    private Border QProfferContents => QContract.QContractFind<Border>(_qProfferSurface, "PContents");

    internal void QProfferIntroduce(CEditor editor)
    {
        _cEditor = editor;
    }

    private void QProfferApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PProfferItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Run>(container, "PProfferLead") is Run lead)
        {
            lead.Text = row.PProfferItemLead;
        }

        if (QLook.QLookPartFind<Run>(container, "PProfferMark") is Run mark)
        {
            mark.Text = row.PProfferItemMark;
        }

        if (QLook.QLookPartFind<Run>(container, "PProfferTail") is Run tail)
        {
            tail.Text = row.PProfferItemTail;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PProfferCount") is TextBlock count)
        {
            count.Text = row.PProfferItemCount;
        }

        if (QLook.QLookPartFind<Grid>(container, "PProfferRow") is Grid surface)
        {
            surface.PreviewMouseLeftButtonDown -= QProfferMissRefine;
            surface.PreviewMouseLeftButtonDown -= QProfferPickObserve;
            surface.PreviewMouseLeftButtonDown += QProfferMissRefine;
            surface.PreviewMouseLeftButtonDown += QProfferPickObserve;
        }
    }

    private void QProfferMissRefine(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PProfferItem })
        {
            QProfferShutRefine();
        }
    }

    private void QProfferPickObserve(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PProfferItem item })
        {
            return;
        }

        switch (Keyboard.FocusedElement)
        {
            case FrameworkElement { DataContext: PRegisterCaret caret }
                when _qProfferRegister.QRegisterCardFind(caret) is PCard card:
                _cEditor.CEditorCard.CCardRegisterInsert(
                    card.PCardId, item.PProfferItemId, card.PCardRegisterPosition);
                e.Handled = true;
                QProfferShutRefine();
                card.PCardRegisterClear();
                break;
            case FrameworkElement { DataContext: PContextCaret caret }
                when _qProfferContext.QContextCardFind(caret) is PCard card:
                _cEditor.CEditorCard.CCardSituationInsert(
                    card.PCardId, item.PProfferItemId, card.PCardContextPosition);
                e.Handled = true;
                QProfferShutRefine();
                card.PCardContextClear();
                break;
            case FrameworkElement { DataContext: PSentence row }
                when _qProfferSentence.QSentenceCardFind(row) is PCard card:
                _cEditor.CEditorSentence.CSentenceCitationSet(
                    card.PCardId, row.PSentenceRow, item.PProfferItemId);
                e.Handled = true;
                QProfferShutRefine();
                break;
        }
    }

    internal void QProfferKeyRefine(object sender, KeyEventArgs e)
    {
        int? lit = e.Key switch
        {
            Key.Down => CLantern.CLanternMove(QProfferList.SelectedIndex, QProfferList.Items.Count, 1),
            Key.Up => CLantern.CLanternMove(QProfferList.SelectedIndex, QProfferList.Items.Count, -1),
            _ => null,
        };
        if (lit is int chosen)
        {
            QProfferList.SelectedIndex = chosen;
            QProfferList.ScrollIntoView(QProfferList.SelectedItem);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape && QProfferPopup.IsOpen)
        {
            QProfferShutRefine();
            e.Handled = true;
        }
    }

    private void QProfferCloseRefine(object? sender, EventArgs e)
    {
        QProfferList.SelectedIndex = -1;
        _qProfferItem.Clear();
    }

    internal void QProfferKeyObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        if (QProfferList.SelectedItem is not PProfferItem item)
        {
            return;
        }

        switch (sender)
        {
            case FrameworkElement { DataContext: PRegisterCaret caret }
                when _qProfferRegister.QRegisterCardFind(caret) is PCard card:
                _cEditor.CEditorCard.CCardRegisterInsert(
                    card.PCardId, item.PProfferItemId, card.PCardRegisterPosition);
                e.Handled = true;
                QProfferShutRefine();
                card.PCardRegisterClear();
                break;
            case FrameworkElement { DataContext: PContextCaret caret }
                when _qProfferContext.QContextCardFind(caret) is PCard card:
                _cEditor.CEditorCard.CCardSituationInsert(
                    card.PCardId, item.PProfferItemId, card.PCardContextPosition);
                e.Handled = true;
                QProfferShutRefine();
                card.PCardContextClear();
                break;
            case FrameworkElement { DataContext: PSentence row }
                when _qProfferSentence.QSentenceCardFind(row) is PCard card:
                _cEditor.CEditorSentence.CSentenceCitationSet(
                    card.PCardId, row.PSentenceRow, item.PProfferItemId);
                e.Handled = true;
                QProfferShutRefine();
                break;
        }
    }

    internal void QProfferRegisterRefine(PCard card, CProffer offer)
    {
        card.PCardRegisterRefine(offer.CProfferText);
        if (!offer.CProfferShown)
        {
            QProfferShutRefine();
            return;
        }

        QProfferOpenRefine(QProfferRegisterFind(card), offer.CProfferRows);
    }

    private void QProfferOpenRefine(TextBox? box, IReadOnlyList<CProfferRow> rows)
    {
        _qProfferItem.Clear();
        foreach (CProfferRow row in rows)
        {
            _qProfferItem.Add(new PProfferItem(
                row.CProfferRowId,
                row.CProfferRowLead,
                row.CProfferRowMark,
                row.CProfferRowTail,
                row.CProfferRowCount));
        }

        QProfferPopup.PlacementTarget = QProfferFrameFind(box) ?? box ?? (UIElement)QProfferContents;
        QProfferSheet.SetBinding(
            FrameworkElement.MinWidthProperty,
            new Binding(nameof(FrameworkElement.ActualWidth)) { Source = QProfferPopup.PlacementTarget });
        QProfferPopup.IsOpen = true;
        QProfferList.SelectedIndex = -1;
    }

    internal void QProfferSituationRefine(PCard card, CProffer offer)
    {
        card.PCardContextRefine(offer.CProfferText);
        if (!offer.CProfferShown)
        {
            QProfferShutRefine();
            return;
        }

        QProfferOpenRefine(QProfferBoxFind(card), offer.CProfferRows);
    }

    internal void QProfferCitationRefine(TextBox box, CProffer offer)
    {
        if (!offer.CProfferShown)
        {
            QProfferShutRefine();
            return;
        }

        QProfferOpenRefine(box, offer.CProfferRows);
    }

    internal void QProfferShutRefine()
    {
        QProfferPopup.IsOpen = false;
        QProfferList.SelectedIndex = -1;
        _qProfferItem.Clear();
    }

    private TextBox? QProfferRegisterFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PRegisterCaret row } box &&
            _qProfferRegister.QRegisterCardFind(row) == card
            ? box
            : null;
    }

    private TextBox? QProfferBoxFind(PCard card)
    {
        return Keyboard.FocusedElement is TextBox { DataContext: PContextCaret row } box &&
            _qProfferContext.QContextCardFind(row) == card
            ? box
            : null;
    }

    private static CustomPopupPlacement[] QProfferPlace(Size popup, Size target, Point offset)
    {
        var pBelow = new Point(-QProfferShade, target.Height + QProfferGap - QProfferShade);
        var pAbove = new Point(-QProfferShade, QProfferShade - QProfferGap - popup.Height);
        return
        [
            new CustomPopupPlacement(pBelow, PopupPrimaryAxis.Vertical),
            new CustomPopupPlacement(pAbove, PopupPrimaryAxis.Vertical),
        ];
    }

    private static FrameworkElement? QProfferFrameFind(TextBox? box)
    {
        if (box is null)
        {
            return null;
        }

        box.ApplyTemplate();
        return box.Template?.FindName(QField.QFieldSurfaceName, box) as FrameworkElement;
    }
}
