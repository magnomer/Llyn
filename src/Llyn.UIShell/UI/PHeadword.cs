namespace Llyn.UIShell;

public partial class PEditor
{
    private void PHeadwordFontApply(string language)
    {
        PFont.PFontApply(_lEngine, language, PHeadword, PHeadwordHint, PHeadwordGhost);
    }
}
