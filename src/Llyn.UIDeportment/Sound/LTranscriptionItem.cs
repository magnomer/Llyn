using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LTranscriptionItem : INotifyPropertyChanged
{
    private string _lTranscriptionItemScheme;
    private string _lTranscriptionItemText;

    public LTranscriptionItem(long id, string scheme, string text)
    {
        LTranscriptionItemId = id;
        _lTranscriptionItemScheme = scheme;
        _lTranscriptionItemText = text;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long LTranscriptionItemId { get; }

    public string LTranscriptionItemScheme
    {
        get => _lTranscriptionItemScheme;
        set
        {
            string scheme = value ?? string.Empty;
            if (scheme.Length == 0 || string.Equals(_lTranscriptionItemScheme, scheme, StringComparison.Ordinal))
            {
                return;
            }

            _lTranscriptionItemScheme = scheme;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LTranscriptionItemScheme)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LTranscriptionItemLabel)));
        }
    }

    public string LTranscriptionItemLabel => LTranscriptionLabelFormat(_lTranscriptionItemScheme);

    public string LTranscriptionItemText
    {
        get => _lTranscriptionItemText;
        set
        {
            string text = value ?? string.Empty;
            if (string.Equals(_lTranscriptionItemText, text, StringComparison.Ordinal))
            {
                return;
            }

            _lTranscriptionItemText = text;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LTranscriptionItemText)));
        }
    }

    public ObservableCollection<LTranscriptionChoice> LTranscriptionItemChoice { get; } = [];

    public void LTranscriptionItemUpdate(IReadOnlyList<string> schemes, IReadOnlyList<LTranscriptionItem> rows)
    {
        ArgumentNullException.ThrowIfNull(schemes);
        ArgumentNullException.ThrowIfNull(rows);

        if (LTranscriptionItemChoice.Count != schemes.Count)
        {
            LTranscriptionItemChoice.Clear();
            foreach (string scheme in schemes)
            {
                LTranscriptionItemChoice.Add(new LTranscriptionChoice(scheme, LTranscriptionLabelFormat(scheme)));
            }
        }

        foreach (LTranscriptionChoice choice in LTranscriptionItemChoice)
        {
            bool taken = false;
            foreach (LTranscriptionItem row in rows)
            {
                if (!ReferenceEquals(row, this)
                    && string.Equals(
                        row.LTranscriptionItemScheme, choice.LTranscriptionChoiceScheme, StringComparison.Ordinal))
                {
                    taken = true;
                    break;
                }
            }

            choice.LTranscriptionChoiceTaken = taken;
        }
    }

    internal static void LTranscriptionItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LTranscriptionItem row)
        {
            return;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PTranscriptionLabel") is TextBlock label)
        {
            label.Text = row.LTranscriptionItemLabel;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PTranscriptionText") is TextBlock text)
        {
            text.Text = row.LTranscriptionItemText;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PTranscriptionMeasure") is TextBlock measure)
        {
            LTranscriptionMeasureApply(measure, row.LTranscriptionItemText);
        }
    }

    private static void LTranscriptionMeasureApply(TextBlock measure, string text)
    {
        if (text.Length == 0)
        {
            measure.SetResourceReference(TextBlock.TextProperty, "Input.Transcription");
            return;
        }

        measure.Text = text;
    }

    public static LTranscriptionItem LTranscriptionItemCreate(LTranscriptionDraft spelled)
    {
        ArgumentNullException.ThrowIfNull(spelled);

        return new LTranscriptionItem(
            spelled.LTranscriptionDraftId,
            spelled.LTranscriptionDraftScheme,
            spelled.LTranscriptionDraftText);
    }

    public static string LTranscriptionLabelFormat(string scheme)
    {
        ArgumentNullException.ThrowIfNull(scheme);

        return scheme.Length == 0
            ? string.Empty
            : LLocalizationCatalog.LLocalizationTextFind(string.Concat("Scheme.", scheme)) ?? scheme;
    }
}
