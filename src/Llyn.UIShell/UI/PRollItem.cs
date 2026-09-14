using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PRollItem : INotifyPropertyChanged
{
    private bool _pRollItemChosen;

    internal PRollItem(LCatalogAuthor row, string work)
    {
        PRollItemId = row.LCatalogAuthorStored.LAuthorId;
        PRollItemName = row.LCatalogAuthorStored.LAuthorName;
        PRollItemWork = work;
        PRollItemCount = row.LCatalogAuthorUsage.ToString(CultureInfo.CurrentCulture);
        PRollItemMark = PIcon.PIconResolve("guild", 16);
    }

    internal PRollItem(string name, string work, int usage)
    {
        PRollItemId = 0;
        PRollItemName = name;
        PRollItemWork = work;
        PRollItemCount = usage.ToString(CultureInfo.CurrentCulture);
        PRollItemMark = PIcon.PIconResolve("unlink", 16);
    }

    public long PRollItemId { get; }

    public string PRollItemName { get; }

    public string PRollItemWork { get; }

    public string PRollItemCount { get; }

    public Geometry PRollItemMark { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PRollItemChosen
    {
        get => _pRollItemChosen;

        set
        {
            if (_pRollItemChosen == value)
            {
                return;
            }

            _pRollItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PRollItemChosen)));
        }
    }
}
