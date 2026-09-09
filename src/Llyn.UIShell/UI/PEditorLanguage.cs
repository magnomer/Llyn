using System;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    private void PEditorLanguageShow(string language)
    {
        PEditorExampleShow(language);

        if (language.Length == 0)
        {
            return;
        }

        _pSpeakerEntry = true;
        PHeadwordFontApply(language);

        if (string.Equals(_pSpeakerChoice, language, StringComparison.Ordinal))
        {
            return;
        }

        _pSpeakerChoice = language;
        PSpeakerName.Text = language;
        PSpeakerFlagUpdate();
    }

    private void PEditorExampleShow(string language)
    {
        LFont font = PFont.PFontExampleRead(_lEngine, language);

        if (font.LFontFamily is string named)
        {
            Resources["Theme.Card.ExampleFamily"] = new FontFamily(named);
        }
        else
        {
            Resources.Remove("Theme.Card.ExampleFamily");
        }

        if (font.LFontSize > 0)
        {
            Resources["Theme.Card.ExampleSize"] = font.LFontSize;
        }
        else
        {
            Resources.Remove("Theme.Card.ExampleSize");
        }
    }
}
