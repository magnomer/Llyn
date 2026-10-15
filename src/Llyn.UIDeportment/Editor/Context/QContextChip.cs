using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QContextChip
{
    internal QContextChip(CStateWording text, long id)
    {
        ArgumentNullException.ThrowIfNull(text);

        QContextChipText = text;
        QContextChipId = id;
    }

    public long QContextChipId { get; }

    public CStateWording QContextChipText { get; }
}
