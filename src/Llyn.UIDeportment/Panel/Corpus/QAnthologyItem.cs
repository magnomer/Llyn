using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAnthologyItem : INotifyPropertyChanged
{
    private bool _qAnthologyItemChosen;

    internal QAnthologyItem(CCatalogExample row, bool chosen)
    {
        _qAnthologyItemChosen = chosen;
        QAnthologyItemId = row.CCatalogExampleId;
        QAnthologyItemText = row.CCatalogExampleText;
        QAnthologyItemName = row.CCatalogExampleName;
        QAnthologyItemLanguage = row.CCatalogExampleLanguage;
        QAnthologyItemFlag = LEnsignImage.LEnsignFind(row.CCatalogExampleLanguage);
        QAnthologyItemCount = row.CCatalogExampleUsage.ToString(CultureInfo.CurrentCulture);
    }

    public long QAnthologyItemId { get; }

    public string QAnthologyItemText { get; }

    public string QAnthologyItemName { get; }

    public string QAnthologyItemLanguage { get; }

    public ImageSource? QAnthologyItemFlag { get; }

    public string QAnthologyItemCount { get; }

    internal static bool QAnthologyItemMatch(QAnthologyItem held, QAnthologyItem fresh)
    {
        return held.QAnthologyItemId == fresh.QAnthologyItemId
            && string.Equals(held.QAnthologyItemText, fresh.QAnthologyItemText, StringComparison.Ordinal)
            && string.Equals(held.QAnthologyItemName, fresh.QAnthologyItemName, StringComparison.Ordinal)
            && string.Equals(held.QAnthologyItemLanguage, fresh.QAnthologyItemLanguage, StringComparison.Ordinal)
            && string.Equals(held.QAnthologyItemCount, fresh.QAnthologyItemCount, StringComparison.Ordinal);
    }

    internal static void QAnthologyItemSync(QAnthologyItem held, QAnthologyItem fresh)
    {
        held.QAnthologyItemChosen = fresh.QAnthologyItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool QAnthologyItemChosen
    {
        get => _qAnthologyItemChosen;

        set
        {
            if (_qAnthologyItemChosen == value)
            {
                return;
            }

            _qAnthologyItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QAnthologyItemChosen)));
        }
    }
}
