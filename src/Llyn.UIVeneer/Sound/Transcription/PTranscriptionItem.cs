using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PTranscriptionItem : INotifyPropertyChanged
{
    private readonly PWindow _pTranscriptionItemHost;
    private string _pTranscriptionItemScheme;
    private string _pTranscriptionItemText;

    internal PTranscriptionItem(PWindow host, long id, string scheme, string text)
    {
        _pTranscriptionItemHost = host;
        PTranscriptionItemId = id;
        _pTranscriptionItemScheme = scheme;
        _pTranscriptionItemText = text;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long PTranscriptionItemId { get; }

    public string PTranscriptionItemScheme
    {
        get => _pTranscriptionItemScheme;
        set
        {
            string scheme = value ?? string.Empty;
            if (scheme.Length == 0 || string.Equals(_pTranscriptionItemScheme, scheme, StringComparison.Ordinal))
            {
                return;
            }

            _pTranscriptionItemScheme = scheme;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PTranscriptionItemScheme)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PTranscriptionItemLabel)));
        }
    }

    public string PTranscriptionItemLabel =>
        PTranscriptionLabelFormat(_pTranscriptionItemHost, _pTranscriptionItemScheme);

    public string PTranscriptionItemText
    {
        get => _pTranscriptionItemText;
        set
        {
            string text = value ?? string.Empty;
            if (string.Equals(_pTranscriptionItemText, text, StringComparison.Ordinal))
            {
                return;
            }

            _pTranscriptionItemText = text;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PTranscriptionItemText)));
        }
    }

    public ObservableCollection<PTranscriptionChoice> PTranscriptionItemChoice { get; } = [];

    internal void PTranscriptionItemUpdate(IReadOnlyList<string> schemes, IReadOnlyList<PTranscriptionItem> rows)
    {
        if (PTranscriptionItemChoice.Count != schemes.Count)
        {
            PTranscriptionItemChoice.Clear();
            foreach (string scheme in schemes)
            {
                PTranscriptionItemChoice.Add(new PTranscriptionChoice(
                    scheme, PTranscriptionLabelFormat(_pTranscriptionItemHost, scheme)));
            }
        }

        foreach (PTranscriptionChoice choice in PTranscriptionItemChoice)
        {
            bool taken = false;
            foreach (PTranscriptionItem row in rows)
            {
                if (!ReferenceEquals(row, this)
                    && string.Equals(
                        row.PTranscriptionItemScheme, choice.PTranscriptionChoiceScheme, StringComparison.Ordinal))
                {
                    taken = true;
                    break;
                }
            }

            choice.PTranscriptionChoiceTaken = taken;
        }
    }

    internal static PTranscriptionItem PTranscriptionItemCreate(PWindow host, LTranscriptionDraft spelled)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(spelled);

        return new PTranscriptionItem(
            host,
            spelled.LTranscriptionDraftId,
            spelled.LTranscriptionDraftScheme,
            spelled.LTranscriptionDraftText);
    }

    internal static string PTranscriptionLabelFormat(PWindow host, string scheme)
    {
        ArgumentNullException.ThrowIfNull(host);

        return scheme.Length == 0
            ? string.Empty
            : PLocalizationCatalog.PLocalizationTextFind(string.Concat("Scheme.", scheme)) ?? scheme;
    }
}
