using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QImprint : QChronicleHost
{
    private readonly ObservableCollection<QAuthorItem> _qAuthorList = [];

    private readonly UserControl _qImprintSurface;

    private CImprint _cImprint = null!;

    internal QImprint(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qImprintSurface = surface;
        QChronicle.QChronicleIntroduce(surface, this);

        QImprintKindIcon.QIconSource = QIcon.QIconResolve("expand", 12);

        QImprintTitle.TextChanged += QImprintTitleObserve;
        QImprintYear.TextChanged += QImprintYearObserve;
        QImprintUrl.TextChanged += QImprintUrlObserve;
        QImprintNote.TextChanged += QImprintNoteObserve;

        QAuthorCredit.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QAuthorTextObserve));
        QAuthorCredit.PreviewKeyDown += QAuthorKeyObserve;
        QAuthorCredit.LostKeyboardFocus += QAuthorLeaveObserve;
        QAuthorCredit.MouseEnter += (_, _) => QAuthorShelfRefine();
        QAuthorCredit.MouseLeave += (_, _) => QAuthorShelfRefine();
        QAuthorCredit.IsKeyboardFocusWithinChanged += (_, _) => QAuthorShelfRefine();

        QChoice.QChoiceDropperAttach(QImprintKind, QImprintKindMenu, QImprintKind);

        QAuthorCredit.ItemsSource = _qAuthorList;
        QByline.CustomPopupPlacementCallback = QField.QFieldPopupPlace;
        QChoice.QChoiceMenuBuild(QImprintKindList, QImprintKindObserve, CImprint.CImprintKindRead());

        QLookItem.QLookItemAttach(QAuthorCredit, (container, item, change) =>
        {
            QAuthorItem.QAuthorItemRefine(container, item, change);
            QAuthorShelfIntroduce(container);
            QAuthorShelfRefine();
        });
        QLookItem.QLookItemAttach(
            QBylineList,
            (container, item, _) => QBylineItem.QBylineItemRefine(container, item, QBylineObserve));
    }

    private TextBox QImprintTitle => QContract.QContractFind<TextBox>(_qImprintSurface, "PImprintTitle");

    private ToggleButton QImprintKind => QContract.QContractFind<ToggleButton>(_qImprintSurface, "PImprintKind");

    private TextBlock QImprintKindName => QContract.QContractFind<TextBlock>(_qImprintSurface, "PImprintKindName");

    private QIconImage QImprintKindIcon => QContract.QContractFind<QIconImage>(_qImprintSurface, "PImprintKindIcon");

    private Popup QImprintKindMenu => QContract.QContractFind<Popup>(_qImprintSurface, "PImprintKindMenu");

    private StackPanel QImprintKindList => QContract.QContractFind<StackPanel>(_qImprintSurface, "PImprintKindList");

    private TextBlock QImprintTally => QContract.QContractFind<TextBlock>(_qImprintSurface, "PImprintTally");

    private ItemsControl QAuthorCredit => QContract.QContractFind<ItemsControl>(_qImprintSurface, "PAuthorCredit");

    private TextBlock QAuthorNotice => QContract.QContractFind<TextBlock>(_qImprintSurface, "PAuthorNotice");

    private Popup QByline => QContract.QContractFind<Popup>(_qImprintSurface, "PByline");

    private Border QBylineFrame => QContract.QContractFind<Border>(_qImprintSurface, "PBylineFrame");

    private ListBox QBylineList => QContract.QContractFind<ListBox>(_qImprintSurface, "PBylineList");

    private TextBox QImprintYear => QContract.QContractFind<TextBox>(_qImprintSurface, "PImprintYear");

    private TextBox QImprintUrl => QContract.QContractFind<TextBox>(_qImprintSurface, "PImprintUrl");

    private TextBox QImprintNote => QContract.QContractFind<TextBox>(_qImprintSurface, "PImprintNote");

    internal void QImprintIntroduce(PWindow host, CImprint imprint)
    {
        _cImprint = imprint;
        _cImprint.CImprintChanged += QAuthorRefine;
        _cImprint.CImprintFocused += QAuthorFocusRefine;
        _cImprint.CImprintReverted += QAuthorRestoreRefine;
        _cImprint.CImprintByline.CBylineChanged += QBylineRefine;
        _cImprint.CImprintReferenceChanged += QImprintDraftRefine;
        _cImprint.CImprintDesk.CDeskFailed += host.PWindowFailureRefine;
    }

    internal void QImprintClearRefine()
    {
        QImprintDraftRefine(_cImprint.CImprintEmptyRead());
    }

    internal void QImprintCloseRefine()
    {
        QImprintKindMenu.IsOpen = false;
    }

    internal void QImprintTallyRefine()
    {
        QImprintTally.Text = _cImprint.CImprintTallyRead();
    }

    public void QChronicleUndoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cImprint.CImprintDesk.CDeskUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cImprint.CImprintDesk.CDeskRedo);
    }

    private void QImprintDraftRefine(CReference reference)
    {
        QImprintTitle.Text = reference.CReferenceTitle;
        QImprintTitle.SetResourceReference(QField.QFieldHintProperty, reference.CReferenceTitleHint);
        QImprintYear.Text = reference.CReferenceYear;
        QImprintYear.SetResourceReference(QField.QFieldHintProperty, reference.CReferenceYearHint);
        QImprintUrl.Text = reference.CReferenceUrl;
        QImprintUrl.SetResourceReference(QField.QFieldHintProperty, reference.CReferenceUrlHint);
        QImprintNote.Text = reference.CReferenceNote;
        QImprintNote.SetResourceReference(QField.QFieldHintProperty, reference.CReferenceNoteHint);
        QImprintKindName.SetResourceReference(TextBlock.TextProperty, reference.CReferenceKindKey);
        QChoice.QChoiceMenuApply(QImprintKindList, reference.CReferenceKindTag);
        QImprintTallyRefine();
    }

    private void QAuthorRefine()
    {
        LSplice.LSpliceApply(
            _qAuthorList,
            QAuthorItem.QAuthorItemBuild(_cImprint.CImprintCreditRead()),
            QAuthorItem.QAuthorItemMatch,
            QAuthorItem.QAuthorItemSync);
        QAuthorNotice.Visibility = QLook.QLookVisibleRead(!_cImprint.CImprintHeld);
    }

    private void QAuthorShelfIntroduce(FrameworkElement container)
    {
        if (QLook.QLookPartFind<Button>(container, "PAuthorAddition") is Button addition)
        {
            addition.Click -= QAuthorAddObserve;
            addition.Click += QAuthorAddObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PAuthorRemoval") is Button removal)
        {
            removal.Click -= QAuthorRemoveObserve;
            removal.Click += QAuthorRemoveObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PAuthorEarlier") is Button earlier)
        {
            earlier.Click -= QAuthorRetreatObserve;
            earlier.Click += QAuthorRetreatObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PAuthorLater") is Button later)
        {
            later.Click -= QAuthorAdvanceObserve;
            later.Click += QAuthorAdvanceObserve;
        }
    }

    private void QAuthorShelfRefine()
    {
        bool shown = QAuthorCredit.IsMouseOver || QAuthorCredit.IsKeyboardFocusWithin;
        foreach (object item in QAuthorCredit.Items)
        {
            if (QAuthorCredit.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement container
                && QLook.QLookPartFind<FrameworkElement>(container, "PAuthorShelf") is FrameworkElement shelf)
            {
                shelf.Opacity = shown ? 1 : 0;
                shelf.IsHitTestVisible = shown;
            }
        }
    }

    private void QAuthorFocusRefine()
    {
        QField.QFieldFocusDefer(QAuthorCredit, QAuthorItem.QAuthorItemFind(_qAuthorList));
    }

    private void QAuthorRestoreRefine()
    {
        QAuthorItem.QAuthorItemRefine(Keyboard.FocusedElement);
    }

    private void QBylineRefine()
    {
        QBylineList.ItemsSource = QBylineItem.QBylineItemBuild(_cImprint.CImprintByline.CBylineRowsRead());
        QByline.PlacementTarget = QField.QFieldSurfaceFind(Keyboard.FocusedElement);
        QBylineFrame.MinWidth = (QByline.PlacementTarget as FrameworkElement)?.ActualWidth ?? 0;
        QByline.IsOpen = _cImprint.CImprintByline.CBylineShown;
        QBylineList.SelectedIndex = _cImprint.CImprintByline.CBylineIndex;
        QBylineList.ScrollIntoView(QBylineList.SelectedItem);
    }

    private void QImprintTitleObserve(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintTitleSet(QImprintTitle.Text);
    }

    private void QImprintYearObserve(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintYearSet(QImprintYear.Text);
    }

    private void QImprintUrlObserve(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintUrlSet(QImprintUrl.Text);
    }

    private void QImprintNoteObserve(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintNoteSet(QImprintNote.Text);
    }

    private void QImprintKindObserve(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintKindSet(QSender.QSenderTagRead(sender));
        QImprintKindRefine();
    }

    private void QImprintKindRefine()
    {
        QImprintKind.IsChecked = false;
    }

    private void QAuthorAddObserve(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintAuthorAdd(
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemPosition,
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId);
    }

    private void QAuthorRemoveObserve(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintAuthorRemove(QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId);
    }

    private void QAuthorRetreatObserve(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintAuthorMove(
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemPosition,
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId,
            -1);
    }

    private void QAuthorAdvanceObserve(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintAuthorMove(
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemPosition,
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId,
            1);
    }

    private void QAuthorTextObserve(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintByline.CBylineWordSet(
            (e.OriginalSource as TextBox)?.Text,
            (e.OriginalSource as TextBox)?.IsKeyboardFocusWithin);
    }

    private void QAuthorKeyObserve(object sender, KeyEventArgs e)
    {
        e.Handled = QSender.QSenderKeyRead(e) switch
        {
            "Enter" => _cImprint.CImprintAuthorFinish(
                QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemPosition,
                QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemId,
                (e.OriginalSource as TextBox)?.Text,
                QBylineItem.QBylineItemRead(QBylineList.SelectedItem)),
            "Escape" => _cImprint.CImprintAuthorCancel(
                QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemPosition,
                QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemId),
            "Down" => _cImprint.CImprintByline.CBylineMove(
                QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemPosition,
                QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemId,
                1),
            "Up" => _cImprint.CImprintByline.CBylineMove(
                QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemPosition,
                QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemId,
                -1),
            _ => false,
        };
    }

    private void QAuthorLeaveObserve(object sender, KeyboardFocusChangedEventArgs e)
    {
        _cImprint.CImprintByline.CBylineClose();
        QAuthorItem.QAuthorItemRefine(e.OriginalSource);
    }

    private void QBylineObserve(object sender, MouseButtonEventArgs e)
    {
        _cImprint.CImprintByline.CBylineSelect(
            QSender.QSenderItemRead<QBylineItem>(sender)?.QBylineItemId,
            QSender.QSenderFocusRead<QAuthorItem>()?.QAuthorItemPosition,
            QSender.QSenderFocusRead<QAuthorItem>()?.QAuthorItemId);
        e.Handled = true;
    }
}
