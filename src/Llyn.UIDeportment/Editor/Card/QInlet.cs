using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Llyn.UIDeportment;

internal sealed class QInlet<QInletChip> : INotifyPropertyChanged
    where QInletChip : class
{
    private readonly string _qInletHintKey;
    private readonly Func<QInletChip, long?> _qInletKey;
    private readonly Func<QInletChip, QInletChip, QInletChip> _qInletRevision;
    private string _qInletText = string.Empty;
    private string _qInletHint = string.Empty;
    private QInletChip? _qInletAnchor;

    internal QInlet(
        string hint,
        Func<QInletChip, long?> key,
        Func<QInletChip, QInletChip, QInletChip> update)
    {
        _qInletHintKey = hint;
        _qInletKey = key;
        _qInletRevision = update;
        QInletUpdate();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<QInletChip> QInletRow { get; } = [];

    public string QInletText
    {
        get => _qInletText;
        private set
        {
            string written = value ?? string.Empty;
            if (string.Equals(_qInletText, written, StringComparison.Ordinal))
            {
                return;
            }

            _qInletText = written;
            QInletRaise(nameof(QInletText));
        }
    }

    public string QInletHint
    {
        get => _qInletHint;
        private set
        {
            string shown = value ?? string.Empty;
            if (string.Equals(_qInletHint, shown, StringComparison.Ordinal))
            {
                return;
            }

            _qInletHint = shown;
            QInletRaise(nameof(QInletHint));
        }
    }

    public QInletChip? QInletAnchor
    {
        get => _qInletAnchor;
        private set
        {
            if (ReferenceEquals(_qInletAnchor, value))
            {
                return;
            }

            _qInletAnchor = value;
            QInletRaise(nameof(QInletAnchor));
        }
    }

    internal void QInletRowShow(IReadOnlyList<QInletChip> chips)
    {
        QLookItem.QLookItemShow(
            QInletRow,
            chips,
            _qInletKey,
            chip => _qInletKey(chip).GetValueOrDefault(),
            static chip => chip,
            _qInletRevision);
        QInletUpdate();
    }

    internal void QInletAnchorShow(QInletChip? anchor)
    {
        QInletAnchor = anchor;
    }

    internal void QInletClear()
    {
        QInletText = string.Empty;
        QInletUpdate();
    }

    internal void QInletRefine(string rest)
    {
        if (!string.Equals(_qInletText, rest, StringComparison.Ordinal))
        {
            QInletText = rest;
            QInletUpdate();
        }
    }

    private void QInletUpdate()
    {
        QInletHint = QInletRow.Count > 0
            ? string.Empty
            : QLocalizationCatalog.QLocalizationTextRead(_qInletHintKey);
    }

    private void QInletRaise(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
