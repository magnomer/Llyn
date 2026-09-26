using System.ComponentModel;

namespace Llyn.UIDeportment;

public sealed class PVolumeCatalog : INotifyPropertyChanged
{
    private double _pVolumeCatalogLevel = 1;

    public static PVolumeCatalog PVolumeCatalogCurrent { get; } = new();

    public double PVolumeCatalogLevel
    {
        get => _pVolumeCatalogLevel;
        set
        {
            double chosen = value < 0 ? 0 : value > 1 ? 1 : value;
            if (_pVolumeCatalogLevel == chosen)
            {
                return;
            }

            _pVolumeCatalogLevel = chosen;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PVolumeCatalogLevel)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
