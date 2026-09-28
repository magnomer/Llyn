using System;
using Llyn.Core;

namespace Llyn.Conduct;

internal static class CPanel
{
    internal static CVistaRow CPanelRowRead(LVistaRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new CVistaRow(
            row.LVistaRowId,
            row.LVistaRowHeadword,
            row.LVistaRowLanguage,
            row.LVistaRowEpithet ?? string.Empty,
            row.LVistaRowName,
            row.LVistaRowChosen);
    }
}
