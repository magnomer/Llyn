using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private static string? QExcerptTextRead(CStateValue value)
    {
        return value.CStateValueUncertain
            ? QLocalizationCatalog.QLocalizationTextRead("Display.Unknown")
            : value.CStateValueShown;
    }

    private void QExcerptShow(CExample example)
    {
        QExcerptSentenceShow(example);
        QExcerptLanguage.Text = example.CExampleLanguage;
        QExcerptFlag.Source = LEnsignImage.LEnsignFind(example.CExampleLanguage);
        QExcerptGlossShow(example.CExampleGloss);
        QExcerptCitationShow(example.CExampleSource);
        QExcerptTally.Text = QCorpusTallyRead(_lCorpus.LCorpusAnthology.LAnthologyChosen);
    }

    private void QExcerptSentenceShow(CExample example)
    {
        string? text = QExcerptTextRead(example.CExampleText);

        QExcerptText.PMentionText = text ?? QLocalizationCatalog.QLocalizationTextRead("Example.Unwritten");
        QExcerptText.PMentionLanguage = example.CExampleLanguage;
        QExcerptText.PMentionMarkShow(QExcerptMarkRead(example.CExampleExcerpt));
        QExcerptText.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private static List<PMentionMark> QExcerptMarkRead(IReadOnlyList<CMention> mentions)
    {
        List<PMentionMark> marks = new(mentions.Count);
        foreach (CMention mention in mentions)
        {
            marks.Add(new PMentionMark(
                mention.CMentionId,
                mention.CMentionOffset,
                mention.CMentionLength,
                mention.CMentionEntry,
                mention.CMentionSense));
        }

        return marks;
    }

    private void QExcerptCitationShow(long? value)
    {
        string? text = value is long id ? QCitationNameRead(id) : null;

        QExcerptCitation.Text = text ?? string.Empty;
        QExcerptCitationSection.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void QExcerptMentionHandle(object? sender, PMentionArgument e)
    {
        if (!_lCorpus.LCorpusLeaveConfirm())
        {
            return;
        }

        try
        {
            QExcerptMentionShow(_lCorpus.LCorpusAnthology.LAnthologyMentionFind(
                _lCorpus.LCorpusAnthology.LAnthologyChosen, e.PMentionArgumentOffset));
        }
        catch (Exception exception)
        {
            _qCorpusHost.PWindowFailureShow("Mention.FindFailed", exception);
        }
    }

    private void QExcerptMentionShow(CMentionResult? result)
    {
        if (result is null)
        {
            return;
        }

        _qCorpusHost.PWindowMentionHandle(QExcerptText, result);
    }
}
