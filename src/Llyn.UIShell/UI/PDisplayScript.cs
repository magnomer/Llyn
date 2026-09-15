using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayScriptStart(long id)
    {
        try
        {
            _lEngine.LEngineScriptStart(id);
        }
        catch (Exception)
        {
        }
    }

    private void PDisplayScriptShow(long id, string language)
    {
        IReadOnlyList<LScriptImage> images;
        IReadOnlyList<LScriptStyle> styles;
        bool pending;
        try
        {
            styles = _lEngine.LEngineStyleRead(language);
            images = styles.Count == 0 ? [] : _lEngine.LEngineScriptRead(id);
            pending = styles.Count > 0 && _lEngine.LEngineScriptCheck(id);
        }
        catch (Exception)
        {
            styles = [];
            images = [];
            pending = false;
        }

        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PDisplayScript);
        PDisplayScript.PScriptItems = PScriptItem.PScriptItemScan(images, styles);
        PDisplayScript.PScriptPending = pending;
    }

    private void PDisplayScriptClear()
    {
        PDisplayScript.PScriptItems = null;
        PDisplayScript.PScriptPending = false;
    }
}
