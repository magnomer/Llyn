using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private string _qTranscriptCitation = string.Empty;

    private void QTranscriptAttach()
    {
        QTranscriptText.TextChanged += QTranscriptTextObserve;
    }

    private void QTranscriptTeardown()
    {
        QTranscriptText.TextChanged -= QTranscriptTextObserve;
    }

    private void QSpeakerRefine(IReadOnlyList<string> languages)
    {
        PLanguageItem.PLanguageItemReset(_qLanguageItem, languages);
    }

    private void QSpeakerObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<PLanguageItem>(sender) is not PLanguageItem item)
        {
            return;
        }

        _cCorpus.CCorpusAnthology.CAnthologySpeakerSet(item.PLanguageItemName);
        QSpeakerDropperRefine();
    }

    private void QSpeakerDropperRefine()
    {
        QSpeaker.IsChecked = false;
    }

    private void QSpeakerApply(FrameworkElement container, object item, string? change)
    {
        PLanguageItem.PLanguageItemApply(container, item, change);

        if (QLook.QLookPartFind<Button>(container, "PSpeakerChoice") is Button choice)
        {
            choice.Click -= QSpeakerObserve;
            choice.Click += QSpeakerObserve;
        }
    }

    private void QSpeakerShow(string language)
    {
        QSpeakerName.Text = language;
        QSpeakerFlag.Source = QEnsignImage.QEnsignRead(language);
    }

    private void QTranscriptTextObserve(object sender, TextChangedEventArgs e)
    {
        QTranscriptHintRefine(_cCorpus.CCorpusAnthology.CAnthologyTextSet(QTranscriptText.Text));
    }

    private void QTranscriptHintRefine(string hint)
    {
        QTranscriptText.SetValue(QField.QFieldHintProperty, QLocalizationCatalog.QLocalizationTextRead(hint));
    }

    private void QTranscriptRefine(CExample example)
    {
        QTranscriptTeardown();

        QTranscriptText.Text = example.CExampleText.CStateValueText;
        QTranscriptHintRefine(example.CExampleTextHint);

        QTranscriptGlossShow(example);

        QSpeakerShow(example.CExampleLanguage);

        _qTranscriptCitation = example.CExampleCitation;
        QCitationRefine();

        QTranscriptMentionRefine();

        QTranscriptTally.Text = example.CExampleTally;

        QTranscriptAttach();
    }

    private void QTranscriptDraftRefine(CExample example)
    {
        QTranscriptTeardown();

        if (!example.CExampleTextKept)
        {
            QTranscriptText.Text = example.CExampleText.CStateValueText;
        }

        QTranscriptHintRefine(example.CExampleTextHint);
        QTranscriptGlossShow(example);

        QSpeakerShow(example.CExampleLanguage);

        string shown = _qTranscriptCitation;
        _qTranscriptCitation = example.CExampleCitation;
        if (!string.Equals(shown, _qTranscriptCitation, StringComparison.Ordinal))
        {
            QCitationRefine();
        }

        QTranscriptMentionRefine();

        QTranscriptAttach();
    }

    private void QCorpusFreshObserve(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusDiptych.CDiptychEntryCreate();
    }
}
