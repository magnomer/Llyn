using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QRegisterChip
{
    internal QRegisterChip(CStateWording text, long id)
    {
        ArgumentNullException.ThrowIfNull(text);

        QRegisterChipText = text;
        QRegisterChipId = id;
    }

    public long QRegisterChipId { get; }

    public CStateWording QRegisterChipText { get; }
}
