using System;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QQuotationItem : INotifyPropertyChanged
{
    private bool _qQuotationItemChosen;

    internal QQuotationItem(CVistaRow row, bool chosen)
    {
        _qQuotationItemChosen = chosen;
        QQuotationItemId = row.CVistaRowId;
        QQuotationItemHeadword = row.CVistaRowHeadword;
        QQuotationItemEpithet = row.CVistaRowEpithet;
        QQuotationItemName = row.CVistaRowName;
        QQuotationItemLanguage = row.CVistaRowLanguage;
        QQuotationItemFlag = LEnsignImage.LEnsignFind(row.CVistaRowLanguage);
    }

    public long QQuotationItemId { get; }

    public string QQuotationItemHeadword { get; }

    public string QQuotationItemEpithet { get; }

    public string QQuotationItemName { get; }

    public string QQuotationItemLanguage { get; }

    public ImageSource? QQuotationItemFlag { get; }

    internal static bool QQuotationItemMatch(QQuotationItem held, QQuotationItem fresh)
    {
        return held.QQuotationItemId == fresh.QQuotationItemId
            && string.Equals(held.QQuotationItemHeadword, fresh.QQuotationItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.QQuotationItemEpithet, fresh.QQuotationItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.QQuotationItemName, fresh.QQuotationItemName, StringComparison.Ordinal)
            && string.Equals(held.QQuotationItemLanguage, fresh.QQuotationItemLanguage, StringComparison.Ordinal);
    }

    internal static void QQuotationItemSync(QQuotationItem held, QQuotationItem fresh)
    {
        held.QQuotationItemChosen = fresh.QQuotationItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QQuotationItemChosen
    {
        get => _qQuotationItemChosen;

        set
        {
            if (_qQuotationItemChosen == value)
            {
                return;
            }

            _qQuotationItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QQuotationItemChosen)));
        }
    }
}
