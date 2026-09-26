using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PCorpus
{
    private string? PExcerptTextRead(LStateValue value)
    {
        return value.LStateValueUncertain
            ? PLocalizationCatalog.PLocalizationTextRead("Display.Unknown")
            : value.LStateValueShown;
    }

    private void PExcerptShow(LExample example)
    {
        PExcerptSentenceShow(example);
        PExcerptLanguage.Text = example.LExampleLanguage;
        PExcerptFlag.Source = LEnsignImage.LEnsignFind(example.LExampleLanguage);
        PExcerptGlossShow(example.LExampleGloss);
        PExcerptCitationShow(example.LExampleSource);
        PExcerptTally.Text = PCorpusTallyRead(_lCorpus.LCorpusAnthology.LAnthologyChosen);
    }

    private void PExcerptSentenceShow(LExample example)
    {
        string? text = PExcerptTextRead(example.LExampleText);

        PExcerptText.PMentionText = text ?? PLocalizationCatalog.PLocalizationTextRead("Example.Unwritten");
        PExcerptText.PMentionLanguage = example.LExampleLanguage;
        PExcerptText.PMentionMention = example.LExampleText.LStateValueSound ? example.LExampleMention : [];
        PExcerptText.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void PExcerptCitationShow(LStateAnchor value)
    {
        string? text = value.LStateAnchorShown is long id ? PCitationNameRead(id) : null;

        PExcerptCitation.Text = text ?? string.Empty;
        PExcerptCitationSection.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PExcerptMentionHandle(object? sender, PMentionArgument e)
    {
        if (_lCorpus.LCorpusAnthology.LAnthologyChosen is not long id)
        {
            return;
        }

        if (!PCorpusLeaveConfirm())
        {
            return;
        }

        LMentionResult result;
        try
        {
            result = _lCorpus.LCorpusAnthology.LAnthologyMentionFind(id, e.PMentionArgumentOffset);
        }
        catch (Exception exception)
        {
            _pCorpusHost.PWindowFailureShow("Mention.FindFailed", exception);
            return;
        }

        _pCorpusHost.PWindowMentionHandle(PExcerptText, result);
    }
}
