using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PReference
{
    private LReference? _pImprintReference;

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

        PImprintRemoval.IsEnabled = reference is not null;
        PImprintCountShow(reference);

        _pImprintLoading = false;
    }

    private static void PImprintFieldShow(
        TextBox field, ToggleButton mark, LStateValue? value, string unreadable, ref bool held)
    {
        field.Text = value?.LStateValueShow() ?? string.Empty;
        held = value?.LStateValueState == LState.LStateUnknown;
        field.Tag = held ? unreadable : string.Empty;
        mark.IsChecked = held;
    }

    private void PImprintCountShow(LReference? reference)
    {
        if (reference is null)
        {
            PImprintCount.Text = string.Empty;
            return;
        }

        int usage = PShelfCountRead(reference.LReferenceId);
        PImprintCount.Text = $"{_pReferenceHost.PLocalizationTextRead("Source.DetachCount")} "
            + usage.ToString(CultureInfo.CurrentCulture);
    }

    private LReference PImprintRead()
    {
        return new LReference(
            _pImprintReference?.LReferenceId ?? string.Empty,
            PImprintValueRead(PImprintTitle.Text, _pImprintTitleUnreadable),
            PImprintValueRead(PImprintProgram.Text, _pImprintProgramUnreadable),
            PImprintValueRead(PImprintChannel.Text, _pImprintChannelUnreadable),
            PImprintValueRead(PImprintYear.Text, _pImprintYearUnreadable),
            PImprintValueRead(PImprintUrl.Text, _pImprintUrlUnreadable),
            _pAuthorState);
    }

    private static LStateValue PImprintValueRead(string text, bool unreadable)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            return LStateValue.LStateValueCreate(text);
        }

        return unreadable ? LStateValue.LStateValueUnknown : LStateValue.LStateValueUnspecified;
    }

    private bool PImprintChangeCheck()
    {
        LReference written = PImprintRead();

        if (_pImprintReference is null)
        {
            return !written.LReferenceTitle.LStateValueEmpty
                || !written.LReferenceProgram.LStateValueEmpty
                || !written.LReferenceChannel.LStateValueEmpty
                || !written.LReferenceYear.LStateValueEmpty
                || !written.LReferenceUrl.LStateValueEmpty;
        }

        return written.LReferenceTitle != _pImprintReference.LReferenceTitle
            || written.LReferenceProgram != _pImprintReference.LReferenceProgram
            || written.LReferenceChannel != _pImprintReference.LReferenceChannel
            || written.LReferenceYear != _pImprintReference.LReferenceYear
            || written.LReferenceUrl != _pImprintReference.LReferenceUrl
            || written.LReferenceAuthorState != _pImprintReference.LReferenceAuthorState;
    }

    private void PReferenceFreshHandle(object sender, RoutedEventArgs e)
    {
        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PReferenceClear();
        _pImprintReference = null;
        PImprintApply(null);
        PReferenceScribe.IsEnabled = true;
        PReferenceScribeShow(true);
    }

    private void PImprintDiscardHandle(object sender, RoutedEventArgs e)
    {
        if (!PReferenceLeaveConfirm())
        {
            return;
        }

        PImprintApply(_pImprintReference);
    }

    private void PImprintStoreHandle(object sender, RoutedEventArgs e)
    {
        LReference written = PImprintRead();

        try
        {
            if (_pImprintReference is null)
            {
                written = _lEngine.LEngineReferenceCreate(written);
            }
            else
            {
                _lEngine.LEngineReferenceUpdate(written);
            }
        }
        catch (Exception exception)
        {
            _pReferenceHost.PWindowFailureShow("Source.SaveFailed", exception);
            return;
        }

        _pImprintReference = written;
        _pColophonReference = written.LReferenceId;

        PShelfFind(PSurvey.Text ?? string.Empty);
        PReferenceScribeShow(false);
        PReferenceShow(written.LReferenceId);
    }

    private void PImprintRemovalHandle(object sender, RoutedEventArgs e)
    {
        if (_pImprintReference is null)
        {
            return;
        }

        string id = _pImprintReference.LReferenceId;
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
        PShelfFind(PSurvey.Text ?? string.Empty);
    }
}
