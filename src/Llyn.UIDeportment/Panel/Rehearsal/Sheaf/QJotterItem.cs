using System.ComponentModel;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QJotterItem : INotifyPropertyChanged
{
    private CCueCell _qJotterItemCell;

    internal QJotterItem(long cue, CCueCell cell)
    {
        QJotterItemCue = cue;
        _qJotterItemCell = cell;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long QJotterItemCue { get; }

    public CCueCell QJotterItemCell
    {
        get => _qJotterItemCell;

        private set
        {
            if (_qJotterItemCell == value)
            {
                return;
            }

            _qJotterItemCell = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QJotterItemCell)));
        }
    }

    internal static bool QJotterItemMatch(QJotterItem held, QJotterItem fresh)
    {
        return held.QJotterItemCue == fresh.QJotterItemCue
            && held.QJotterItemCell.CCueCellColumn == fresh.QJotterItemCell.CCueCellColumn
            && held.QJotterItemCell.CCueCellIndex == fresh.QJotterItemCell.CCueCellIndex;
    }

    internal static void QJotterItemSync(QJotterItem held, QJotterItem fresh)
    {
        held.QJotterItemCell = fresh.QJotterItemCell;
    }
}
