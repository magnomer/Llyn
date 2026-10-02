using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCadence
{
    private readonly FrameworkElement _qCadenceSurface;

    private CEditor _cEditor = null!;

    internal QCadence(FrameworkElement surface)
    {
        _qCadenceSurface = surface;
    }

    private TextBlock QCadenceReading => QContract.QContractFind<TextBlock>(_qCadenceSurface, "PEditorReading");

    private QParadigm QCadenceParadigm => QContract.QContractFind<QParadigm>(_qCadenceSurface, "PEditorParadigm");

    private QFanqie QCadenceFanqie => QContract.QContractFind<QFanqie>(_qCadenceSurface, "PEditorFanqie");

    private QScript QCadenceScript => QContract.QContractFind<QScript>(_qCadenceSurface, "PEditorScript");

    internal void QCadenceIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorDesk.CDeskStarted += QCadenceParadigmRefine;
        editor.CEditorDesk.CDeskStarted += QCadenceScriptRefine;
        editor.CEditorDesk.CDeskStarted += QCadenceFanqieRefine;
        editor.CEditorTimbre.CTimbreParadigmChanged += QCadenceParadigmRefine;
        editor.CEditorTimbre.CTimbreScriptChanged += QCadenceScriptRefine;
        editor.CEditorSounding.CSoundingChanged += QCadenceFanqieRefine;
        QCadenceScript.QScriptRenewalNotice += QCadenceScriptObserve;
        QCadenceFanqie.QFanqieRenewalNotice += QCadenceFanqieObserve;
        QCadenceFanqie.QFanqieDiweiNotice += QCadenceDiweiObserve;
        QCadenceFanqie.QFanqieRepresentativeNotice += QCadenceRepresentativeObserve;
    }

    private void QCadenceParadigmRefine()
    {
        CLecternParadigm paradigm = _cEditor.CEditorSounding.CSoundingParadigmRead();
        QFontFace.QFontRefine(paradigm.CLecternParadigmFont, QCadenceParadigm);
        QCadenceParadigm.QParadigmItems = QParadigmItem.QParadigmItemScan(paradigm.CLecternParadigmSlots);
    }

    private void QCadenceScriptRefine()
    {
        CSoundingScript script = _cEditor.CEditorSounding.CSoundingScriptRead();
        QFontFace.QFontRefine(script.CSoundingScriptFont, QCadenceScript);
        QCadenceScript.QScriptItems = QScriptItem.QScriptItemScan(
            script.CSoundingScriptGroups, QCadenceScript.QScriptFailureRefine);
        QCadenceScript.QScriptPending = script.CSoundingScriptPending;
        QCadenceScript.QScriptRenewable = script.CSoundingScriptRebuildable;
    }

    private void QCadenceScriptObserve()
    {
        _cEditor.CEditorSounding.CSoundingScriptResolve();
    }

    private void QCadenceFanqieRefine()
    {
        CSoundingFanqie fanqie = _cEditor.CEditorSounding.CSoundingFanqieRead();
        QFontFace.QFontRefine(fanqie.CSoundingFanqieFont, QCadenceFanqie);
        QCadenceFanqie.QFanqieItems = QFanqieItem.QFanqieItemScan(fanqie.CSoundingFanqieGroups);
        QCadenceFanqie.QFanqiePending = fanqie.CSoundingFanqiePending;
        QCadenceFanqie.QFanqieRenewable = fanqie.CSoundingFanqieRebuildable;
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
