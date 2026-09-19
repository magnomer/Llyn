using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

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

    private LStateValue _pImprintTitle = LStateValue.LStateValueUnspecified;

    private LStateValue _pImprintYear = LStateValue.LStateValueUnspecified;

    private LStateValue _pImprintNote = LStateValue.LStateValueUnspecified;

    private LStateValue _pImprintUrl = LStateValue.LStateValueUnspecified;

    private LReferenceKind _pImprintKind;

    private bool _pImprintLoading;

    private void PImprintTitleHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintTitle, PImprintTitle);
    }

    private void PImprintNoteHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintNote, PImprintNote);
    }

    private void PImprintKindHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement chosen)
        {
            return;
        }

        if (chosen.Tag is not LReferenceKind kind)
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
        PImprintMarkClear(ref _pImprintYear, PImprintYear);
    }

    private void PImprintUrlHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintUrl, PImprintUrl);
    }

    private void PImprintMarkClear(ref LStateValue held, TextBox field)
    {
        if (_pImprintLoading)
        {
            return;
        }

        held = LStateValue.LStateValueUnspecified;
        field.Tag = PImprintHintRead(field, false);
        PImprintChangeDefer();
    }

    private static LStateWritten PImprintFieldRead(TextBox field, LStateValue held)
    {
        return new LStateWritten(field.Text, held.LStateValueUncertain);
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

        return PLocalizationCatalog.PLocalizationTextRead(key);
    }

    private void PImprintApply(LDraft? draft)
    {
        LReference? reference = draft?.LDraftReference;
        _pImprintLoading = true;

        PImprintFieldShow(PImprintTitle, reference?.LReferenceTitle, ref _pImprintTitle);
        PImprintFieldShow(PImprintYear, reference?.LReferenceYear, ref _pImprintYear);
        PImprintFieldShow(PImprintNote, reference?.LReferenceNote, ref _pImprintNote);
        PImprintFieldShow(PImprintUrl, reference?.LReferenceUrl, ref _pImprintUrl);
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

        PImprintFieldShow(PImprintTitle, reference.LReferenceTitle, ref _pImprintTitle, true);
        PImprintFieldShow(PImprintYear, reference.LReferenceYear, ref _pImprintYear, true);
        PImprintFieldShow(PImprintNote, reference.LReferenceNote, ref _pImprintNote, true);
        PImprintFieldShow(PImprintUrl, reference.LReferenceUrl, ref _pImprintUrl, true);

        if (!reference.LReferenceKindMatch(PImprintKindRead()))
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

    private void PImprintFieldShow(TextBox field, LStateValue? value, ref LStateValue held, bool differing = false)
    {
        LStateValue shown = value ?? LStateValue.LStateValueUnspecified;
        if (differing && held == shown)
        {
            return;
        }

        held = shown;
        field.Text = shown.LStateValueShow();
        field.Tag = PImprintHintRead(field, shown.LStateValueUncertain);
    }

    internal void PImprintTallyShow()
    {
        PImprintTally.Text = _pImprintOwner.PReferenceTallyRead(PImprintReferenceRead());
    }

    private LRequestReferenceBody PImprintRead(long draft)
    {
        return new LRequestReferenceBody(
            draft,
            PImprintFieldRead(PImprintTitle, _pImprintTitle),
            PImprintFieldRead(PImprintYear, _pImprintYear),
            PImprintKindRead(),
            PImprintFieldRead(PImprintNote, _pImprintNote),
            PImprintFieldRead(PImprintUrl, _pImprintUrl),
            _pAuthorState.LStateMarkState);
    }

    internal void PImprintStoreRun()
    {
        if (_pImprintTenure is not LTenure held)
        {
            return;
        }

        held.LTenurePersist();
        if (!held.LTenureStateRead().LTenureStateChanged)
        {
            return;
        }

        long? stored;
        try
        {
            stored = _pImprintHost.PWindowCommitRun(held, true);
        }
        catch (Exception exception)
        {
            _pImprintHost.PWindowFailureShow("Source.SaveFailed", exception);
            return;
        }

        _pImprintTenure = null;
        if (stored is not long reference)
        {
            return;
        }

        _pImprintOwner.PReferenceScribeShow(false);
        _pImprintOwner.PReferenceShow(reference);
    }
}
