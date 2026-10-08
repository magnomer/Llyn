using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIDeportment;

public sealed class QLinkChip : INotifyPropertyChanged
{
    private ImageSource? _qLinkChipFlag;

    public QLinkChip(long id, string headword, string language)
    {
        ArgumentNullException.ThrowIfNull(headword);
        ArgumentNullException.ThrowIfNull(language);

        QLinkChipId = id;
        QLinkChipHeadword = headword;
        QLinkChipLanguage = language;
        _qLinkChipFlag = QEnsignImage.QEnsignRead(language);
    }

    public long QLinkChipId { get; }

    public string QLinkChipHeadword { get; }

    public string QLinkChipLanguage { get; }

    public ImageSource? QLinkChipFlag => _qLinkChipFlag;

    public event PropertyChangedEventHandler? PropertyChanged;

    internal void QLinkChipRefine()
    {
        if (_qLinkChipFlag is not null)
        {
            return;
        }

        _qLinkChipFlag = QEnsignImage.QEnsignRead(QLinkChipLanguage);
        if (_qLinkChipFlag is not null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QLinkChipFlag)));
        }
    }
}
