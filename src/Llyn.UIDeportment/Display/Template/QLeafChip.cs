using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLeafChip
{
    internal QLeafChip(CLeafChip chip)
    {
        ArgumentNullException.ThrowIfNull(chip);

        QLeafChipOrigin = chip;
        QLeafChipText = chip.CLeafChipWording.CStateWordingKey is string key
            ? QLocalizationCatalog.QLocalizationTextRead(key)
            : chip.CLeafChipWording.CStateWordingText;
    }

    internal CLeafChip QLeafChipOrigin { get; }

    internal string QLeafChipText { get; }
}
