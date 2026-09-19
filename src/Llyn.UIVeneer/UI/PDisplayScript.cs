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
        bool pending;
        try
        {
            groups = _lEngine.LEngineScriptDivide(id);
            pending = _lEngine.LEngineScriptCheck(id);
        }
        catch (Exception)
        {
            groups = [];
            pending = false;
        }

        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PDisplayScript);
        PDisplayScript.PScriptItems = PScriptItem.PScriptItemScan(groups);
        PDisplayScript.PScriptPending = pending;
    }

    private void PDisplayScriptClear()
    {
        PDisplayScript.PScriptItems = null;
        PDisplayScript.PScriptPending = false;
    }
}
