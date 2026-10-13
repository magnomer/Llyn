using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Llyn.UIDeportment;

internal sealed class PCaret<PCaretChip> : INotifyPropertyChanged
    where PCaretChip : class
{
    private readonly string _pCaretHintKey;
    private readonly Func<PCaretChip, long?> _pCaretKey;
    private readonly Func<PCaretChip, PCaretChip, PCaretChip> _pCaretRevision;
    private string _pCaretText = string.Empty;
    private string _pCaretHint = string.Empty;
    private PCaretChip? _pCaretAnchor;

    internal PCaret(
        string hint,
        Func<PCaretChip, long?> key,
        Func<PCaretChip, PCaretChip, PCaretChip> update)
    {
        _pCaretHintKey = hint;
        _pCaretKey = key;
        _pCaretRevision = update;
        PCaretUpdate();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<PCaretChip> PCaretRow { get; } = [];

    public string PCaretText
    {
        get => _pCaretText;
        private set
        {
            string written = value ?? string.Empty;
            if (string.Equals(_pCaretText, written, StringComparison.Ordinal))
            {
                return;
            }

            _pCaretText = written;
            PCaretRaise(nameof(PCaretText));
        }
    }

    public string PCaretHint
    {
        get => _pCaretHint;
        private set
        {
            string shown = value ?? string.Empty;
            if (string.Equals(_pCaretHint, shown, StringComparison.Ordinal))
            {
                return;
            }

            _pCaretHint = shown;
            PCaretRaise(nameof(PCaretHint));
        }
    }

    public PCaretChip? PCaretAnchor
    {
        get => _pCaretAnchor;
        private set
        {
            if (ReferenceEquals(_pCaretAnchor, value))
            {
                return;
            }

            _pCaretAnchor = value;
            PCaretRaise(nameof(PCaretAnchor));
        }
    }

    internal void PCaretRowShow(IReadOnlyList<PCaretChip> chips)
    {
        QLookItem.QLookItemShow(
            PCaretRow,
            chips,
            _pCaretKey,
            chip => _pCaretKey(chip).GetValueOrDefault(),
            static chip => chip,
            _pCaretRevision);
        PCaretUpdate();
    }

    internal void PCaretAnchorShow(PCaretChip? anchor)
    {
        PCaretAnchor = anchor;
    }

    internal void PCaretClear()
    {
        PCaretText = string.Empty;
        PCaretUpdate();
    }

    internal void PCaretRefine(string rest)
    {
        if (!string.Equals(_pCaretText, rest, StringComparison.Ordinal))
        {
            PCaretText = rest;
            PCaretUpdate();
        }
    }

    private void PCaretUpdate()
    {
        PCaretHint = PCaretRow.Count > 0
            ? string.Empty
            : QLocalizationCatalog.QLocalizationTextRead(_pCaretHintKey);
    }

    private void PCaretRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
