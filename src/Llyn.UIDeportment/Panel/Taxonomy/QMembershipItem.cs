using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QMembershipItem : INotifyPropertyChanged
{
    private bool _qMembershipItemChosen;

    internal QMembershipItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _qMembershipItemChosen = chosen;
        QMembershipItemId = id;
        QMembershipItemHeadword = headword;
        QMembershipItemEpithet = epithet ?? string.Empty;
        QMembershipItemLanguage = language;
        QMembershipItemFlag = QEnsignImage.QEnsignRead(language);
    }

    public long QMembershipItemId { get; }

    public string QMembershipItemHeadword { get; }

    public string QMembershipItemEpithet { get; }

    public required string QMembershipItemName { get; init; }

    public string QMembershipItemLanguage { get; }

    public ImageSource? QMembershipItemFlag { get; }

    internal static bool QMembershipItemMatch(QMembershipItem held, QMembershipItem fresh)
    {
        return held.QMembershipItemId == fresh.QMembershipItemId
            && string.Equals(held.QMembershipItemHeadword, fresh.QMembershipItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.QMembershipItemEpithet, fresh.QMembershipItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.QMembershipItemName, fresh.QMembershipItemName, StringComparison.Ordinal)
            && string.Equals(held.QMembershipItemLanguage, fresh.QMembershipItemLanguage, StringComparison.Ordinal);
    }

    internal static void QMembershipItemSync(QMembershipItem held, QMembershipItem fresh)
    {
        held.QMembershipItemChosen = fresh.QMembershipItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QMembershipItemChosen
    {
        get => _qMembershipItemChosen;

        set
        {
            if (_qMembershipItemChosen == value)
            {
                return;
            }

            _qMembershipItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QMembershipItemChosen)));
        }
    }
}
