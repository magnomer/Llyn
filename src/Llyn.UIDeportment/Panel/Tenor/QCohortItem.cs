using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QCohortItem : INotifyPropertyChanged
{
    private bool _qCohortItemChosen;

    internal QCohortItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _qCohortItemChosen = chosen;
        QCohortItemId = id;
        QCohortItemHeadword = headword;
        QCohortItemEpithet = epithet ?? string.Empty;
        QCohortItemLanguage = language;
        QCohortItemFlag = QEnsignImage.QEnsignRead(language);
    }

    public long QCohortItemId { get; }

    public string QCohortItemHeadword { get; }

    public string QCohortItemEpithet { get; }

    public required string QCohortItemName { get; init; }

    public string QCohortItemLanguage { get; }

    public ImageSource? QCohortItemFlag { get; }

    internal static bool QCohortItemMatch(QCohortItem held, QCohortItem fresh)
    {
        return held.QCohortItemId == fresh.QCohortItemId
            && string.Equals(held.QCohortItemHeadword, fresh.QCohortItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.QCohortItemEpithet, fresh.QCohortItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.QCohortItemName, fresh.QCohortItemName, StringComparison.Ordinal)
            && string.Equals(held.QCohortItemLanguage, fresh.QCohortItemLanguage, StringComparison.Ordinal);
    }

    internal static void QCohortItemSync(QCohortItem held, QCohortItem fresh)
    {
        held.QCohortItemChosen = fresh.QCohortItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QCohortItemChosen
    {
        get => _qCohortItemChosen;

        set
        {
            if (_qCohortItemChosen == value)
            {
                return;
            }

            _qCohortItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QCohortItemChosen)));
        }
    }
}
