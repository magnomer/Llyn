using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    internal void PEditorFanqieShow()
    {
        long? entry = PEditorEntryRead();
        PEditorFanqie.PFanqieRebuildNotice = null;
        PEditorFanqie.PFanqieDiweiNotice = null;
        if (entry is null)
        {
            _pReflexFanqie = [];
            PReflexAnchorShow();
            PEditorFanqie.PFanqieItems = null;
            PEditorFanqie.PFanqiePending = false;
            return;
        }

        string language = PSpeakerLanguageRead();
        IReadOnlyList<LFanqieGroup> groups;
        bool pending;
        try
        {
            _lEngine.LEngineFanqieStart(entry.Value);
            groups = _lEngine.LEngineFanqieDivide(entry.Value);
            pending = _lEngine.LEngineFanqieCheck(entry.Value);
        }
        catch (Exception)
        {
            groups = [];
            pending = false;
        }

        List<LFanqieRow> rows = [];
        foreach (LFanqieGroup group in groups)
        {
            rows.AddRange(group.LFanqieGroupRows);
        }

        _pReflexFanqie = rows;
        PReflexAnchorShow();
        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PEditorFanqie);
        PEditorFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(groups);
        PEditorFanqie.PFanqiePending = pending;
        PEditorFanqie.PFanqieRebuildNotice = _lEngine.LEngineBookCheck(language)
            ? () => PEditorFanqieRebuild(entry.Value)
            : null;
        PEditorFanqie.PFanqieDiweiNotice = (kind, key) => _pEditorHost.PWindowDiweiShow(language, kind, key);
    }

    private void PEditorFanqieRebuild(long entry)
    {
        try
        {
            _lEngine.LEngineFanqieRebuild(entry);
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Display.FanqieRebuildFailed", exception);
            return;
        }

        PEditorFanqieShow();
    }
}
