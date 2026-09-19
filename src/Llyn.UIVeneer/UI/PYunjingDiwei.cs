using System;
using System.Collections.Generic;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PYunjing
{
    private LDiwei? _pDiweiShown;

    private void PDiweiAttach()
    {
        PYunjingDiwei.PDiweiAttach(_lEngine);
        PYunjingDiwei.PDiweiEntryNotice = PDiweiEntryShow;
        PYunjingDiwei.PDiweiSwitchNotice = PTallyChange;
    }

    private void PTallyChange(bool respelled)
    {
        _lEngine.LEngineTallySave(respelled);
        PDiweiLoad();
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
        IReadOnlyList<LTally> tallies;
        bool switched;
        try
        {
            rows = _lEngine.LEngineFanqieRead(diwei);
            hypothesis = _lEngine.LEngineHypothesisRead(diwei.LDiweiLanguage);
            tallies = _lEngine.LEngineTallyRead(diwei);
            switched = _lEngine.LEngineRespellingCheck(diwei.LDiweiLanguage);
        }
        catch (Exception exception)
        {
            _pYunjingHost.PWindowFailureShow(PYunjingFailure, exception);
            return;
        }

        bool respelled = switched && _lEngine.LEngineSettingsRead().LSettingsTally;
        PYunjingDiwei.PDiweiApply(
            diwei,
            PDiweiItem.PDiweiItemScan(
                diwei.LDiweiKind, diwei.LDiweiFinal, rows, hypothesis, tallies, switched, respelled));
    }

    private void PDiweiEntryShow(string character)
    {
        if (_pDiweiShown is LDiwei diwei)
        {
            if (PYunjingLeaveConfirm())
            {
                _pYunjingHost.PWindowGlyphShow(character, diwei.LDiweiLanguage);
            }
        }
    }
}
