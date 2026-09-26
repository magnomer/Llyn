using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private Popup PHeadquarter => (Popup)_pWindowSurface.FindName(nameof(PHeadquarter));

    private void PLogoHandle(object sender, MouseButtonEventArgs e)
    {
        LHeadquarter.LHeadquarterToggle(PHeadquarter, e);
    }

    private void PHeadquarterAboutHandle(object sender, MouseButtonEventArgs e)
    {
        LHeadquarter.LHeadquarterAboutHandle(
            _pWindowSurface,
            PHeadquarter,
            e,
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            PLocalizationCatalog.PLocalizationTextRead("Headquarter.Version"));
    }

    private void PHeadquarterExitHandle(object sender, MouseButtonEventArgs e)
    {
        LHeadquarter.LHeadquarterExitHandle(_pWindowSurface, PHeadquarter, e);
    }
}
