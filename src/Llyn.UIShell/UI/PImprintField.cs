using System;
using System.Windows;
using System.Windows.Controls;
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

    private LReferenceKind _pImprintKind;

    private bool _pImprintLoading;

    private void PImprintTitleHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintTitleUnknown, PImprintTitle);
    }

    private void PImprintNoteHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintNoteUnknown, PImprintNote);
    }

    private void PImprintKindHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: LReferenceKind kind })
        {
            return;
        }

        PImprintKind.IsChecked = false;
        if (_pImprintLoading || kind == _pImprintKind)
        {
            return;
        }

        PImprintKindShow(kind);
        PImprintChangeDefer();
    }

    private void PImprintYearHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintYearUnknown, PImprintYear);
    }

    private void PImprintUrlHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintUrlUnknown, PImprintUrl);
    }

    private void PImprintMarkClear(ref bool unknown, TextBox field)
    {
        if (_pImprintLoading)
        {
            return;
        }

        unknown = false;
        field.Tag = PImprintHintRead(field, false);
        PImprintChangeDefer();
    }

    private string PImprintHintRead(TextBox field, bool unknown)
    {
        string key = unknown
            ? "Display.Unknown"
            : ReferenceEquals(field, PImprintTitle)
                ? "Source.Untitled"
                : ReferenceEquals(field, PImprintYear)
                    ? "Source.Year"
                    : ReferenceEquals(field, PImprintUrl)
                        ? "Source.Url"
                        : "Source.Note";

        return _pImprintHost.PLocalizationTextRead(key);
    }

    private void PImprintApply(LDraft? draft)
    {
        LReference? reference = draft?.LDraftReference;
        _pImprintLoading = true;

        PImprintFieldShow(PImprintTitle, reference?.LReferenceTitle, ref _pImprintTitleUnknown);
        PImprintFieldShow(PImprintYear, reference?.LReferenceYear, ref _pImprintYearUnknown);
        PImprintFieldShow(PImprintNote, reference?.LReferenceNote, ref _pImprintNoteUnknown);
        PImprintFieldShow(PImprintUrl, reference?.LReferenceUrl, ref _pImprintUrlUnknown);
        PImprintKindShow(reference?.LReferenceKind ?? LReferenceKind.LReferenceKindUnspecified);

        PAuthorOpen(draft);
        PImprintTallyShow();

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

        PImprintFieldShow(PImprintTitle, reference.LReferenceTitle, ref _pImprintTitleUnknown, true);
        PImprintFieldShow(PImprintYear, reference.LReferenceYear, ref _pImprintYearUnknown, true);
        PImprintFieldShow(PImprintNote, reference.LReferenceNote, ref _pImprintNoteUnknown, true);
        PImprintFieldShow(PImprintUrl, reference.LReferenceUrl, ref _pImprintUrlUnknown, true);

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
        if (PImprintKindList.Children.Count == 0)
        {
            foreach (LReferenceKind offered in _pImprintKindOrder)
            {
                RadioButton choice = new()
                {
                    GroupName = nameof(PImprintKind),
                    Tag = offered,
                };
                choice.SetResourceReference(FrameworkElement.StyleProperty, "Theme.Choice.Order");
                choice.SetResourceReference(ContentControl.ContentProperty, PReference.PReferenceKindRead(offered));
                choice.Click += PImprintKindHandle;
                PImprintKindList.Children.Add(choice);
            }
        }

        _pImprintKind = kind;
        PImprintKindName.SetResourceReference(TextBlock.TextProperty, PReference.PReferenceKindRead(kind));

        foreach (RadioButton choice in PImprintKindList.Children)
        {
            choice.IsChecked = choice.Tag is LReferenceKind offered && offered == kind;
        }
    }

    private LReferenceKind PImprintKindRead()
    {
        return _pImprintKind;
    }

    private void PImprintFieldShow(TextBox field, LStateValue? value, ref bool held, bool differing = false)
    {
        if (differing && new LStateWritten(field.Text, held).LStateWrittenMatch(value))
        {
            return;
        }

        field.Text = value?.LStateValueShow() ?? string.Empty;
        held = value?.LStateValueState == LState.LStateUnknown;
        field.Tag = PImprintHintRead(field, held);
    }

    internal void PImprintTallyShow()
    {
        PImprintTally.Text = _pImprintOwner.PReferenceTallyRead(PImprintReferenceRead());
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

    internal void PImprintStoreRun()
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
}
