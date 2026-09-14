using System;

namespace Llyn.UIShell;

public partial class PEditor
{
    private void PEditorLanguageShow(string language)
    {
        if (language.Length == 0 || string.Equals(_pSpeakerChoice, language, StringComparison.Ordinal))
        {
            return;
        }

        _pSpeakerEntry = true;
        _pSpeakerChoice = language;
        PSpeakerName.Text = language;
        PSpeakerFlagUpdate();
        PHeadwordFontApply(language);
        PEditorContourApply(language);
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
}
