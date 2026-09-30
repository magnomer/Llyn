using System;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCadence
{
    private readonly PEditor _pEditor;

    private CEditor _cEditor = null!;

    internal QCadence(PEditor editor)
    {
        _pEditor = editor;
    }

    private TextBlock QCadenceReading => QContract.QContractFind<TextBlock>(_pEditor, "PEditorReading");

    private PParadigm QCadenceParadigm => QContract.QContractFind<PParadigm>(_pEditor, "PEditorParadigm");

    private PFanqie QCadenceFanqie => QContract.QContractFind<PFanqie>(_pEditor, "PEditorFanqie");

    private PScript QCadenceScript => QContract.QContractFind<PScript>(_pEditor, "PEditorScript");

    internal void QCadenceIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorDesk.CDeskStarted += QCadenceParadigmRefine;
        editor.CEditorDesk.CDeskStarted += QCadenceScriptRefine;
        editor.CEditorDesk.CDeskStarted += QCadenceFanqieRefine;
        editor.CEditorTimbre.CTimbreParadigmChanged += QCadenceParadigmRefine;
        editor.CEditorTimbre.CTimbreScriptChanged += QCadenceScriptRefine;
        editor.CEditorSounding.CSoundingChanged += QCadenceFanqieRefine;
        QCadenceFanqie.PFanqieDiweiNotice += QCadenceDiweiObserve;
        QCadenceFanqie.PFanqieRepresentativeNotice += QCadenceRepresentativeObserve;
    }

    private void QCadenceParadigmRefine()
    {
        CLecternParadigm paradigm = _cEditor.CEditorSounding.CSoundingParadigmRead();
        QFontFace.QFontRefine(paradigm.CLecternParadigmFont, QCadenceParadigm);
        QCadenceParadigm.PParadigmItems = PParadigmItem.PParadigmItemScan(paradigm.CLecternParadigmSlots);
    }

    private void QCadenceScriptRefine()
    {
        CSoundingScript script = _cEditor.CEditorSounding.CSoundingScriptRead();
        QFontFace.QFontRefine(script.CSoundingScriptFont, QCadenceScript);
        QCadenceScript.PScriptItems = PScriptItem.PScriptItemScan(script.CSoundingScriptGroups);
        QCadenceScript.PScriptPending = script.CSoundingScriptPending;
        QCadenceScript.PScriptRenewal =
            QLook.QLookFirstRead<Action?>(script.CSoundingScriptRebuildable, QCadenceScriptObserve, null);
    }

    private void QCadenceScriptObserve()
    {
        _cEditor.CEditorSounding.CSoundingScriptResolve();
    }

    private void QCadenceFanqieRefine()
    {
        CSoundingFanqie fanqie = _cEditor.CEditorSounding.CSoundingFanqieRead();
        QFontFace.QFontRefine(fanqie.CSoundingFanqieFont, QCadenceFanqie);
        QCadenceFanqie.PFanqieItems = PFanqieItem.PFanqieItemScan(fanqie.CSoundingFanqieGroups);
        QCadenceFanqie.PFanqiePending = fanqie.CSoundingFanqiePending;
        QCadenceFanqie.PFanqieRenewal =
            QLook.QLookFirstRead<Action?>(fanqie.CSoundingFanqieRebuildable, QCadenceFanqieObserve, null);
    }

    private void QCadenceFanqieObserve()
    {
        _cEditor.CEditorSounding.CSoundingFanqieResolve();
    }

    private void QCadenceDiweiObserve(bool initial, string key)
    {
        _cEditor.CEditorSounding.CSoundingDiweiOpen(initial, key);
    }

    private void QCadenceRepresentativeObserve(long fanqieId, int rank, bool raise)
    {
        _cEditor.CEditorSounding.CSoundingFanqieSet(fanqieId, rank, raise);
    }

    internal void QCadenceReadingRefine(string headword)
    {
        QCadenceReading.Text = _cEditor.CEditorSounding.CSoundingReadingRead(headword);
    }
}
