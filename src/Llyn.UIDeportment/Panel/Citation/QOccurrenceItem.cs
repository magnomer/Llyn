using System;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QOccurrenceItem : INotifyPropertyChanged
{
    private bool _qOccurrenceItemChosen;

    internal QOccurrenceItem(CVistaRow row, bool chosen)
    {
        _qOccurrenceItemChosen = chosen;
        QOccurrenceItemId = row.CVistaRowId;
        QOccurrenceItemHeadword = row.CVistaRowHeadword;
        QOccurrenceItemEpithet = row.CVistaRowEpithet;
        QOccurrenceItemName = row.CVistaRowName;
        QOccurrenceItemLanguage = row.CVistaRowLanguage;
        QOccurrenceItemFlag = LEnsignImage.LEnsignFind(row.CVistaRowLanguage);
    }

    public long QOccurrenceItemId { get; }

    public string QOccurrenceItemHeadword { get; }

    public string QOccurrenceItemEpithet { get; }

    public string QOccurrenceItemName { get; }

    public string QOccurrenceItemLanguage { get; }

    public ImageSource? QOccurrenceItemFlag { get; }

    internal static bool QOccurrenceItemMatch(QOccurrenceItem held, QOccurrenceItem fresh)
    {
        return held.QOccurrenceItemId == fresh.QOccurrenceItemId
            && string.Equals(held.QOccurrenceItemHeadword, fresh.QOccurrenceItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.QOccurrenceItemEpithet, fresh.QOccurrenceItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.QOccurrenceItemName, fresh.QOccurrenceItemName, StringComparison.Ordinal)
            && string.Equals(held.QOccurrenceItemLanguage, fresh.QOccurrenceItemLanguage, StringComparison.Ordinal);
    }

    internal static void QOccurrenceItemSync(QOccurrenceItem held, QOccurrenceItem fresh)
    {
        held.QOccurrenceItemChosen = fresh.QOccurrenceItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QOccurrenceItemChosen
    {
        get => _qOccurrenceItemChosen;

        set
        {
            if (_qOccurrenceItemChosen == value)
            {
                return;
            }

            _qOccurrenceItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QOccurrenceItemChosen)));
        }
    }
}
