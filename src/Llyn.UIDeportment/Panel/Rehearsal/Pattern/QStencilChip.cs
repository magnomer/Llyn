using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStencilChip
{
    internal QStencilChip(int side, int position, CStencilChip field, IReadOnlyList<CStencilJoin> joins)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(joins);

        QStencilChipSide = side;
        QStencilChipPosition = position;
        QStencilChipField = field;
        QStencilChipJoins = joins;
    }

    public int QStencilChipSide { get; }

    public int QStencilChipPosition { get; }

    public CStencilChip QStencilChipField { get; }

    public IReadOnlyList<CStencilJoin> QStencilChipJoins { get; }
}
