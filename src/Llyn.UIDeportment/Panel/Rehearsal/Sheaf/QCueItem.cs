using System;
using System.ComponentModel;

namespace Llyn.UIDeportment;

internal sealed class QCueItem : INotifyPropertyChanged
{
    private bool _qCueItemFlag;

    private bool _qCueItemExcluded;

    private string _qCueItemText;

    internal QCueItem(long id, int position, string text, bool orphaned, bool flag, bool excluded)
    {
        _qCueItemFlag = flag;
        _qCueItemExcluded = excluded;
        _qCueItemText = text;
        QCueItemId = id;
        QCueItemPosition = position;
        QCueItemOrphaned = orphaned;
    }

    public long QCueItemId { get; }

    public int QCueItemPosition { get; }

    public bool QCueItemOrphaned { get; }

    internal static bool QCueItemMatch(QCueItem held, QCueItem fresh)
    {
        return held.QCueItemId == fresh.QCueItemId
            && held.QCueItemPosition == fresh.QCueItemPosition
            && held.QCueItemOrphaned == fresh.QCueItemOrphaned;
    }

    internal static void QCueItemSync(QCueItem held, QCueItem fresh)
    {
        held.QCueItemText = fresh.QCueItemText;
        held.QCueItemFlag = fresh.QCueItemFlag;
        held.QCueItemExcluded = fresh.QCueItemExcluded;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string QCueItemText
    {
        get => _qCueItemText;

        set
        {
            if (string.Equals(_qCueItemText, value, StringComparison.Ordinal))
            {
                return;
            }

            _qCueItemText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QCueItemText)));
        }
    }

    public bool QCueItemFlag
    {
        get => _qCueItemFlag;

        set
        {
            if (_qCueItemFlag == value)
            {
                return;
            }

            _qCueItemFlag = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QCueItemFlag)));
        }
    }

    public bool QCueItemExcluded
    {
        get => _qCueItemExcluded;

        set
        {
            if (_qCueItemExcluded == value)
            {
                return;
            }

            _qCueItemExcluded = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QCueItemExcluded)));
        }
    }
}
