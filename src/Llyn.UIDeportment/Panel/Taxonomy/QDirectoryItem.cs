using System.ComponentModel;

namespace Llyn.UIDeportment;

internal sealed class QDirectoryItem : INotifyPropertyChanged
{
    private bool _qDirectoryItemChosen;

    internal QDirectoryItem(long id, string text, bool chosen)
    {
        QDirectoryItemId = id;
        QDirectoryItemText = text;
        _qDirectoryItemChosen = chosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal long QDirectoryItemId { get; }

    public string QDirectoryItemText { get; }

    public bool QDirectoryItemChosen
    {
        get => _qDirectoryItemChosen;

        set
        {
            if (_qDirectoryItemChosen == value)
            {
                return;
            }

            _qDirectoryItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QDirectoryItemChosen)));
        }
    }
}
