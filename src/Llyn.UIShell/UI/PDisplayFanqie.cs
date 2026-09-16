using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

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
        IReadOnlyList<LFanqieRow> rows;
        IReadOnlyList<LFanqieBook> books;
        bool pending;
        try
        {
            books = _lEngine.LEngineBookRead(language);
            rows = books.Count == 0 ? [] : _lEngine.LEngineFanqieRead(id);
            pending = books.Count > 0 && _lEngine.LEngineFanqieCheck(id);
        }
        catch (Exception)
        {
            books = [];
            rows = [];
            pending = false;
        }

        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PDisplayFanqie);
        PDisplayFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(rows, books);
        PDisplayFanqie.PFanqiePending = pending;
        PDisplayFanqie.PFanqieDiweiNotice = (kind, key) => _pDisplayHost.PWindowDiweiShow(language, kind, key);
    }

    private void PDisplayFanqieClear()
    {
        PDisplayFanqie.PFanqieItems = null;
        PDisplayFanqie.PFanqiePending = false;
        PDisplayFanqie.PFanqieDiweiNotice = null;
    }
}
