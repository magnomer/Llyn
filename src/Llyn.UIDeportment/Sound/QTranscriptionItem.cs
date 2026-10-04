using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QTranscriptionItem : INotifyPropertyChanged
{
    private string _qTranscriptionItemScheme;
    private string _qTranscriptionItemText;

    public QTranscriptionItem(long id, string scheme, string text)
    {
        QTranscriptionItemId = id;
        _qTranscriptionItemScheme = scheme;
        _qTranscriptionItemText = text;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long QTranscriptionItemId { get; }

    public string QTranscriptionItemScheme
    {
        get => _qTranscriptionItemScheme;
        set
        {
            string scheme = value ?? string.Empty;
            if (scheme.Length == 0 || string.Equals(_qTranscriptionItemScheme, scheme, StringComparison.Ordinal))
            {
                return;
            }

            _qTranscriptionItemScheme = scheme;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QTranscriptionItemScheme)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QTranscriptionItemLabel)));
        }
    }

    public string QTranscriptionItemKey { get; set; } = string.Empty;

    public string QTranscriptionItemLabel =>
        QTranscriptionLabelRefine(QTranscriptionItemKey, _qTranscriptionItemScheme);

    public string QTranscriptionItemText
    {
        get => _qTranscriptionItemText;
        set
        {
            string text = value ?? string.Empty;
            if (string.Equals(_qTranscriptionItemText, text, StringComparison.Ordinal))
            {
                return;
            }

            _qTranscriptionItemText = text;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QTranscriptionItemText)));
        }
    }

    public ObservableCollection<QTranscriptionChoice> QTranscriptionItemChoice { get; } = [];

    internal void QTranscriptionSchemeRefine(IReadOnlyList<CScheme> schemes)
    {
        ArgumentNullException.ThrowIfNull(schemes);

        bool matched = QTranscriptionItemChoice.Count == schemes.Count;
        for (int index = 0; matched && index < schemes.Count; index++)
        {
            matched = string.Equals(
                    QTranscriptionItemChoice[index].QTranscriptionChoiceScheme,
                    schemes[index].CSchemeName,
                    StringComparison.Ordinal)
                && string.Equals(
                    QTranscriptionItemChoice[index].QTranscriptionChoiceLabel,
                    QTranscriptionLabelRefine(schemes[index].CSchemeKey, schemes[index].CSchemeName),
                    StringComparison.Ordinal);
        }

        if (!matched)
        {
            QTranscriptionItemChoice.Clear();
            foreach (CScheme scheme in schemes)
            {
                QTranscriptionItemChoice.Add(new QTranscriptionChoice(
                    scheme.CSchemeName, QTranscriptionLabelRefine(scheme.CSchemeKey, scheme.CSchemeName)));
            }
        }

        for (int index = 0; index < schemes.Count; index++)
        {
            QTranscriptionItemChoice[index].QTranscriptionChoiceTaken = schemes[index].CSchemeTaken;
        }
    }

    internal static void QTranscriptionItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QTranscriptionItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PTranscriptionLabel") is TextBlock label)
        {
            label.Text = row.QTranscriptionItemLabel;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PTranscriptionText") is TextBlock text)
        {
            text.Text = row.QTranscriptionItemText;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PTranscriptionMeasure") is TextBlock measure)
        {
            QTranscriptionMeasureRefine(measure, row.QTranscriptionItemText);
        }
    }

    private static void QTranscriptionMeasureRefine(TextBlock measure, string text)
    {
        if (text.Length == 0)
        {
            measure.SetResourceReference(TextBlock.TextProperty, "Input.Transcription");
            return;
        }

        measure.Text = text;
    }

    internal static QTranscriptionItem QTranscriptionRowRefine(CTranscriptionDraft spelled)
    {
        ArgumentNullException.ThrowIfNull(spelled);

        return new QTranscriptionItem(
            spelled.CTranscriptionDraftId, spelled.CTranscriptionDraftScheme, spelled.CTranscriptionDraftText)
        {
            QTranscriptionItemKey = spelled.CTranscriptionDraftKey,
        };
    }

    internal static QTranscriptionItem QTranscriptionStateRefine(QTranscriptionItem row, CTranscriptionDraft spelled)
    {
        row.QTranscriptionItemKey = spelled.CTranscriptionDraftKey;
        row.QTranscriptionItemScheme = spelled.CTranscriptionDraftScheme;
        row.QTranscriptionItemText = spelled.CTranscriptionDraftText;
        return row;
    }

    internal static string QTranscriptionLabelRefine(string key, string scheme)
    {
        return QLocalizationCatalog.QLocalizationTextFind(key) ?? scheme;
    }
}
