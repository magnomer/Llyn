using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private string _qTranscriptCitation = string.Empty;

    private string _qTranscriptLanguage = string.Empty;

    private void QTranscriptAttach()
    {
        QTranscriptText.TextChanged += QTranscriptTextHandle;
    }

    private void QTranscriptDetach()
    {
        QTranscriptText.TextChanged -= QTranscriptTextHandle;
    }

    private void QSpeakerLoad()
    {
        IReadOnlyList<string> languages;
        try
        {
            languages = _qCorpusHost.PWindowAtelier.CAtelierCatalog.CCatalogLanguageRead();
        }
        catch (Exception)
        {
            languages = [];
        }

        PLanguageItem.PLanguageItemReset(_qLanguageItem, languages);
        QSpeakerShow();
    }

    private void QSpeakerHandle(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<PLanguageItem>(sender) is not PLanguageItem item)
        {
            return;
        }

        QSpeaker.IsChecked = false;
        QTranscriptQuill?.LQuillSpeakerSet(item.PLanguageItemName);
    }

    private void QSpeakerApply(FrameworkElement container, object item, string? change)
    {
        PLanguageItem.PLanguageItemApply(container, item, change);

        if (QLook.QLookPartFind<Button>(container, "PSpeakerChoice") is Button choice)
        {
            choice.Click -= QSpeakerHandle;
            choice.Click += QSpeakerHandle;
        }
    }

    private void QSpeakerShow()
    {
        QSpeakerName.Text = _qTranscriptLanguage;
        QSpeakerFlag.Source = LEnsignImage.LEnsignFind(_qTranscriptLanguage);
    }

    private void QTranscriptTextHandle(object sender, TextChangedEventArgs e)
    {
        QTranscriptText.SetValue(QField.QFieldHintProperty, QTranscriptHintRead(false));
        QTranscriptQuill?.LQuillExampleSet(QTranscriptText.Text);
    }

    private static string QTranscriptHintRead(bool unknown)
    {
        return QLocalizationCatalog.QLocalizationTextRead(unknown ? "Display.Unknown" : "Example.Text");
    }

    private void QTranscriptApply(CExample? example)
    {
        QTranscriptDetach();

        QTranscriptText.Text = example?.CExampleText.CStateValueText ?? string.Empty;
        QTranscriptText.SetValue(
            QField.QFieldHintProperty, QTranscriptHintRead(example?.CExampleText.CStateValueUncertain ?? false));

        QTranscriptGlossShow(example);

        _qTranscriptLanguage = example?.CExampleLanguage ?? string.Empty;
        QSpeakerShow();

        _qTranscriptCitation = example?.CExampleCitation ?? string.Empty;
        QCitationRefine();

        QTranscriptMentionShow(example);

        QTranscriptTally.Text = QCorpusTallyRead(QTranscriptDesk.CDeskStoredRead());

        QTranscriptAttach();
    }

    private void QTranscriptShow(CExample? example)
    {
        if (example is null)
        {
            return;
        }

        QTranscriptDetach();

        if (!CAnthology.CAnthologyTextCheck(QTranscriptText.Text, example.CExampleText))
        {
            QTranscriptText.Text = example.CExampleText.CStateValueText;
        }

        QTranscriptText.SetValue(
            QField.QFieldHintProperty, QTranscriptHintRead(example.CExampleText.CStateValueUncertain));
        QTranscriptGlossShow(example);

        if (!string.Equals(_qTranscriptLanguage, example.CExampleLanguage, StringComparison.Ordinal))
        {
            _qTranscriptLanguage = example.CExampleLanguage;
            QSpeakerShow();
        }

        string shown = _qTranscriptCitation;
        _qTranscriptCitation = example.CExampleCitation;
        if (!string.Equals(shown, _qTranscriptCitation, StringComparison.Ordinal))
        {
            QCitationRefine();
        }

        QTranscriptMentionShow(example);

        QTranscriptAttach();
    }

    private void QCorpusFreshHandle(object sender, RoutedEventArgs e)
    {
        _cCorpus.CCorpusExampleCreate();
    }
}
