using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PReference
{
    private bool _pImprintTitleUnreadable;

    private bool _pImprintProgramUnreadable;

    private bool _pImprintChannelUnreadable;

    private bool _pImprintYearUnreadable;

    private bool _pImprintUrlUnreadable;

    private bool _pImprintLoading;

    private void PImprintTitleHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintTitleUnreadable, PImprintTitle, PImprintTitleUnknown);
    }

    private void PImprintProgramHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintProgramUnreadable, PImprintProgram, PImprintProgramUnknown);
    }

    private void PImprintChannelHandle(object sender, TextChangedEventArgs e)
    {
        PImprintMarkClear(ref _pImprintChannelUnreadable, PImprintChannel, PImprintChannelUnknown);
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
            case "Program":
                PImprintMarkShow(unreadable, PImprintProgram, ref _pImprintProgramUnreadable);
                return;
            case "Channel":
                PImprintMarkShow(unreadable, PImprintChannel, ref _pImprintChannelUnreadable);
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
        field.Tag = unreadable ? _pReferenceHost.PLocalizationTextRead("Display.Unreadable") : string.Empty;

        _pImprintLoading = false;

        PImprintChangeDefer();
    }

    private void PImprintApply(LReference? reference)
    {
        _pImprintLoading = true;

        string unreadable = _pReferenceHost.PLocalizationTextRead("Display.Unreadable");

        PImprintFieldShow(
            PImprintTitle, PImprintTitleUnknown, reference?.LReferenceTitle, unreadable,
            ref _pImprintTitleUnreadable);
        PImprintFieldShow(
            PImprintProgram, PImprintProgramUnknown, reference?.LReferenceProgram, unreadable,
            ref _pImprintProgramUnreadable);
        PImprintFieldShow(
            PImprintChannel, PImprintChannelUnknown, reference?.LReferenceChannel, unreadable,
            ref _pImprintChannelUnreadable);
        PImprintFieldShow(
            PImprintYear, PImprintYearUnknown, reference?.LReferenceYear, unreadable,
            ref _pImprintYearUnreadable);
        PImprintFieldShow(
            PImprintUrl, PImprintUrlUnknown, reference?.LReferenceUrl, unreadable,
            ref _pImprintUrlUnreadable);

        PAuthorApply(reference);

        string? stored = PImprintReferenceRead();
        PImprintRemoval.IsEnabled = stored is not null;
        PImprintCountShow(stored);

        _pImprintLoading = false;

        PImprintChangeUpdate();
    }

    private static void PImprintFieldShow(
        TextBox field, ToggleButton mark, LStateValue? value, string unreadable, ref bool held)
    {
        field.Text = value?.LStateValueShow() ?? string.Empty;
        held = value?.LStateValueState == LState.LStateUnknown;
        field.Tag = held ? unreadable : string.Empty;
        mark.IsChecked = held;
    }

    private void PImprintCountShow(string? stored)
    {
        if (stored is null)
        {
            PImprintCount.Text = string.Empty;
            return;
        }

        int usage = PShelfCountRead(stored);
        PImprintCount.Text = $"{_pReferenceHost.PLocalizationTextRead("Source.DetachCount")} "
            + usage.ToString(CultureInfo.CurrentCulture);
    }

    private LReference PImprintRead(LReference held)
    {
        return held with
        {
            LReferenceTitle =
                LStateValue.LStateValueResolve(PImprintTitle.Text, _pImprintTitleUnreadable),
            LReferenceProgram =
                LStateValue.LStateValueResolve(PImprintProgram.Text, _pImprintProgramUnreadable),
            LReferenceChannel =
                LStateValue.LStateValueResolve(PImprintChannel.Text, _pImprintChannelUnreadable),
            LReferenceYear =
                LStateValue.LStateValueResolve(PImprintYear.Text, _pImprintYearUnreadable),
            LReferenceUrl =
                LStateValue.LStateValueResolve(PImprintUrl.Text, _pImprintUrlUnreadable),
            LReferenceAuthorState = _pAuthorState,
        };
    }

    private void PReferenceFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PReferenceClear();
        PReferenceScribeShow(true);
        PImprintDraftShow(PImprintDraftStart(null));
        PReferenceScribe.IsEnabled = true;
    }

    private void PImprintDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PImprintDraftShow(PImprintDraftStart(PImprintReferenceRead()));
    }

    private void PImprintStoreHandle(object sender, RoutedEventArgs e)
    {
        PImprintChangeSave();

        string held = _pImprintDraft;
        if (held.Length == 0)
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
            _pReferenceHost.PWindowFailureShow("Source.SaveFailed", exception);
            return;
        }

        _pImprintDraft = string.Empty;
        _pColophonReference = stored.LReferenceId;

        PReferenceScribeShow(false);
        PReferenceShow(stored.LReferenceId);
    }

    private void PImprintRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (PImprintReferenceRead() is not string id)
        {
            return;
        }

        int usage = PShelfCountRead(id);

        if (!_pReferenceHost.PWindowRemovalConfirm(usage, "Source"))
        {
            return;
        }

        try
        {
            _lEngine.LEngineReferenceDelete(id, usage > 0);
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.DeleteFailed", exception);
            return;
        }

        PReferenceScribeShow(false);
        PReferenceClear();
    }
}
