using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public class PImprint : UserControl, PChronicleHost
{
    private readonly ObservableCollection<PAuthorItem> _pAuthorList = [];

    private readonly PBylineTemplate _pBylineTemplate;

    private LImprint _lImprint = null!;

    public PImprint()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Imprint/PImprint.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));
        QLook.QLookStyleAttach(surface.Resources);

        _pBylineTemplate = new PBylineTemplate(this);
        Resources.MergedDictionaries.Add(_pBylineTemplate);

        PImprintKindIcon.QIconSource = QIcon.QIconResolve("expand", 12);

        PImprintTitle.TextChanged += PImprintTitleHandle;
        PImprintYear.TextChanged += PImprintYearHandle;
        PImprintUrl.TextChanged += PImprintUrlHandle;
        PImprintNote.TextChanged += PImprintNoteHandle;

        PAuthorCredit.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PAuthorCreditHandle));
        PAuthorCredit.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(PAuthorTextHandle));
        PAuthorCredit.PreviewKeyDown += PAuthorKeyHandle;
        PAuthorCredit.LostKeyboardFocus += PAuthorLeaveHandle;
        PAuthorCredit.MouseEnter += (_, _) => PAuthorShelfUpdate();
        PAuthorCredit.MouseLeave += (_, _) => PAuthorShelfUpdate();
        PAuthorCredit.IsKeyboardFocusWithinChanged += (_, _) => PAuthorShelfUpdate();

        QChoice.QChoiceDropperAttach(PImprintKind, PImprintKindMenu, PImprintKind);

        PAuthorCredit.ItemsSource = _pAuthorList;
        PByline.CustomPopupPlacementCallback = QField.QFieldPopupPlace;
        QChoice.QChoiceMenuBuild(PImprintKindList, PImprintKindHandle, LReference.LReferenceKindMenu);

        QLookItem.QLookItemAttach(PAuthorCredit, (container, item, change) =>
        {
            PAuthorItem.PAuthorItemApply(container, item, change);
            PAuthorShelfUpdate();
        });
        QLookItem.QLookItemAttach(
            PBylineList,
            (container, item, _) =>PBylineItem.PBylineItemApply(container, item, _pBylineTemplate.PBylineHandle));
    }

    private TextBox PImprintTitle => (TextBox)FindName(nameof(PImprintTitle));

    private ToggleButton PImprintKind => (ToggleButton)FindName(nameof(PImprintKind));

    private TextBlock PImprintKindName => (TextBlock)FindName(nameof(PImprintKindName));

    private QIconImage PImprintKindIcon => (QIconImage)FindName(nameof(PImprintKindIcon));

    private Popup PImprintKindMenu => (Popup)FindName(nameof(PImprintKindMenu));

    private StackPanel PImprintKindList => (StackPanel)FindName(nameof(PImprintKindList));

    private TextBlock PImprintTally => (TextBlock)FindName(nameof(PImprintTally));

    private ItemsControl PAuthorCredit => (ItemsControl)FindName(nameof(PAuthorCredit));

    private TextBlock PAuthorNotice => (TextBlock)FindName(nameof(PAuthorNotice));

    private Popup PByline => (Popup)FindName(nameof(PByline));

    private Border PBylineFrame => (Border)FindName(nameof(PBylineFrame));

    private ListBox PBylineList => (ListBox)FindName(nameof(PBylineList));

    private TextBox PImprintYear => (TextBox)FindName(nameof(PImprintYear));

    private TextBox PImprintUrl => (TextBox)FindName(nameof(PImprintUrl));

    private TextBox PImprintNote => (TextBox)FindName(nameof(PImprintNote));

    internal void PImprintAttach(PWindow host, LImprint imprint)
    {
        _lImprint = imprint;
        _lImprint.LImprintChanged += PAuthorUpdate;
        _lImprint.LImprintFocused += PAuthorFocusDefer;
        _lImprint.LImprintReverted += PAuthorRestore;
        _lImprint.LBylineChanged += PBylineUpdate;
        _lImprint.LImprintDesk.LDeskDraftAttach(
            LSubject.LSubjectDraft, LObserver.LObserverCreate(this, _lImprint.LImprintDesk.LDeskDraftUpdate));
        _lImprint.LImprintDesk.LDeskDraftAttach(
            LSubject.LSubjectTenure, LObserver.LObserverCreate(this, _lImprint.LImprintDesk.LDeskStateUpdate));
        _lImprint.LImprintDesk.LDeskDraftChanged += PImprintDraftUpdate;
        _lImprint.LImprintDesk.LDeskFailed += host.PWindowFailureShow;
    }

    internal void PImprintClear()
    {
        PImprintTitle.Text = string.Empty;
        PImprintTitle.SetResourceReference(QField.QFieldHintProperty, "Source.Untitled");
        PImprintYear.Text = string.Empty;
        PImprintYear.SetResourceReference(QField.QFieldHintProperty, "Source.Year");
        PImprintUrl.Text = string.Empty;
        PImprintUrl.SetResourceReference(QField.QFieldHintProperty, "Source.Url");
        PImprintNote.Text = string.Empty;
        PImprintNote.SetResourceReference(QField.QFieldHintProperty, "Source.Note");
        PImprintKindShow(
            LReference.LReferenceKindResolve(LReferenceKind.LReferenceKindUnspecified),
            LReference.LReferenceKindFormat(LReferenceKind.LReferenceKindUnspecified));
        PImprintTallyShow();
    }

    internal void PImprintClose()
    {
        _lImprint.LBylineHide();
        PImprintKindMenu.IsOpen = false;
    }

    internal void PImprintTallyShow()
    {
        PImprintTally.Text = _lImprint.LImprintTallyRead();
    }

    public void PChronicleUndo()
    {
        PChronicle.PChronicleRun(_lImprint.LImprintDesk.LDeskUndo);
    }

    public void PChronicleRedo()
    {
        PChronicle.PChronicleRun(_lImprint.LImprintDesk.LDeskRedo);
    }

    public void PChronicleUpdate()
    {
        _lImprint.LImprintDesk.LDeskStateUpdate();
    }

    private void PImprintDraftUpdate(LDraft draft)
    {
        LReference reference = _lImprint.LImprintReferenceRead(draft);
        PImprintTitle.Text = reference.LReferenceTitle.LStateValueShow();
        PImprintTitle.SetResourceReference(QField.QFieldHintProperty, reference.LReferenceTitleHint);
        PImprintYear.Text = reference.LReferenceYear.LStateValueShow();
        PImprintYear.SetResourceReference(QField.QFieldHintProperty, reference.LReferenceYearHint);
        PImprintUrl.Text = reference.LReferenceUrl.LStateValueShow();
        PImprintUrl.SetResourceReference(QField.QFieldHintProperty, reference.LReferenceUrlHint);
        PImprintNote.Text = reference.LReferenceNote.LStateValueShow();
        PImprintNote.SetResourceReference(QField.QFieldHintProperty, reference.LReferenceNoteHint);
        PImprintKindShow(reference.LReferenceKindKey, reference.LReferenceKindTag);
        PImprintTallyShow();
    }

    private void PImprintKindShow(string key, string tag)
    {
        PImprintKindName.SetResourceReference(TextBlock.TextProperty, key);
        QChoice.QChoiceMenuApply(PImprintKindList, tag);
    }

    private void PAuthorUpdate()
    {
        LSplice.LSpliceApply(
            _pAuthorList,
            PAuthorItem.PAuthorItemBuild(_lImprint.LImprintCreditRead(), _lImprint.LImprintBlankAt),
            PAuthorItem.PAuthorItemMatch,
            PAuthorItem.PAuthorItemSync);
        PAuthorNotice.Visibility = QLook.QLookVisibleRead(!_lImprint.LImprintHeld);
    }

    private void PAuthorShelfUpdate()
    {
        bool shown = PAuthorCredit.IsMouseOver || PAuthorCredit.IsKeyboardFocusWithin;
        foreach (object item in PAuthorCredit.Items)
        {
            if (PAuthorCredit.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement container
                && QLook.QLookPartFind<FrameworkElement>(container, "PAuthorShelf") is FrameworkElement shelf)
            {
                shelf.Opacity = shown ? 1 : 0;
                shelf.IsHitTestVisible = shown;
            }
        }
    }

    private void PAuthorFocusDefer()
    {
        QField.QFieldFocusDefer(PAuthorCredit, PAuthorItem.PAuthorItemFind(_pAuthorList));
    }

    private void PAuthorRestore()
    {
        PAuthorItem.PAuthorItemRestore(Keyboard.FocusedElement);
    }

    private void PBylineUpdate()
    {
        PBylineList.ItemsSource = PBylineItem.PBylineItemBuild(_lImprint.LBylineRowsRead(), _lImprint.LBylineWord);
        PByline.PlacementTarget = QField.QFieldSurfaceFind(Keyboard.FocusedElement);
        PBylineFrame.MinWidth = (PByline.PlacementTarget as FrameworkElement)?.ActualWidth ?? 0;
        PByline.IsOpen = _lImprint.LBylineShown;
        PBylineList.SelectedIndex = _lImprint.LBylineIndex;
        PBylineList.ScrollIntoView(PBylineList.SelectedItem);
    }

    private void PImprintTitleHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LImprintTitleSet(PImprintTitle.Text);
    }

    private void PImprintYearHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LImprintYearSet(PImprintYear.Text);
    }

    private void PImprintUrlHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LImprintUrlSet(PImprintUrl.Text);
    }

    private void PImprintNoteHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LImprintNoteSet(PImprintNote.Text);
    }

    private void PImprintKindHandle(object sender, RoutedEventArgs e)
    {
        PImprintKind.IsChecked = false;
        _lImprint.LImprintKindSet(QSender.QSenderTagRead(sender));
    }

    private void PAuthorCreditHandle(object sender, RoutedEventArgs e)
    {
        string? action = (e.OriginalSource as FrameworkElement)?.Name switch
        {
            "PAuthorAddition" => "Add",
            "PAuthorRemoval" => "Remove",
            "PAuthorEarlier" => "Earlier",
            "PAuthorLater" => "Later",
            _ => null,
        };
        _lImprint.LImprintCreditApply(
            action,
            QSender.QSenderSourceRead<PAuthorItem>(e)?.PAuthorItemPosition,
            QSender.QSenderSourceRead<PAuthorItem>(e)?.PAuthorItemId);
    }

    private void PAuthorTextHandle(object sender, TextChangedEventArgs e)
    {
        _lImprint.LBylineWordSet(
            (e.OriginalSource as TextBox)?.Text,
            (e.OriginalSource as TextBox)?.IsKeyboardFocusWithin);
    }

    private void PAuthorKeyHandle(object sender, KeyEventArgs e)
    {
        e.Handled = _lImprint.LImprintKeyApply(
            QSender.QSenderKeyRead(e),
            QSender.QSenderSourceRead<PAuthorItem>(e)?.PAuthorItemPosition,
            QSender.QSenderSourceRead<PAuthorItem>(e)?.PAuthorItemId,
            (e.OriginalSource as TextBox)?.Text,
            PBylineItem.PBylineItemRead(PBylineList.SelectedItem));
    }

    private void PAuthorLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        _lImprint.LBylineHide();
        PAuthorItem.PAuthorItemRestore(e.OriginalSource);
    }

    internal void PBylineHandle(object sender, MouseButtonEventArgs e)
    {
        _lImprint.LBylineSelect(
            QSender.QSenderItemRead<PBylineItem>(sender)?.PBylineItemId,
            QSender.QSenderFocusRead<PAuthorItem>()?.PAuthorItemPosition,
            QSender.QSenderFocusRead<PAuthorItem>()?.PAuthorItemId);
        e.Handled = true;
    }
}
