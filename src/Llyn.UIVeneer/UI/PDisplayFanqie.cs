using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayFanqieStart(long id)
    {
        try
        {
            _lEngine.LEngineFanqieStart(id);
        }
        catch (Exception)
        {
        }
    }

    private void PDisplayFanqieShow(long id, string language)
    {
        IReadOnlyList<LFanqieGroup> groups;
        bool pending;
        try
        {
            groups = _lEngine.LEngineFanqieDivide(id);
            pending = _lEngine.LEngineFanqieCheck(id);
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

        _pDisplayFanqie = rows;
        PReflexAnchorApply(_pDisplayReflex, rows, PDisplayHeadword.Text);
        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PDisplayFanqie);
        PDisplayFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(groups);
        PDisplayFanqie.PFanqiePending = pending;
        PDisplayFanqie.PFanqieDiweiNotice = (kind, key) => _pDisplayHost.PWindowDiweiShow(language, kind, key);
    }

    private void PDisplayFanqieClear()
    {
        _pDisplayFanqie = [];
        PDisplayFanqie.PFanqieItems = null;
        PDisplayFanqie.PFanqiePending = false;
        PDisplayFanqie.PFanqieDiweiNotice = null;
    }
}
