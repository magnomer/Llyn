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
        QExcerptText.PMentionPiece = example.CExampleWording is string key
            ? QMentionPiece.QMentionPieceCreate(QLocalizationCatalog.QLocalizationTextRead(key))
            : QMentionPiece.QMentionPieceCreate(example.CExamplePiece);
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
        _qCorpusHost.QWindowMentionRefine(QExcerptText, _cCorpus.CCorpusMentionFind(e.PMentionArgumentOffset));
    }
}
