using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

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
        IReadOnlyList<LScriptImage> images;
        IReadOnlyList<LScriptStyle> styles;
        bool pending;
        try
        {
            styles = _lEngine.LEngineStyleRead(language);
            if (styles.Count > 0)
            {
                _lEngine.LEngineScriptStart(entry.Value);
            }

            images = styles.Count == 0 ? [] : _lEngine.LEngineScriptRead(entry.Value);
            pending = styles.Count > 0 && _lEngine.LEngineScriptCheck(entry.Value);
        }
        catch (Exception)
        {
            styles = [];
            images = [];
            pending = false;
        }

        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PEditorScript);
        PEditorScript.PScriptItems = PScriptItem.PScriptItemScan(images, styles);
        PEditorScript.PScriptPending = pending;
    }
}
