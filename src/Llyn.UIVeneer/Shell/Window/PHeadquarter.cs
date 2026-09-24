using System.Windows.Input;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PWindow
{
    private void PLogoHandle(object sender, MouseButtonEventArgs e)
    {
        LHeadquarter.LHeadquarterToggle(PHeadquarter, e);
    }

    private void PHeadquarterAboutHandle(object sender, MouseButtonEventArgs e)
    {
        LHeadquarter.LHeadquarterAboutHandle(
            this,
            PHeadquarter,
            e,
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            PLocalizationCatalog.PLocalizationTextRead("Headquarter.Version"));
    }

    private void PHeadquarterExitHandle(object sender, MouseButtonEventArgs e)
    {
        LHeadquarter.LHeadquarterExitHandle(this, PHeadquarter, e);
    }
}
