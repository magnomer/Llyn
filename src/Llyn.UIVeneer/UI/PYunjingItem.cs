using System.ComponentModel;

namespace Llyn.UIVeneer;

internal sealed class PYunjingItem : INotifyPropertyChanged
{
    private bool _pYunjingItemChosen;

    internal PYunjingItem(long id, string key, int count)
    {
        PYunjingItemId = id;
        PYunjingItemKey = key;
        PYunjingItemCount = count;
    }

    public long PYunjingItemId { get; }

    public string PYunjingItemKey { get; }

    public int PYunjingItemCount { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PYunjingItemChosen
    {
        get => _pYunjingItemChosen;

        set
        {
            if (_pYunjingItemChosen == value)
            {
                return;
            }

            _pYunjingItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PYunjingItemChosen)));
        }
    }
}
