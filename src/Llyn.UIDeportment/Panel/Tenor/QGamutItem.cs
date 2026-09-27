using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class QGamutItem : INotifyPropertyChanged
{
    private static readonly Dictionary<string, string> QGamutItemIcons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["archaic"] = "register/archaic",
        ["epic"] = "register/epic",
        ["formal"] = "register/formal",
        ["impolite"] = "register/impolite",
        ["informal"] = "register/informal",
        ["poetic"] = "register/poetic",
        ["polite"] = "register/polite",
    };

    private bool _qGamutItemChosen;

    internal QGamutItem(long id, string name, int usage, bool chosen)
    {
        QGamutItemId = id;
        QGamutItemName = name;
        QGamutItemIcon = QIcon.QIconResolve(QGamutItemIcons.GetValueOrDefault(name.Trim(), "register"), 32);
        QGamutItemUsage = usage;
        _qGamutItemChosen = chosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long QGamutItemId { get; }

    public string QGamutItemName { get; }

    public ImageSource QGamutItemIcon { get; }

    public int QGamutItemUsage { get; }

    public bool QGamutItemChosen
    {
        get => _qGamutItemChosen;

        set
        {
            if (_qGamutItemChosen == value)
            {
                return;
            }

            _qGamutItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QGamutItemChosen)));
        }
    }

    public string QGamutItemCount => QGamutItemUsage.ToString(CultureInfo.CurrentCulture);
}
