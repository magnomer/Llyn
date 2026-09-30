using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private void QExcerptRefine(CExample example)
    {
        QExcerptSentenceRefine(example);
        QExcerptLanguage.Text = example.CExampleLanguage;
        QExcerptFlag.Source = QEnsignImage.QEnsignRead(example.CExampleLanguage);
        QExcerptGlossShow(example.CExampleGloss);
        QExcerptCitationRefine(example);
        QExcerptTally.Text = example.CExampleTally;
    }

    private void QExcerptSentenceRefine(CExample example)
    {
        QExcerptText.PMentionText = example.CExampleWording is string key
            ? QLocalizationCatalog.QLocalizationTextRead(key)
            : example.CExampleText.CStateValueText;
        QExcerptText.PMentionLanguage = example.CExampleLanguage;
        QExcerptText.PMentionMention = example.CExampleExcerpt;
        QExcerptText.SetResourceReference(
            TextBlock.ForegroundProperty,
            example.CExampleMuted ? "Theme.Muted" : "Theme.Ink");
    }

    private void QExcerptCitationRefine(CExample example)
    {
        QExcerptCitation.Text = example.CExampleCitation;
        QExcerptCitationSection.Visibility = example.CExampleSource is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void QExcerptMentionObserve(object? sender, PMentionArgument e)
    {
        _qCorpusHost.PWindowMentionRefine(QExcerptText, _cCorpus.CCorpusMentionFind(e.PMentionArgumentOffset));
    }
}
