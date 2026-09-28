using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    private Popup PHeadquarter => (Popup)_pWindowSurface.FindName(nameof(PHeadquarter));

    private void PLogoHandle(object sender, MouseButtonEventArgs e)
    {
        LHeadquarter.LHeadquarterMenuRefine(PHeadquarter, e);
    }

    private void PHeadquarterAboutHandle(object sender, MouseButtonEventArgs e)
    {
        LHeadquarter.LHeadquarterAboutRefine(
            _pWindowSurface,
            PHeadquarter,
            e,
            CAtelier.CAtelierAboutRead());
    }

    private void PHeadquarterExitHandle(object sender, MouseButtonEventArgs e)
    {
        LHeadquarter.LHeadquarterExitRefine(_pWindowSurface, PHeadquarter, e);
    }
}
