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

    private LImprint _lImprint = null!;

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

    internal void QImprintAttach(PWindow host, LImprint imprint)
    {
        _lImprint = imprint;
        _lImprint.LImprintChanged += QAuthorUpdate;
        _lImprint.LImprintFocused += QAuthorFocusDefer;
        _lImprint.LImprintReverted += QAuthorRestore;
        _lImprint.LBylineChanged += QBylineUpdate;
        _lImprint.LImprintDesk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectDraft,
            LObserver.LObserverCreate<CBulletin>(_qImprintSurface, _lImprint.LImprintDesk.LDeskDraftUpdate));
        _lImprint.LImprintDesk.LDeskVigil.QVigilDraftAttach(
            CSubject.CSubjectTenure,
            LObserver.LObserverCreate<CBulletin>(_qImprintSurface, _lImprint.LImprintDesk.LDeskStateUpdate));
        _lImprint.LImprintReferenceChanged += QImprintDraftUpdate;
        _lImprint.LImprintDesk.LDeskFailed += host.PWindowFailureShow;
    }

    internal void QImprintClear()
    {
        QImprintDraftUpdate(LImprint.LImprintEmpty);
    }

    internal void QImprintClose()
    {
        _lImprint.LBylineHide();
        QImprintKindMenu.IsOpen = false;
    }

    internal void QImprintTallyShow()
    {
        QImprintTally.Text = _lImprint.LImprintTallyRead();
    }

    public void QChronicleUndo()
    {
        QChronicle.QChronicleRun(_lImprint.LImprintDesk.LDeskUndo);
    }

    public void QChronicleRedo()
    {
        QChronicle.QChronicleRun(_lImprint.LImprintDesk.LDeskRedo);
    }

    public void QChronicleUpdate()
    {
        _lImprint.LImprintDesk.LDeskStateUpdate();
    }

    private void QImprintDraftUpdate(CImprint reference)
    {
        QImprintTitle.Text = reference.CImprintTitle;
        QImprintTitle.SetResourceReference(QField.QFieldHintProperty, reference.CImprintTitleHint);
        QImprintYear.Text = reference.CImprintYear;
        QImprintYear.SetResourceReference(QField.QFieldHintProperty, reference.CImprintYearHint);
        QImprintUrl.Text = reference.CImprintUrl;
        QImprintUrl.SetResourceReference(QField.QFieldHintProperty, reference.CImprintUrlHint);
        QImprintNote.Text = reference.CImprintNote;
        QImprintNote.SetResourceReference(QField.QFieldHintProperty, reference.CImprintNoteHint);
        QImprintKindName.SetResourceReference(TextBlock.TextProperty, reference.CImprintKindKey);
        QChoice.QChoiceMenuApply(QImprintKindList, reference.CImprintKindTag);
        QImprintTallyShow();
    }

    private void QAuthorUpdate()
    {
        LSplice.LSpliceApply(
            _qAuthorList,
            QAuthorItem.QAuthorItemBuild(_lImprint.LImprintCreditRead(), _lImprint.LImprintBlankAt),
            QAuthorItem.QAuthorItemMatch,
            QAuthorItem.QAuthorItemSync);
        QAuthorNotice.Visibility = QLook.QLookVisibleRead(!_lImprint.LImprintHeld);
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
        QBylineList.ItemsSource = QBylineItem.QBylineItemBuild(_lImprint.LBylineRowsRead(), _lImprint.LBylineWord);
        QByline.PlacementTarget = QField.QFieldSurfaceFind(Keyboard.FocusedElement);
        QBylineFrame.MinWidth = (QByline.PlacementTarget as FrameworkElement)?.ActualWidth ?? 0;
        QByline.IsOpen = _lImprint.LBylineShown;
        QBylineList.SelectedIndex = _lImprint.LBylineIndex;
        QBylineList.ScrollIntoView(QBylineList.SelectedItem);
    }

    private void QImprintTitleHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LImprintTitleSet(QImprintTitle.Text);
    }

    private void QImprintYearHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LImprintYearSet(QImprintYear.Text);
    }

    private void QImprintUrlHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LImprintUrlSet(QImprintUrl.Text);
    }

    private void QImprintNoteHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LImprintNoteSet(QImprintNote.Text);
    }

    private void QImprintKindHandle(object sender, RoutedEventArgs e)
    {
        QImprintKind.IsChecked = false;
        _lImprint.LImprintKindSet(QSender.QSenderTagRead(sender));
    }

    private void QAuthorAddHandle(object sender, RoutedEventArgs e)
    {
        _lImprint.LImprintAuthorAdd(
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemPosition,
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId);
    }

    private void QAuthorRemoveHandle(object sender, RoutedEventArgs e)
    {
        _lImprint.LImprintAuthorRemove(QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId);
    }

    private void QAuthorRetreatHandle(object sender, RoutedEventArgs e)
    {
        _lImprint.LImprintAuthorRetreat(
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemPosition,
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId);
    }

    private void QAuthorAdvanceHandle(object sender, RoutedEventArgs e)
    {
        _lImprint.LImprintAuthorAdvance(
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemPosition,
            QSender.QSenderItemRead<QAuthorItem>(sender)?.QAuthorItemId);
    }

    private void QAuthorTextHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LBylineWordSet(
            (e.OriginalSource as TextBox)?.Text,
            (e.OriginalSource as TextBox)?.IsKeyboardFocusWithin);
    }

    private void QAuthorKeyHandle(object sender, KeyEventArgs e)
    {
        e.Handled = _lImprint.LImprintKeyApply(
            QSender.QSenderKeyRead(e),
            QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemPosition,
            QSender.QSenderSourceRead<QAuthorItem>(e)?.QAuthorItemId,
            (e.OriginalSource as TextBox)?.Text,
            QBylineItem.QBylineItemRead(QBylineList.SelectedItem));
    }

    private void QAuthorLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        _lImprint.LBylineHide();
        QAuthorItem.QAuthorItemRestore(e.OriginalSource);
    }

    private void QBylineHandle(object sender, MouseButtonEventArgs e)
    {
        _lImprint.LBylineSelect(
            QSender.QSenderItemRead<QBylineItem>(sender)?.QBylineItemId,
            QSender.QSenderFocusRead<QAuthorItem>()?.QAuthorItemPosition,
            QSender.QSenderFocusRead<QAuthorItem>()?.QAuthorItemId);
        e.Handled = true;
    }
}
