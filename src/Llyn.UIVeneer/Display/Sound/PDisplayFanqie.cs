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
            _lDisplay.LDisplayFanqieStart(id);
        }
        catch (Exception)
        {
        }
    }

    private void PDisplayFanqieShow(long id, string language)
    {
        IReadOnlyList<LFanqieGroup> groups;
        try
        {
            groups = _lDisplay.LDisplayFanqieDivide(id);
        }
        catch (Exception)
        {
            groups = [];
        }

        List<LFanqieRow> rows = [];
        foreach (LFanqieGroup group in groups)
        {
            rows.AddRange(group.LFanqieGroupRows);
        }

        _pDisplayFanqie = rows;
        PReflexAnchorApply(_pDisplayReflex, rows, PDisplayHeadword.Text);
        PFont.PFontApply(_pDisplayHost.PWindowDeportment, language, LFontRole.LFontRoleGlyph, PDisplayFanqie);
        PDisplayFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(groups);
        PDisplayFanqie.PFanqiePending = _lDisplay.LDisplayFanqieCheck(id);
        PDisplayFanqie.PFanqieDiweiNotice = (kind, key) => _pDisplayHost.PWindowDiweiShow(language, kind, key);
        PDisplayFanqie.PFanqieStemNotice = key => _pDisplayHost.PWindowStemShow(language, key);
        PDisplayFanqie.PFanqieRepresentativeNotice = (fanqieId, rank) => PDisplayFanqieSet(id, fanqieId, rank);
        PDisplayReadingShow(id);
    }

    private void PDisplayReadingShow(long id)
    {
        try
        {
            PDisplayReading.Text = _lDisplay.LDisplayReadingRead(id, PDisplayHeadword.Text);
        }
        catch (Exception)
        {
            PDisplayReading.Text = string.Empty;
        }
    }

    private void PDisplayFanqieSet(long id, long fanqieId, int rank)
    {
        try
        {
            _lDisplay.LDisplayFanqieSet(id, fanqieId, rank);
        }
        catch (Exception)
        {
        }
    }

    private void PDisplayFanqieClear()
    {
        _pDisplayFanqie = [];
        PDisplayFanqie.PFanqieItems = null;
        PDisplayFanqie.PFanqiePending = _lDisplay.LDisplayFanqieCheck(null);
        PDisplayFanqie.PFanqieDiweiNotice = null;
        PDisplayFanqie.PFanqieStemNotice = null;
        PDisplayFanqie.PFanqieRepresentativeNotice = null;
        PDisplayReading.Text = string.Empty;
    }
}
