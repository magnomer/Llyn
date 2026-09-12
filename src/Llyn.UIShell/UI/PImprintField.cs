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

    private bool _pImprintTitleUnknown;

    private bool _pImprintYearUnknown;

    private bool _pImprintNoteUnknown;

    private bool _pImprintUrlUnknown;

    private bool _pImprintLoading;

    private void PImprintTitleHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintTitleUnknown, PImprintTitle, PImprintTitleUnknown);
    }

    private void PImprintNoteHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintNoteUnknown, PImprintNote, PImprintNoteUnknown);
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
        PImprintMarkClear(ref _pImprintYearUnknown, PImprintYear, PImprintYearUnknown);
    }

    private void PImprintUrlHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintUrlUnknown, PImprintUrl, PImprintUrlUnknown);
    }

    private void PImprintMarkClear(ref bool unknown, TextBox field, ToggleButton mark)
    {
        if (_pImprintLoading)
        {
            return;
        }

        unknown = false;
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

        bool unknown = mark.IsChecked == true;
        switch (field)
        {
            case "Title":
                PImprintMarkShow(unknown, PImprintTitle, ref _pImprintTitleUnknown);
                return;
            case "Note":
                PImprintMarkShow(unknown, PImprintNote, ref _pImprintNoteUnknown);
                return;
            case "Year":
                PImprintMarkShow(unknown, PImprintYear, ref _pImprintYearUnknown);
                return;
            default:
                PImprintMarkShow(unknown, PImprintUrl, ref _pImprintUrlUnknown);
                return;
        }
    }

    private void PImprintMarkShow(bool unknown, TextBox field, ref bool held)
    {
        _pImprintLoading = true;

        held = unknown;
        field.Text = string.Empty;
        field.Tag = unknown ? _pImprintHost.PLocalizationTextRead("Display.Unknown") : string.Empty;

        _pImprintLoading = false;

        PImprintChangeDefer();
    }

    private void PImprintApply(LDraft? draft)
    {
        LReference? reference = draft?.LDraftReference;
        _pImprintLoading = true;

        string unknown = _pImprintHost.PLocalizationTextRead("Display.Unknown");

        PImprintFieldShow(
            PImprintTitle, PImprintTitleUnknown, reference?.LReferenceTitle, unknown,
            ref _pImprintTitleUnknown);
        PImprintFieldShow(
            PImprintYear, PImprintYearUnknown, reference?.LReferenceYear, unknown,
            ref _pImprintYearUnknown);
        PImprintFieldShow(
            PImprintNote, PImprintNoteUnknown, reference?.LReferenceNote, unknown,
            ref _pImprintNoteUnknown);
        PImprintFieldShow(
            PImprintUrl, PImprintUrlUnknown, reference?.LReferenceUrl, unknown,
            ref _pImprintUrlUnknown);
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

        string unknown = _pImprintHost.PLocalizationTextRead("Display.Unknown");

        PImprintFieldShow(
            PImprintTitle, PImprintTitleUnknown, reference.LReferenceTitle, unknown, ref _pImprintTitleUnknown, true);
        PImprintFieldShow(
            PImprintYear, PImprintYearUnknown, reference.LReferenceYear, unknown, ref _pImprintYearUnknown, true);
        PImprintFieldShow(
            PImprintNote, PImprintNoteUnknown, reference.LReferenceNote, unknown, ref _pImprintNoteUnknown, true);
        PImprintFieldShow(
            PImprintUrl, PImprintUrlUnknown, reference.LReferenceUrl, unknown, ref _pImprintUrlUnknown, true);

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
        TextBox field, ToggleButton mark, LStateValue? value, string unknown, ref bool held, bool differing = false)
    {
        if (differing && new LStateWritten(field.Text, held).LStateWrittenMatch(value))
        {
            return;
        }

        field.Text = value?.LStateValueShow() ?? string.Empty;
        held = value?.LStateValueState == LState.LStateUnknown;
        field.Tag = held ? unknown : string.Empty;
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

    private LRequestReferenceBody PImprintRead(long draft)
    {
        return new LRequestReferenceBody(
            draft,
            new LStateWritten(PImprintTitle.Text, _pImprintTitleUnknown),
            new LStateWritten(PImprintYear.Text, _pImprintYearUnknown),
            PImprintKindRead(),
            new LStateWritten(PImprintNote.Text, _pImprintNoteUnknown),
            new LStateWritten(PImprintUrl.Text, _pImprintUrlUnknown),
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
            stored = _pImprintHost.PWindowCommitRun(held, _lEngine.LEngineReferenceCommit);
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
