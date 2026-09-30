using System.Globalization;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QGamutItem
{
    internal QGamutItem(CCatalogRegister row)
    {
        QGamutItemId = row.CCatalogRegisterStored.CRegisterId;
        QGamutItemName = row.CCatalogRegisterStored.CRegisterName;
        QGamutItemIcon = QIcon.QIconResolve(row.CCatalogRegisterIcon, 32);
        QGamutItemUsage = row.CCatalogRegisterUsage;
        QGamutItemChosen = row.CCatalogRegisterChosen;
    }

    public long QGamutItemId { get; }

    public string QGamutItemName { get; }

    public ImageSource QGamutItemIcon { get; }

    public int QGamutItemUsage { get; }

    public bool QGamutItemChosen { get; }

    public string QGamutItemCount => QGamutItemUsage.ToString(CultureInfo.CurrentCulture);
}
