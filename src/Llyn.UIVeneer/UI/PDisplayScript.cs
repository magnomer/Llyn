using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

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
        IReadOnlyList<LScriptGroup> groups;
        try
        {
            groups = _lEngine.LEngineScriptDivide(id);
        }
        catch (Exception)
        {
            groups = [];
        }

        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PDisplayScript);
        PDisplayScript.PScriptItems = PScriptItem.PScriptItemScan(groups);
        PDisplayScript.PScriptPending = _lDisplay.LDisplayScriptCheck(id);
    }

    private void PDisplayScriptClear()
    {
        PDisplayScript.PScriptItems = null;
        PDisplayScript.PScriptPending = _lDisplay.LDisplayScriptCheck(null);
    }
}
