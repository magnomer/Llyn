using System;
using System.Collections.Generic;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PYunjing
{
    private LDiwei? _pDiweiShown;

    private void PDiweiAttach()
    {
        PYunjingDiwei.PDiweiAttach(_lEngine);
        PYunjingDiwei.PDiweiEntryNotice = PDiweiEntryShow;
    }

    private LDiwei? PDiweiFind(string kind, string key)
    {
        if (_pYunjingLanguage is not string language)
        {
            return null;
        }

        try
        {
            return _lEngine.LEngineDiweiFind(language, kind, key);
        }
        catch (Exception exception)
        {
            _pYunjingHost.PWindowFailureShow(PYunjingFailure, exception);
            return null;
        }
    }

    private void PDiweiShow(LDiwei diwei)
    {
        PYunjingClear();
        _pDiweiShown = diwei;
        PDisplay.Visibility = Visibility.Collapsed;
        PYunjingDiwei.Visibility = Visibility.Visible;
        PDiweiLoad();
    }

    private void PDiweiHide()
    {
        _pDiweiShown = null;
        PYunjingDiwei.PDiweiClear();
        PYunjingDiwei.Visibility = Visibility.Collapsed;
        if (PEditor.Visibility != Visibility.Visible)
        {
            PDisplay.Visibility = Visibility.Visible;
        }
    }

    private void PDiweiLoad()
    {
        if (_pDiweiShown is not LDiwei diwei)
        {
            return;
        }

        IReadOnlyList<LFanqieRow> rows;
        LHypothesis? hypothesis;
        try
        {
            rows = _lEngine.LEngineFanqieRead(diwei);
            hypothesis = _lEngine.LEngineHypothesisRead(diwei.LDiweiLanguage);
        }
        catch (Exception exception)
        {
            _pYunjingHost.PWindowFailureShow(PYunjingFailure, exception);
            return;
        }

        PYunjingDiwei.PDiweiApply(diwei, PDiweiItem.PDiweiItemScan(rows, hypothesis));
    }

    private void PDiweiEntryShow(string character)
    {
        if (_pDiweiShown is LDiwei diwei && PYunjingLeaveConfirm())
        {
            _pYunjingHost.PWindowGlyphShow(character, diwei.LDiweiLanguage);
        }
    }
}
