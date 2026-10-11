using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QSGalleyItem : INotifyPropertyChanged
{
    internal QSGalleyItem(int index, double width, double height)
    {
        QSGalleyItemIndex = index;
        QSGalleyItemWidth = width;
        QSGalleyItemHeight = height;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public int QSGalleyItemIndex { get; }

    public string QSGalleyItemNumber => (QSGalleyItemIndex + 1).ToString(CultureInfo.CurrentCulture);

    public double QSGalleyItemWidth { get; }

    public double QSGalleyItemHeight { get; }

    public ImageSource? QSGalleyItemPicture
    {
        get;
        set
        {
            if (!ReferenceEquals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QSGalleyItemPicture)));
            }
        }
    }

    public bool QSGalleyItemShaded
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QSGalleyItemShaded)));
            }
        }
    }
}
