using System;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCadence
{
    private readonly PEditor _pEditor;

    private PWindow _pWindow = null!;

    private LEditor _lEditor = null!;

    internal QCadence(PEditor editor)
    {
        _pEditor = editor;
    }

    private TextBlock QCadenceReading => QContract.QContractFind<TextBlock>(_pEditor, "PEditorReading");

    private PParadigm QCadenceParadigm => QContract.QContractFind<PParadigm>(_pEditor, "PEditorParadigm");

    private PFanqie QCadenceFanqie => QContract.QContractFind<PFanqie>(_pEditor, "PEditorFanqie");

    private PScript QCadenceScript => QContract.QContractFind<PScript>(_pEditor, "PEditorScript");

    internal void QCadenceAttach(PWindow host, LEditor editor)
    {
        _pWindow = host;
        _lEditor = editor;
    }

    internal void QCadenceParadigmUpdate()
    {
        LFontFace.LFontRefine(
            _pWindow.PWindowAtelier,
            _lEditor.LEditorStudio.CEditorSounding.CSoundingLanguageRead(),
            CFontRole.CFontRoleHeadword,
            QCadenceParadigm);
        QCadenceParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(
            _lEditor.LEditorStudio.CEditorSounding.CSoundingParadigmRead(),
            _lEditor.LEditorStudio.CEditorTimbre.CTimbreParadigmPending,
            _lEditor.LEditorMorphology,
            true);
    }

    internal void QCadenceScriptUpdate()
    {
        LFontFace.LFontRefine(
            _pWindow.PWindowAtelier, _lEditor.LEditorLanguage, CFontRole.CFontRoleGlyph, QCadenceScript);
        QCadenceScript.PScriptItems =
            PScriptItem.PScriptItemScan(_lEditor.LEditorStudio.CEditorSounding.CSoundingScriptRead());
        QCadenceScript.PScriptPending = _lEditor.LEditorStudio.CEditorTimbre.CTimbreScriptPending;
        QCadenceScript.PScriptRenewal =
            QLook.QLookFirstRead<Action?>(
                _lEditor.LEditorStudio.CEditorTimbre.CTimbreScriptRebuildable,
                _lEditor.LEditorStudio.CEditorSounding.CSoundingScriptResolve,
                null);
    }

    internal void QCadenceFanqieUpdate()
    {
        LFontFace.LFontRefine(
            _pWindow.PWindowAtelier, _lEditor.LEditorLanguage, CFontRole.CFontRoleGlyph, QCadenceFanqie);
        QCadenceFanqie.PFanqieItems =
            PFanqieItem.PFanqieItemScan(_lEditor.LEditorStudio.CEditorSounding.CSoundingFanqieRead());
        QCadenceFanqie.PFanqiePending = _lEditor.LEditorStudio.CEditorTimbre.CTimbreFanqiePending;
        QCadenceFanqie.PFanqieRenewal =
            QLook.QLookFirstRead<Action?>(
                _lEditor.LEditorStudio.CEditorTimbre.CTimbreFanqieRebuildable,
                _lEditor.LEditorStudio.CEditorSounding.CSoundingFanqieResolve,
                null);
        QCadenceFanqie.PFanqieDiweiNotice =
            (kind, key) => _pWindow.PWindowAtelier.CAtelierNavigation.CNavigationDiweiOpen(
                _lEditor.LEditorLanguage, kind, key);
        QCadenceFanqie.PFanqieRepresentativeNotice =
            (fanqieId, rank) => _lEditor.LEditorStudio.CEditorSounding.CSoundingFanqieSet(fanqieId, rank);
    }

    internal void QCadenceReadingShow(string headword)
    {
        QCadenceReading.Text = _lEditor.LEditorStudio.CEditorSounding.CSoundingReadingRead(headword);
    }
}
