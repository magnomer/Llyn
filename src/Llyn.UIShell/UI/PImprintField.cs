using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PImprint
{
    private static readonly LReferenceKind[] _pImprintKindOrder =
    [
        LReferenceKind.LReferenceKindUnspecified,
        LReferenceKind.LReferenceKindBook,
        LReferenceKind.LReferenceKindJournal,
        LReferenceKind.LReferenceKindArticle,
        LReferenceKind.LReferenceKindWeb,
        LReferenceKind.LReferenceKindVideo,
        LReferenceKind.LReferenceKindAudio,
        LReferenceKind.LReferenceKindPicture,
        LReferenceKind.LReferenceKindOther,
        LReferenceKind.LReferenceKindUnknown,
    ];

    private bool _pImprintTitleUnreadable;

    private bool _pImprintYearUnreadable;

    private bool _pImprintNoteUnreadable;

    private bool _pImprintUrlUnreadable;

    private bool _pImprintLoading;

    private void PImprintTitleHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintTitleUnreadable, PImprintTitle, PImprintTitleUnknown);
    }

    private void PImprintNoteHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintNoteUnreadable, PImprintNote, PImprintNoteUnknown);
    }

    private void PImprintKindHandle(object sender, SelectionChangedEventArgs e)
    {
        if (_pImprintLoading)
        {
            return;
        }

        PImprintChangeDefer();
    }

    private void PImprintYearHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintYearUnreadable, PImprintYear, PImprintYearUnknown);
    }

    private void PImprintUrlHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintUrlUnreadable, PImprintUrl, PImprintUrlUnknown);
    }

    private void PImprintMarkClear(ref bool unreadable, TextBox field, ToggleButton mark)
    {
        if (_pImprintLoading)
        {
            return;
        }

        unreadable = false;
        field.Tag = string.Empty;
        mark.IsChecked = false;
        PImprintChangeDefer();
    }

    private void PImprintUnknownHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton { Tag: string field } mark)
        {
            return;
        }

        bool unreadable = mark.IsChecked == true;
        switch (field)
        {
            case "Title":
                PImprintMarkShow(unreadable, PImprintTitle, ref _pImprintTitleUnreadable);
                return;
            case "Note":
                PImprintMarkShow(unreadable, PImprintNote, ref _pImprintNoteUnreadable);
                return;
            case "Year":
                PImprintMarkShow(unreadable, PImprintYear, ref _pImprintYearUnreadable);
                return;
            default:
                PImprintMarkShow(unreadable, PImprintUrl, ref _pImprintUrlUnreadable);
                return;
        }
    }

    private void PImprintMarkShow(bool unreadable, TextBox field, ref bool held)
    {
        _pImprintLoading = true;

        held = unreadable;
        field.Text = string.Empty;
        field.Tag = unreadable ? _pImprintHost.PLocalizationTextRead("Display.Unreadable") : string.Empty;

        _pImprintLoading = false;

        PImprintChangeDefer();
    }

    private void PImprintApply(LDraft? draft)
    {
        LReference? reference = draft?.LDraftReference;
        _pImprintLoading = true;

        string unreadable = _pImprintHost.PLocalizationTextRead("Display.Unreadable");

        PImprintFieldShow(
            PImprintTitle, PImprintTitleUnknown, reference?.LReferenceTitle, unreadable,
            ref _pImprintTitleUnreadable);
        PImprintFieldShow(
            PImprintYear, PImprintYearUnknown, reference?.LReferenceYear, unreadable,
            ref _pImprintYearUnreadable);
        PImprintFieldShow(
            PImprintNote, PImprintNoteUnknown, reference?.LReferenceNote, unreadable,
            ref _pImprintNoteUnreadable);
        PImprintFieldShow(
            PImprintUrl, PImprintUrlUnknown, reference?.LReferenceUrl, unreadable,
            ref _pImprintUrlUnreadable);
        PImprintKindShow(reference?.LReferenceKind ?? LReferenceKind.LReferenceKindUnspecified);

        PAuthorShow(draft);

        long? stored = PImprintReferenceRead();
        PImprintRemoval.IsEnabled = stored is not null;
        PImprintCountShow(stored);

        _pImprintLoading = false;

        PImprintChangeUpdate();
    }

    private void PImprintShow(LDraft draft)
    {
        if (draft.LDraftReference is not LReference reference)
        {
            return;
        }

        _pImprintLoading = true;

        string unreadable = _pImprintHost.PLocalizationTextRead("Display.Unreadable");

        PImprintFieldShow(
            PImprintTitle, PImprintTitleUnknown, reference.LReferenceTitle, unreadable, ref _pImprintTitleUnreadable, true);
        PImprintFieldShow(
            PImprintYear, PImprintYearUnknown, reference.LReferenceYear, unreadable, ref _pImprintYearUnreadable, true);
        PImprintFieldShow(
            PImprintNote, PImprintNoteUnknown, reference.LReferenceNote, unreadable, ref _pImprintNoteUnreadable, true);
        PImprintFieldShow(
            PImprintUrl, PImprintUrlUnknown, reference.LReferenceUrl, unreadable, ref _pImprintUrlUnreadable, true);

        if (PImprintKindRead() != reference.LReferenceKind)
        {
            PImprintKindShow(reference.LReferenceKind);
        }

        PAuthorShow(draft);

        _pImprintLoading = false;

        PImprintChangeUpdate();
    }

    private void PImprintKindShow(LReferenceKind kind)
    {
        if (PImprintKind.Items.Count == 0)
        {
            PImprintKind.SelectedValuePath = "Tag";
            foreach (LReferenceKind offered in _pImprintKindOrder)
            {
                PImprintKind.Items.Add(new ComboBoxItem
                {
                    Content = _pImprintHost.PLocalizationTextRead(PReference.PReferenceKindRead(offered)),
                    Tag = offered,
                });
            }
        }

        PImprintKind.SelectedValue = kind;
    }

    private LReferenceKind PImprintKindRead()
    {
        return PImprintKind.SelectedValue is LReferenceKind kind
            ? kind
            : LReferenceKind.LReferenceKindUnspecified;
    }

    private static void PImprintFieldShow(
        TextBox field, ToggleButton mark, LStateValue? value, string unreadable, ref bool held, bool differing = false)
    {
        if (differing && LStateValue.LStateValueResolve(field.Text, held) == (value ?? LStateValue.LStateValueUnspecified))
        {
            return;
        }

        field.Text = value?.LStateValueShow() ?? string.Empty;
        held = value?.LStateValueState == LState.LStateUnknown;
        field.Tag = held ? unreadable : string.Empty;
        mark.IsChecked = held;
    }

    private void PImprintCountShow(long? stored)
    {
        if (stored is null)
        {
            PImprintCount.Text = string.Empty;
            return;
        }

        int usage = _pImprintOwner.PShelfCountRead(stored.Value);
        PImprintCount.Text = $"{_pImprintHost.PLocalizationTextRead("Source.DetachCount")} "
            + usage.ToString(CultureInfo.CurrentCulture);
    }

    private LReference PImprintRead()
    {
        return new LReference(
            0,
            LStateValue.LStateValueResolve(PImprintTitle.Text, _pImprintTitleUnreadable),
            LStateValue.LStateValueResolve(PImprintYear.Text, _pImprintYearUnreadable),
            PImprintKindRead(),
            LStateValue.LStateValueResolve(PImprintNote.Text, _pImprintNoteUnreadable),
            LStateValue.LStateValueResolve(PImprintUrl.Text, _pImprintUrlUnreadable),
            _pAuthorState);
    }

    private void PImprintDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!_pImprintOwner.PReferenceLeaveConfirm())
        {
            return;
        }

        PImprintDraftOpen(PImprintReferenceRead());
    }

    private void PImprintStoreHandle(object sender, RoutedEventArgs e)
    {
        PImprintChangeSave();

        long held = _pImprintDraft;
        if (held == 0)
        {
            return;
        }

        LReference stored;
        try
        {
            stored = _lEngine.LEngineReferenceCommit(held);
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.SaveFailed", exception);
            return;
        }

        _pImprintDraft = 0;

        _pImprintOwner.PReferenceScribeShow(false);
        _pImprintOwner.PReferenceShow(stored.LReferenceId);
    }

    private void PImprintRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (PImprintReferenceRead() is not long id)
        {
            return;
        }

        int usage = _pImprintOwner.PShelfCountRead(id);

        if (!_pImprintHost.PWindowRemovalConfirm(usage, "Source"))
        {
            return;
        }

        try
        {
            _lEngine.LEngineReferenceDelete(id, usage > 0);
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.DeleteFailed", exception);
            return;
        }

        _pImprintOwner.PReferenceScribeShow(false);
        _pImprintOwner.PReferenceClear();
    }
}
