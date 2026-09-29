using System;
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
        QExcerptCitationShow(example);
        QExcerptTally.Text = QCorpusTallyRead(_cCorpus.CCorpusAnthology.CAnthologyChosen);
    }

    private void QExcerptSentenceShow(CExample example)
    {
        string? text = QExcerptTextRead(example.CExampleText);

        QExcerptText.PMentionText = text ?? QLocalizationCatalog.QLocalizationTextRead("Example.Unwritten");
        QExcerptText.PMentionLanguage = example.CExampleLanguage;
        QExcerptText.PMentionMention = example.CExampleExcerpt;
        QExcerptText.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void QExcerptCitationShow(CExample example)
    {
        QExcerptCitation.Text = example.CExampleCitation;
        QExcerptCitationSection.Visibility = example.CExampleSource is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void QExcerptMentionHandle(object? sender, PMentionArgument e)
    {
        if (!_cCorpus.CCorpusLeaveConfirm())
        {
            return;
        }

        try
        {
            QExcerptMentionShow(_cCorpus.CCorpusAnthology.CAnthologyMentionRead(e.PMentionArgumentOffset));
        }
        catch (Exception exception)
        {
            _qCorpusHost.PWindowFailureRefine("Mention.FindFailed", exception);
        }
    }

    private void QExcerptMentionShow(CMentionResult? result)
    {
        if (result is null)
        {
            return;
        }

        _qCorpusHost.PWindowMentionObserve(QExcerptText, result);
    }
}
