using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayScriptStart(long id)
    {
        try
        {
            _lLectern.LLecternScriptStart(id);
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
            groups = _lLectern.LLecternScriptDivide(id);
        }
        catch (Exception)
        {
            groups = [];
        }

        LFontFace.LFontApply(_pDisplayHost.PWindowDeportment, language, LFontRole.LFontRoleGlyph, PDisplayScript);
        PDisplayScript.PScriptItems = PScriptItem.PScriptItemScan(groups);
        PDisplayScript.PScriptPending = _lLectern.LLecternScriptCheck(id);
    }

    private void PDisplayScriptClear()
    {
        PDisplayScript.PScriptItems = null;
        PDisplayScript.PScriptPending = _lLectern.LLecternScriptCheck(null);
    }
}
