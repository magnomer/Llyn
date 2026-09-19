using System.ComponentModel;

namespace Llyn.UIVeneer;

internal sealed class PDirectoryItem : INotifyPropertyChanged
{
    private bool _pDirectoryItemChosen;

    internal PDirectoryItem(long id, string text, bool chosen)
    {
        PDirectoryItemId = id;
        PDirectoryItemText = text;
        _pDirectoryItemChosen = chosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal long PDirectoryItemId { get; }

    public string PDirectoryItemText { get; }

    public bool PDirectoryItemChosen
    {
        get => _pDirectoryItemChosen;

        set
        {
            if (_pDirectoryItemChosen == value)
            {
                return;
            }

            _pDirectoryItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PDirectoryItemChosen)));
        }
    }
}
