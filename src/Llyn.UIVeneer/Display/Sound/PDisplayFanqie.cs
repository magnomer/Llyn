using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayFanqieStart(long id)
    {
        try
        {
            _lLectern.LLecternFanqieStart(id);
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
            groups = _lLectern.LLecternFanqieDivide(id);
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

        LReflexItem.LReflexAnchorApply(_pDisplayHost.PWindowDeportment, _pDisplayReflex, rows, PDisplayHeadword.Text);
        LFontFace.LFontApply(_pDisplayHost.PWindowDeportment, language, LFontRole.LFontRoleGlyph, PDisplayFanqie);
        PDisplayFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(groups);
        PDisplayFanqie.PFanqiePending = _lLectern.LLecternFanqieCheck(id);
        PDisplayFanqie.PFanqieDiweiNotice = (kind, key) => _pDisplayHost.PWindowDiweiShow(language, kind, key);
        PDisplayFanqie.PFanqieStemNotice = key => _pDisplayHost.PWindowStemShow(language, key);
        PDisplayFanqie.PFanqieRepresentativeNotice = (fanqieId, rank) => PDisplayFanqieSet(id, fanqieId, rank);
        PDisplayReadingShow(id);
    }

    private void PDisplayReadingShow(long id)
    {
        try
        {
            PDisplayReading.Text = _lLectern.LLecternReadingRead(id, PDisplayHeadword.Text);
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
            _lLectern.LLecternFanqieSet(id, fanqieId, rank);
        }
        catch (Exception)
        {
        }
    }

    private void PDisplayFanqieClear()
    {
        PDisplayFanqie.PFanqieItems = null;
        PDisplayFanqie.PFanqiePending = _lLectern.LLecternFanqieCheck(null);
        PDisplayFanqie.PFanqieDiweiNotice = null;
        PDisplayFanqie.PFanqieStemNotice = null;
        PDisplayFanqie.PFanqieRepresentativeNotice = null;
        PDisplayReading.Text = string.Empty;
    }
}
