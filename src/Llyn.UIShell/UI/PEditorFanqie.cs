using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor
{
    internal void PEditorFanqieShow()
    {
        long? entry = PEditorEntryRead();
        PEditorFanqie.PFanqieRebuildNotice = null;
        if (entry is null)
        {
            PEditorFanqie.PFanqieItems = null;
            PEditorFanqie.PFanqiePending = false;
            return;
        }

        string language = _pSpeakerChoice;
        IReadOnlyList<LFanqieRow> rows;
        IReadOnlyList<LFanqieBook> books;
        LHypothesis? hypothesis;
        bool pending;
        try
        {
            books = _lEngine.LEngineBookRead(language);
            hypothesis = _lEngine.LEngineHypothesisRead(language);
            if (books.Count > 0)
            {
                _lEngine.LEngineFanqieStart(entry.Value);
            }

            rows = books.Count == 0 ? [] : _lEngine.LEngineFanqieRead(entry.Value);
            pending = books.Count > 0 && _lEngine.LEngineFanqieCheck(entry.Value);
        }
        catch (Exception)
        {
            books = [];
            hypothesis = null;
            rows = [];
            pending = false;
        }

        PFont.PFontApply(_lEngine, language, LFontRole.LFontRoleGlyph, PEditorFanqie);
        PEditorFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(rows, books, hypothesis);
        PEditorFanqie.PFanqiePending = pending;
        PEditorFanqie.PFanqieRebuildNotice = books.Count == 0 ? null : () => PEditorFanqieRebuild(entry.Value);
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
