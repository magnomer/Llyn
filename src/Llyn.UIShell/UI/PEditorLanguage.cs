using System.Windows;

namespace Llyn.UIShell;

public partial class PEditor
{
    private void PEditorLanguageShow(string language)
    {
        if (language.Length == 0)
        {
            return;
        }

        PSpeakerName.Text = language;
        PSpeakerFlagUpdate();
        PHeadwordFontApply(language);
        PEditorContourApply(language);
        PEditorSilentApply(language);
        PEditorExampleShow(language);
        PSentenceFrameLoad(language);
        PCategoryLoad();
    }

    private void PEditorExampleShow(string language)
    {
        PFont.PFontExampleApply(Resources, _lEngine, language);
    }

    private void PEditorContourApply(string language)
    {
        PContour.PContourTonal = _lEngine.LEngineTonalCheck(language);
    }

    private void PEditorSilentApply(string language)
    {
        Visibility shown = _lEngine.LEngineSilentCheck(language) ? Visibility.Collapsed : Visibility.Visible;
        PPronunciation.Visibility = shown;
        PAccent.Visibility = shown;
    }
}
