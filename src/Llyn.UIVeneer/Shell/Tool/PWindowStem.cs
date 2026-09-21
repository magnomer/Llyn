namespace Llyn.UIVeneer;

public partial class PWindow
{
    internal void PWindowStemShow(string language, string? key)
    {
        PWindowStationOpen(
            PNavigationXiesheng, PXiesheng.PXieshengLeaveConfirm, () => PXiesheng.PXieshengStemShow(language, key));
    }
}
