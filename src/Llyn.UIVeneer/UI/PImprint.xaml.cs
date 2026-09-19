using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PImprint : UserControl, PChronicleHost
{
    private readonly ObservableCollection<PAuthorItem> _pAuthorList = [];

    private LImprint _lImprint = null!;

    public PImprint()
    {
        InitializeComponent();

        Resources.MergedDictionaries.Add(new PBylineTemplate(this));

        PAuthorCredit.ItemsSource = _pAuthorList;
        PByline.CustomPopupPlacementCallback = PField.PFieldPopupPlace;
        PChoice.PChoiceMenuBuild(PImprintKindList, PImprintKindHandle);
    }

    internal void PImprintAttach(PWindow host, LImprint imprint)
    {
        _lImprint = imprint;
        _lImprint.LImprintChanged += PAuthorUpdate;
        _lImprint.LImprintFocused += PAuthorFocusDefer;
        _lImprint.LImprintReverted += PAuthorRestore;
        _lImprint.LBylineChanged += PBylineUpdate;
        _lImprint.LImprintDesk.LDeskStarted += PImprintStartUpdate;
        _lImprint.LImprintDesk.LDeskDraftChanged += PImprintDraftUpdate;
        _lImprint.LImprintDesk.LDeskFailed += host.PWindowFailureShow;
    }

    internal void PImprintClear()
    {
        PImprintTitle.Text = string.Empty;
        PImprintTitle.SetResourceReference(TagProperty, "Source.Untitled");
        PImprintYear.Text = string.Empty;
        PImprintYear.SetResourceReference(TagProperty, "Source.Year");
        PImprintUrl.Text = string.Empty;
        PImprintUrl.SetResourceReference(TagProperty, "Source.Url");
        PImprintNote.Text = string.Empty;
        PImprintNote.SetResourceReference(TagProperty, "Source.Note");
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

    private void PImprintStartUpdate(LTenure held)
    {
        held.LTenureDraftAttach(LSubject.LSubjectDraft, new PObserver(this, _lImprint.LImprintDesk.LDeskDraftUpdate));
        held.LTenureDraftAttach(LSubject.LSubjectTenure, new PObserver(this, _lImprint.LImprintDesk.LDeskStateUpdate));
    }

    private void PImprintDraftUpdate(LDraft draft)
    {
        LReference reference = _lImprint.LImprintReferenceRead(draft);
        PImprintTitle.Text = reference.LReferenceTitle.LStateValueShow();
        PImprintTitle.SetResourceReference(TagProperty, reference.LReferenceTitleHint);
        PImprintYear.Text = reference.LReferenceYear.LStateValueShow();
        PImprintYear.SetResourceReference(TagProperty, reference.LReferenceYearHint);
        PImprintUrl.Text = reference.LReferenceUrl.LStateValueShow();
        PImprintUrl.SetResourceReference(TagProperty, reference.LReferenceUrlHint);
        PImprintNote.Text = reference.LReferenceNote.LStateValueShow();
        PImprintNote.SetResourceReference(TagProperty, reference.LReferenceNoteHint);
        PImprintKindShow(reference.LReferenceKindKey, reference.LReferenceKindTag);
        PImprintTallyShow();
    }

    private void PImprintKindShow(string key, string tag)
    {
        PImprintKindName.SetResourceReference(TextBlock.TextProperty, key);
        PChoice.PChoiceMenuApply(PImprintKindList, tag);
    }

    private void PAuthorUpdate()
    {
        PSplice.PSpliceApply(
            _pAuthorList,
            PAuthorItem.PAuthorItemBuild(_lImprint.LImprintCreditRead(), _lImprint.LImprintBlankAt),
            PAuthorItem.PAuthorItemMatch,
            PAuthorItem.PAuthorItemSync);
        PAuthorNotice.Visibility = PLook.PLookVisibleRead(!_lImprint.LImprintHeld);
    }

    private void PAuthorFocusDefer()
    {
        PField.PFieldFocusDefer(PAuthorCredit, PAuthorItem.PAuthorItemFind(_pAuthorList));
    }

    private void PAuthorRestore()
    {
        PAuthorItem.PAuthorItemRestore(Keyboard.FocusedElement);
    }

    private void PBylineUpdate()
    {
        PBylineList.ItemsSource = PBylineItem.PBylineItemBuild(_lImprint.LBylineRowsRead(), _lImprint.LBylineWord);
        PByline.PlacementTarget = PField.PFieldSurfaceFind(Keyboard.FocusedElement);
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
        _lImprint.LImprintKindSet(PSender.PSenderTagRead(sender));
    }

    private void PAuthorCreditHandle(object sender, RoutedEventArgs e)
    {
        _lImprint.LImprintCreditApply(
            PSender.PSenderTagRead(e.OriginalSource),
            PSender.PSenderSourceRead<PAuthorItem>(e)?.PAuthorItemPosition,
            PSender.PSenderSourceRead<PAuthorItem>(e)?.PAuthorItemId);
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
            PSender.PSenderKeyRead(e),
            PSender.PSenderSourceRead<PAuthorItem>(e)?.PAuthorItemPosition,
            PSender.PSenderSourceRead<PAuthorItem>(e)?.PAuthorItemId,
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
            PSender.PSenderItemRead<PBylineItem>(sender)?.PBylineItemId,
            PSender.PSenderFocusRead<PAuthorItem>()?.PAuthorItemPosition,
            PSender.PSenderFocusRead<PAuthorItem>()?.PAuthorItemId);
        e.Handled = true;
    }
}
