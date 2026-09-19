using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    internal void PEditorScriptShow()
    {
        long? entry = PEditorEntryRead();
        if (entry is null)
        {
            PEditorScript.PScriptItems = null;
            PEditorScript.PScriptPending = false;
            return;
        }

        string language = PSpeakerLanguageRead();
        IReadOnlyList<LScriptGroup> groups;
        bool pending;
        try
        {
            _lEngine.LEngineScriptStart(entry.Value);
            groups = _lEngine.LEngineScriptDivide(entry.Value);
            pending = _lEngine.LEngineScriptCheck(entry.Value);
        }
        catch (Exception)
        {
            groups = [];
            pending = false;
        }

        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PEditorScript);
        PEditorScript.PScriptItems = PScriptItem.PScriptItemScan(groups);
        PEditorScript.PScriptPending = pending;
    }
}
