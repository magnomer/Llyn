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
            _lEditor.LEditorTimbre.QTimbreParadigmLanguage,
            CFontRole.CFontRoleHeadword,
            QCadenceParadigm);
        QCadenceParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(
            _lEditor.LEditorTimbre.QTimbreParadigmRead(), _lEditor.LEditorTimbre.QTimbreParadigmPending,
            _lEditor.LEditorMorphology, true);
    }

    internal void QCadenceScriptUpdate()
    {
        LFontFace.LFontRefine(
            _pWindow.PWindowAtelier, _lEditor.LEditorLanguage, CFontRole.CFontRoleGlyph, QCadenceScript);
        QCadenceScript.PScriptItems = PScriptItem.PScriptItemScan(_lEditor.LEditorTimbre.QTimbreScriptRead());
        QCadenceScript.PScriptPending = _lEditor.LEditorTimbre.QTimbreScriptPending;
        QCadenceScript.PScriptRenewal =
            QLook.QLookFirstRead<Action?>(
                _lEditor.LEditorTimbre.QTimbreScriptRebuildable, _lEditor.LEditorTimbre.QTimbreScriptRebuild, null);
    }

    internal void QCadenceFanqieUpdate()
    {
        LFontFace.LFontRefine(
            _pWindow.PWindowAtelier, _lEditor.LEditorLanguage, CFontRole.CFontRoleGlyph, QCadenceFanqie);
        QCadenceFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(_lEditor.LEditorTimbre.QTimbreFanqieRead());
        QCadenceFanqie.PFanqiePending = _lEditor.LEditorTimbre.QTimbreFanqiePending;
        QCadenceFanqie.PFanqieRenewal =
            QLook.QLookFirstRead<Action?>(
                _lEditor.LEditorTimbre.QTimbreFanqieRebuildable, _lEditor.LEditorTimbre.QTimbreFanqieRebuild, null);
        QCadenceFanqie.PFanqieDiweiNotice =
            (kind, key) => _pWindow.PWindowDiweiShow(_lEditor.LEditorLanguage, kind, key);
        QCadenceFanqie.PFanqieRepresentativeNotice =
            (fanqieId, rank) => _lEditor.LEditorTimbre.QTimbreFanqieSet(fanqieId, rank);
    }

    internal void QCadenceReadingShow(string headword)
    {
        QCadenceReading.Text = _lEditor.LEditorTimbre.QTimbreReadingRead(headword);
    }
}
