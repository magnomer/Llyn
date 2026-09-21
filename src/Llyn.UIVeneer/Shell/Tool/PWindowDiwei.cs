namespace Llyn.UIVeneer;

public partial class PWindow
{
    internal void PWindowDiweiShow(string language, string kind, string key)
    {
        PWindowStationOpen(
            PNavigationYunjing,
            PYunjing.PYunjingLeaveConfirm,
            () => PYunjing.PYunjingDiweiShow(language, kind, key));
    }
}
