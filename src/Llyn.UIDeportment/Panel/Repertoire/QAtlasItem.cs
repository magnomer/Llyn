using System;
using System.ComponentModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAtlasItem : INotifyPropertyChanged
{
    private bool _qAtlasItemChosen;

    internal QAtlasItem(CCatalogSituation row, bool chosen)
    {
        _qAtlasItemChosen = chosen;
        QAtlasItemId = row.CCatalogSituationId;
        QAtlasItemTitle = row.CCatalogSituationTitle;
        QAtlasItemKind = row.CCatalogSituationKind;
        QAtlasItemCount = row.CCatalogSituationCount;
    }

    public long QAtlasItemId { get; }

    public string QAtlasItemTitle { get; }

    public string QAtlasItemKind { get; }

    public string QAtlasItemCount { get; }

    internal static bool QAtlasItemMatch(QAtlasItem held, QAtlasItem fresh)
    {
        return held.QAtlasItemId == fresh.QAtlasItemId
            && string.Equals(held.QAtlasItemTitle, fresh.QAtlasItemTitle, StringComparison.Ordinal)
            && string.Equals(held.QAtlasItemKind, fresh.QAtlasItemKind, StringComparison.Ordinal)
            && string.Equals(held.QAtlasItemCount, fresh.QAtlasItemCount, StringComparison.Ordinal);
    }

    internal static void QAtlasItemSync(QAtlasItem held, QAtlasItem fresh)
    {
        held.QAtlasItemChosen = fresh.QAtlasItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QAtlasItemChosen
    {
        get => _qAtlasItemChosen;

        set
        {
            if (_qAtlasItemChosen == value)
            {
                return;
            }

            _qAtlasItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QAtlasItemChosen)));
        }
    }
}
