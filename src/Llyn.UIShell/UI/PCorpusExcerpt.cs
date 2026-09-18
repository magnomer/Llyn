using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PCorpus
{
    private string? PExcerptTextRead(LStateValue value)
    {
        return PStateConverter.PStateConverterCheck(value)
            ? _pCorpusHost.PLocalizationTextRead("Display.Unknown")
            : value.LStateValueShow() is { Length: > 0 } shown ? shown : null;
    }

    private void PExcerptSentenceShow(LExample example)
    {
        string? text = PExcerptTextRead(example.LExampleText);

        PExcerptText.PMentionText = text ?? _pCorpusHost.PLocalizationTextRead("Example.Unwritten");
        PExcerptText.PMentionLanguage = example.LExampleLanguage;
        PExcerptText.PMentionMention =
            text is null
            || example.LExampleText.LStateValueUnreadable
            || PStateConverter.PStateConverterCheck(example.LExampleText)
                ? []
                : example.LExampleMention;
        PExcerptText.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void PExcerptCitationShow(LStateAnchor value)
    {
        string shown = PCitationNameRead(value.LStateAnchorShow());
        string? text = value.LStateAnchorShow() != 0 && !string.IsNullOrEmpty(shown) ? shown : null;

        PExcerptCitation.Text = text ?? string.Empty;
        PExcerptCitationSection.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PExcerptMentionHandle(object? sender, PMentionArgument e)
    {
        if (_pCorpusVista?.LVistaChosen is not long id || !PCorpusLeaveConfirm())
        {
            return;
        }

        LMentionResult result;
        try
        {
            result = _lEngine.LEngineMentionFind(id, e.PMentionArgumentOffset);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Mention.FindFailed", exception);
            return;
        }

        _pCorpusHost.PWindowMentionHandle(PExcerptText, result);
    }
}
