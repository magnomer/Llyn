using System.ComponentModel;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PXiaoyunItem : INotifyPropertyChanged
{
    private bool _pXiaoyunItemChosen;

    internal PXiaoyunItem(LVistaRow row)
    {
        PXiaoyunItemId = row.LVistaRowId;
        PXiaoyunItemHeadword = row.LVistaRowHeadword;
        PXiaoyunItemName = row.LVistaRowName;
        PXiaoyunItemEpithet = row.LVistaRowEpithet ?? string.Empty;
        PXiaoyunItemFlag = PEnsign.PEnsignFind(row.LVistaRowLanguage);
    }

    public long PXiaoyunItemId { get; }

    public string PXiaoyunItemHeadword { get; }

    public string PXiaoyunItemEpithet { get; }

    public string PXiaoyunItemName { get; }

    public ImageSource? PXiaoyunItemFlag { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PXiaoyunItemChosen
    {
        get => _pXiaoyunItemChosen;

        set
        {
            if (_pXiaoyunItemChosen == value)
            {
                return;
            }

            _pXiaoyunItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PXiaoyunItemChosen)));
        }
    }
}
