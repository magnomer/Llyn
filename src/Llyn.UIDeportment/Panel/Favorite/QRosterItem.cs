using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QRosterItem : INotifyPropertyChanged
{
    private bool _qRosterItemChosen;

    internal QRosterItem(
        long id, string headword, string name, string language, string epithet, bool chosen)
    {
        _qRosterItemChosen = chosen;
        QRosterItemId = id;
        QRosterItemHeadword = headword;
        QRosterItemName = name;
        QRosterItemEpithet = epithet;
        QRosterItemLanguage = language;
        QRosterItemFlag = LEnsignImage.LEnsignFind(language);
    }

    public long QRosterItemId { get; }

    public string QRosterItemHeadword { get; }

    public string QRosterItemEpithet { get; }

    public string QRosterItemName { get; }

    public string QRosterItemLanguage { get; }

    public ImageSource? QRosterItemFlag { get; }

    internal static bool QRosterItemMatch(QRosterItem held, QRosterItem fresh)
    {
        return held.QRosterItemId == fresh.QRosterItemId
            && string.Equals(held.QRosterItemHeadword, fresh.QRosterItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.QRosterItemEpithet, fresh.QRosterItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.QRosterItemName, fresh.QRosterItemName, StringComparison.Ordinal)
            && string.Equals(held.QRosterItemLanguage, fresh.QRosterItemLanguage, StringComparison.Ordinal);
    }

    internal static void QRosterItemSync(QRosterItem held, QRosterItem fresh)
    {
        held.QRosterItemChosen = fresh.QRosterItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QRosterItemChosen
    {
        get => _qRosterItemChosen;

        set
        {
            if (_qRosterItemChosen == value)
            {
                return;
            }

            _qRosterItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QRosterItemChosen)));
        }
    }
}
