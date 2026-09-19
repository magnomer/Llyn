namespace Llyn.UIVeneer;

public partial class PEditor
{
    private void PHeadwordFontApply(string language)
    {
        PFont.PFontApply(_lEngine, language, PHeadword, PHeadwordHint, PHeadwordGhost);
        PFont.PFontPlace(PHeadword, PHeadwordHint, PHeadwordGhost);
    }
}
