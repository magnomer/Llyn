using System.ComponentModel;
using System.Globalization;

namespace Llyn.UIVeneer;

internal sealed class PGamutItem : INotifyPropertyChanged
{
    private bool _pGamutItemChosen;

    internal PGamutItem(long id, string name, int usage, bool chosen)
    {
        PGamutItemId = id;
        PGamutItemName = name;
        PGamutItemUsage = usage;
        _pGamutItemChosen = chosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long PGamutItemId { get; }

    public string PGamutItemName { get; }

    public int PGamutItemUsage { get; }

    public bool PGamutItemChosen
    {
        get => _pGamutItemChosen;

        set
        {
            if (_pGamutItemChosen == value)
            {
                return;
            }

            _pGamutItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PGamutItemChosen)));
        }
    }

    public string PGamutItemCount => PGamutItemUsage.ToString(CultureInfo.CurrentCulture);
}
