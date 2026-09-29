using System;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCadence
{
    private readonly PEditor _pEditor;

    private PWindow _pWindow = null!;

    private CEditor _cEditor = null!;

    internal QCadence(PEditor editor)
    {
        _pEditor = editor;
    }

    private TextBlock QCadenceReading => QContract.QContractFind<TextBlock>(_pEditor, "PEditorReading");

    private PParadigm QCadenceParadigm => QContract.QContractFind<PParadigm>(_pEditor, "PEditorParadigm");

    private PFanqie QCadenceFanqie => QContract.QContractFind<PFanqie>(_pEditor, "PEditorFanqie");

    private PScript QCadenceScript => QContract.QContractFind<PScript>(_pEditor, "PEditorScript");

    internal void QCadenceIntroduce(PWindow host, CEditor editor)
    {
        _pWindow = host;
        _cEditor = editor;
        editor.CEditorDesk.CDeskStarted += QCadenceParadigmUpdate;
        editor.CEditorDesk.CDeskStarted += QCadenceScriptUpdate;
        editor.CEditorDesk.CDeskStarted += QCadenceFanqieUpdate;
        editor.CEditorTimbre.CTimbreParadigmChanged += QCadenceParadigmUpdate;
        editor.CEditorTimbre.CTimbreScriptChanged += QCadenceScriptUpdate;
        editor.CEditorSounding.CSoundingChanged += QCadenceFanqieUpdate;
    }

    private void QCadenceParadigmUpdate()
    {
        LFontFace.LFontRefine(
            _pWindow.PWindowAtelier,
            _cEditor.CEditorSounding.CSoundingLanguageRead(),
            CFontRole.CFontRoleHeadword,
            QCadenceParadigm);
        QCadenceParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(
            _cEditor.CEditorSounding.CSoundingParadigmRead(),
            _cEditor.CEditorTimbre.CTimbreParadigmPending,
            _cEditor.CEditorTimbre.CTimbreMorphology,
            true);
    }

    private void QCadenceScriptUpdate()
    {
        LFontFace.LFontRefine(
            _pWindow.PWindowAtelier, _cEditor.CEditorLanguage, CFontRole.CFontRoleGlyph, QCadenceScript);
        QCadenceScript.PScriptItems =
            PScriptItem.PScriptItemScan(_cEditor.CEditorSounding.CSoundingScriptRead());
        QCadenceScript.PScriptPending = _cEditor.CEditorTimbre.CTimbreScriptPending;
        QCadenceScript.PScriptRenewal =
            QLook.QLookFirstRead<Action?>(
                _cEditor.CEditorTimbre.CTimbreScriptRebuildable,
                _cEditor.CEditorSounding.CSoundingScriptResolve,
                null);
    }

    private void QCadenceFanqieUpdate()
    {
        LFontFace.LFontRefine(
            _pWindow.PWindowAtelier, _cEditor.CEditorLanguage, CFontRole.CFontRoleGlyph, QCadenceFanqie);
        QCadenceFanqie.PFanqieItems =
            PFanqieItem.PFanqieItemScan(_cEditor.CEditorSounding.CSoundingFanqieRead());
        QCadenceFanqie.PFanqiePending = _cEditor.CEditorTimbre.CTimbreFanqiePending;
        QCadenceFanqie.PFanqieRenewal =
            QLook.QLookFirstRead<Action?>(
                _cEditor.CEditorTimbre.CTimbreFanqieRebuildable,
                _cEditor.CEditorSounding.CSoundingFanqieResolve,
                null);
        QCadenceFanqie.PFanqieDiweiNotice =
            (kind, key) => _pWindow.PWindowAtelier.CAtelierNavigation.CNavigationDiweiOpen(
                _cEditor.CEditorLanguage, kind, key);
        QCadenceFanqie.PFanqieRepresentativeNotice =
            (fanqieId, rank) => _cEditor.CEditorSounding.CSoundingFanqieSet(fanqieId, rank);
    }

    internal void QCadenceReadingShow(string headword)
    {
        QCadenceReading.Text = _cEditor.CEditorSounding.CSoundingReadingRead(headword);
    }
}
