using System.ComponentModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAccessionItem : INotifyPropertyChanged
{
    private CAccessionRow _qAccessionItemRow;

    internal QAccessionItem(CAccessionRow row)
    {
        _qAccessionItemRow = row;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CAccessionRow QAccessionItemRow
    {
        get => _qAccessionItemRow;

        private set
        {
            if (_qAccessionItemRow == value)
            {
                return;
            }

            _qAccessionItemRow = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QAccessionItemRow)));
        }
    }

    internal static bool QAccessionItemMatch(QAccessionItem held, QAccessionItem fresh)
    {
        return held.QAccessionItemRow.CAccessionRowEntry == fresh.QAccessionItemRow.CAccessionRowEntry;
    }

    internal static void QAccessionItemSync(QAccessionItem held, QAccessionItem fresh)
    {
        held.QAccessionItemRow = fresh.QAccessionItemRow;
    }
}
