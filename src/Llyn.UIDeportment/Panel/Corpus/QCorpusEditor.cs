using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private long _qTranscriptCitation;

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
            languages = _qCorpusHost.PWindowDeportment.LWindowWorkspace.QWorkspaceLanguageRead();
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
        QTranscriptQuill.QQuillExampleChange(CExampleField.CExampleFieldLanguage, item.PLanguageItemName);
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
        QTranscriptQuill.QQuillExampleChange(CExampleField.CExampleFieldText, QTranscriptText.Text);
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

        _qTranscriptCitation = example?.CExampleSource ?? 0;
        QCitationUpdate();

        QTranscriptMentionShow(example);

        QTranscriptTally.Text = QCorpusTallyRead(QTranscriptDesk.LDeskStoredRead());

        QTranscriptAttach();
    }

    private void QTranscriptShow(CExample? example)
    {
        if (example is null)
        {
            return;
        }

        QTranscriptDetach();

        if (!_lCorpus.LCorpusAnthology.LAnthologyTextCheck(QTranscriptText.Text, example.CExampleText))
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

        long shown = _qTranscriptCitation;
        _qTranscriptCitation = example.CExampleSource ?? 0;
        if (shown != _qTranscriptCitation)
        {
            QCitationUpdate();
        }

        QTranscriptMentionShow(example);

        QTranscriptAttach();
    }

    private void QCorpusFreshHandle(object sender, RoutedEventArgs e)
    {
        _lCorpus.LCorpusFreshStart();
    }
}
