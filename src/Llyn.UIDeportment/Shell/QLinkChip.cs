using System;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLinkChip
{
    public QLinkChip(CTranslationTarget target)
    {
        ArgumentNullException.ThrowIfNull(target);

        QLinkChipTarget = target;
        QLinkChipFlag = QEnsignImage.QEnsignRead(target.CTranslationTargetLanguage);
    }

    public CTranslationTarget QLinkChipTarget { get; }

    public ImageSource? QLinkChipFlag { get; }
}
