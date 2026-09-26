using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace Llyn.UIDeportment;

internal sealed class PGamutItem : INotifyPropertyChanged
{
    private static readonly Dictionary<string, string> PGamutItemIcons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["archaic"] = "register/archaic",
        ["epic"] = "register/epic",
        ["formal"] = "register/formal",
        ["impolite"] = "register/impolite",
        ["informal"] = "register/informal",
        ["poetic"] = "register/poetic",
        ["polite"] = "register/polite",
    };

    private bool _pGamutItemChosen;

    internal PGamutItem(long id, string name, int usage, bool chosen)
    {
        PGamutItemId = id;
        PGamutItemName = name;
        PGamutItemIcon = PIcon.PIconResolve(PGamutItemIcons.GetValueOrDefault(name.Trim(), "register"), 32);
        PGamutItemUsage = usage;
        _pGamutItemChosen = chosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long PGamutItemId { get; }

    public string PGamutItemName { get; }

    public ImageSource PGamutItemIcon { get; }

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
