using System;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLinkChip : INotifyPropertyChanged
{
    private ImageSource? _qLinkChipFlag;

    public QLinkChip(CTranslationTarget target)
    {
        ArgumentNullException.ThrowIfNull(target);

        QLinkChipTarget = target;
        _qLinkChipFlag = QEnsignImage.QEnsignRead(target.CTranslationTargetLanguage);
    }

    public CTranslationTarget QLinkChipTarget { get; }

    public ImageSource? QLinkChipFlag => _qLinkChipFlag;

    public event PropertyChangedEventHandler? PropertyChanged;

    internal void QLinkChipRefine()
    {
        if (_qLinkChipFlag is not null)
        {
            return;
        }

        _qLinkChipFlag = QEnsignImage.QEnsignRead(QLinkChipTarget.CTranslationTargetLanguage);
        if (_qLinkChipFlag is not null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QLinkChipFlag)));
        }
    }
}
