using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStencilSide : INotifyPropertyChanged
{
    private CStencilSide _qStencilSideRow;

    internal QStencilSide(CStencilSide row, IReadOnlyList<CStencilField> fields, IReadOnlyList<CStencilJoin> joins)
    {
        _qStencilSideRow = row;
        QStencilSideFields = fields;
        QStencilSideJoins = joins;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public CStencilSide QStencilSideRow
    {
        get => _qStencilSideRow;

        private set
        {
            if (_qStencilSideRow == value)
            {
                return;
            }

            _qStencilSideRow = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QStencilSideRow)));
        }
    }

    public IReadOnlyList<CStencilField> QStencilSideFields { get; private set; }

    public IReadOnlyList<CStencilJoin> QStencilSideJoins { get; private set; }

    public IReadOnlyList<QStencilChip> QStencilSideChips =>
    [
        .. QStencilSideRow.CStencilSideChips.Select((chip, position) =>
            new QStencilChip(QStencilSideRow.CStencilSideIndex, position, chip, QStencilSideJoins)),
    ];

    internal static bool QStencilSideMatch(QStencilSide held, QStencilSide fresh)
    {
        return held.QStencilSideRow.CStencilSideIndex == fresh.QStencilSideRow.CStencilSideIndex;
    }

    internal static void QStencilSideSync(QStencilSide held, QStencilSide fresh)
    {
        held.QStencilSideFields = fresh.QStencilSideFields;
        held.QStencilSideJoins = fresh.QStencilSideJoins;
        held.QStencilSideRow = fresh.QStencilSideRow;
    }
}
