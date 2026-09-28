using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;
using Llyn.Core;

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
        QChronicle.QChronicleAttach(surface, this);

        QImprintKindIcon.QIconSource = QIcon.QIconResolve("expand", 12);

        QImprintTitle.TextChanged += QImprintTitleHandle;
        QImprintYear.TextChanged += QImprintYearHandle;
        QImprintUrl.TextChanged += QImprintUrlHandle;
        QImprintNote.TextChanged += QImprintNoteHandle;

        QAuthorCredit.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(QAuthorTextHandle));
        QAuthorCredit.PreviewKeyDown += QAuthorKeyHandle;
        QAuthorCredit.LostKeyboardFocus += QAuthorLeaveHandle;
        QAuthorCredit.MouseEnter += (_, _) => QAuthorShelfUpdate();
        QAuthorCredit.MouseLeave += (_, _) => QAuthorShelfUpdate();
        QAuthorCredit.IsKeyboardFocusWithinChanged += (_, _) => QAuthorShelfUpdate();

        QChoice.QChoiceDropperAttach(QImprintKind, QImprintKindMenu, QImprintKind);

        QAuthorCredit.ItemsSource = _qAuthorList;
        QByline.CustomPopupPlacementCallback = QField.QFieldPopupPlace;
        QChoice.QChoiceMenuBuild(QImprintKindList, QImprintKindHandle, LReference.LReferenceKindMenu);

        QLookItem.QLookItemAttach(QAuthorCredit, (container, item, change) =>
        {
            QAuthorItem.QAuthorItemApply(container, item, change);
            QAuthorShelfAttach(container);
            QAuthorShelfUpdate();
        });
        QLookItem.QLookItemAttach(
            QBylineList,
            (container, item, _) => QBylineItem.QBylineItemApply(container, item, QBylineHandle));
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

    internal void QImprintAttach(PWindow host, CImprint imprint)
    {
        _cImprint = imprint;
        _cImprint.CImprintChanged += QAuthorUpdate;
        _cImprint.CImprintFocused += QAuthorFocusDefer;
        _cImprint.CImprintReverted += QAuthorRestore;
        _cImprint.CImprintByline.CBylineChanged += QBylineUpdate;
        _cImprint.CImprintDesk.CDeskObserverAttach(LObserver.LObserverCreate<Action>(static run => run()));
        _cImprint.CImprintReferenceChanged += QImprintDraftUpdate;
        _cImprint.CImprintDesk.CDeskFailed += host.PWindowFailureShow;
    }

    internal void QImprintClear()
    {
        QImprintDraftUpdate(_cImprint.CImprintEmptyRead());
    }

    internal void QImprintClose()
    {
        _cImprint.CImprintByline.CBylineClose();
        QImprintKindMenu.IsOpen = false;
    }

    internal void QImprintTallyShow()
    {
        QImprintTally.Text = _cImprint.CImprintTallyRead();
    }

    public void QChronicleUndo()
    {
        QChronicle.QChronicleRun(_cImprint.CImprintDesk.CDeskUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_cImprint.CImprintDesk.CDeskRedo);
    }

    public void QChronicleUpdate()
    {
        _cImprint.CImprintDesk.CDeskStateUpdate();
    }

    private void QImprintDraftUpdate(CReference reference)
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
        QImprintTallyShow();
    }

    private void QAuthorUpdate()
    {
        LSplice.LSpliceApply(
            _qAuthorList,
            QAuthorItem.QAuthorItemBuild(_cImprint.CImprintCreditRead(), _cImprint.CImprintBlankAt),
            QAuthorItem.QAuthorItemMatch,
            QAuthorItem.QAuthorItemSync);
        QAuthorNotice.Visibility = QLook.QLookVisibleRead(!_cImprint.CImprintHeld);
    }

    private void QAuthorShelfAttach(FrameworkElement container)
    {
        if (QLook.QLookPartFind<Button>(container, "PAuthorAddition") is Button addition)
        {
            addition.Click -= QAuthorAddHandle;
            addition.Click += QAuthorAddHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PAuthorRemoval") is Button removal)
        {
            removal.Click -= QAuthorRemoveHandle;
            removal.Click += QAuthorRemoveHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PAuthorEarlier") is Button earlier)
        {
            earlier.Click -= QAuthorRetreatHandle;
            earlier.Click += QAuthorRetreatHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PAuthorLater") is Button later)
        {
            later.Click -= QAuthorAdvanceHandle;
            later.Click += QAuthorAdvanceHandle;
        }
    }

    private void QAuthorShelfUpdate()
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

    private void QAuthorFocusDefer()
    {
        QField.QFieldFocusDefer(QAuthorCredit, QAuthorItem.QAuthorItemFind(_qAuthorList));
    }

    private void QAuthorRestore()
    {
        QAuthorItem.QAuthorItemRestore(Keyboard.FocusedElement);
    }

    private void QBylineUpdate()
    {
        QBylineList.ItemsSource = QBylineItem.QBylineItemBuild(
            _cImprint.CImprintByline.CBylineRowsRead(), _cImprint.CImprintByline.CBylineWord);
        QByline.PlacementTarget = QField.QFieldSurfaceFind(Keyboard.FocusedElement);
        QBylineFrame.MinWidth = (QByline.PlacementTarget as FrameworkElement)?.ActualWidth ?? 0;
        QByline.IsOpen = _cImprint.CImprintByline.CBylineShown;
        QBylineList.SelectedIndex = _cImprint.CImprintByline.CBylineIndex;
        QBylineList.ScrollIntoView(QBylineList.SelectedItem);
    }

    private void QImprintTitleHandle(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintTitleSet(QImprintTitle.Text);
    }

    private void QImprintYearHandle(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintYearSet(QImprintYear.Text);
    }

    private void QImprintUrlHandle(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintUrlSet(QImprintUrl.Text);
    }

    private void QImprintNoteHandle(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintNoteSet(QImprintNote.Text);
    }

    private void QImprintKindHandle(object sender, RoutedEventArgs e)
    {
        QImprintKind.IsChecked = false;
        _cImprint.CImprintKindSet(QSender.QSenderTagRead(sender));
    }

    private void QAuthorAddHandle(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintAuthorAdd(
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemPosition,
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId);
    }

    private void QAuthorRemoveHandle(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintAuthorRemove(QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId);
    }

    private void QAuthorRetreatHandle(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintAuthorMove(
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemPosition,
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId,
            -1);
    }

    private void QAuthorAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _cImprint.CImprintAuthorMove(
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemPosition,
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId,
            1);
    }

    private void QAuthorTextHandle(object sender, TextChangedEventArgs e)
    {
        _cImprint.CImprintByline.CBylineWordSet(
            (e.OriginalSource as TextBox)?.Text,
            (e.OriginalSource as TextBox)?.IsKeyboardFocusWithin);
    }

    private void QAuthorKeyHandle(object sender, KeyEventArgs e)
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

    private void QAuthorLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        _cImprint.CImprintByline.CBylineClose();
        QAuthorItem.QAuthorItemRestore(e.OriginalSource);
    }

    private void QBylineHandle(object sender, MouseButtonEventArgs e)
    {
        _cImprint.CImprintByline.CBylineSelect(
            QSender.QSenderItemRead<QBylineItem>(sender)?.QBylineItemId,
            QSender.QSenderFocusRead<QAuthorItem>()?.QAuthorItemPosition,
            QSender.QSenderFocusRead<QAuthorItem>()?.QAuthorItemId);
        e.Handled = true;
    }
}
