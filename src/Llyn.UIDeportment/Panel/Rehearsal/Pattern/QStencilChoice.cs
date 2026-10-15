using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStencilChoice
{
    internal QStencilChoice(CStencilRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        QStencilChoiceRow = row;
    }

    public CStencilRow QStencilChoiceRow { get; }

    public override string ToString()
    {
        return QStencilChoiceRow.CStencilRowName;
    }
}
