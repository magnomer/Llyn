using System.ComponentModel;

namespace Llyn.UIDeportment;

public sealed class QVolumeCatalog : INotifyPropertyChanged
{
    private double _qVolumeCatalogLevel = 1;

    public static QVolumeCatalog QVolumeCatalogCurrent { get; } = new();

    public double QVolumeCatalogLevel
    {
        get => _qVolumeCatalogLevel;
        set
        {
            if (_qVolumeCatalogLevel == value)
            {
                return;
            }

            _qVolumeCatalogLevel = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QVolumeCatalogLevel)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
