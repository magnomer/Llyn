using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCadence
{
    private readonly FrameworkElement _qCadenceSurface;

    private CSounding _cSounding = null!;

    private CFold _cFold = null!;

    internal QCadence(FrameworkElement surface)
    {
        _qCadenceSurface = surface;
    }

    private TextBlock QCadenceReading => QContract.QContractFind<TextBlock>(_qCadenceSurface, "PEditorReading");

    private QParadigm QCadenceParadigm => QContract.QContractFind<QParadigm>(_qCadenceSurface, "PEditorParadigm");

    private QFanqie QCadenceFanqie => QContract.QContractFind<QFanqie>(_qCadenceSurface, "PEditorFanqie");

    private QScript QCadenceScript => QContract.QContractFind<QScript>(_qCadenceSurface, "PEditorScript");

    internal void QCadenceIntroduce(
        CDesk desk, CEntry entry, CTimbre timbre, CSounding sounding, CFold fold, CLedger ledger, CEnvoy envoy)
    {
        _cSounding = sounding;
        _cFold = fold;
        QCadenceScript.QScriptFailureNotice +=
            exception => ledger.CLedgerFailureShow(envoy, "Display.ScriptFailed", exception);
        desk.CDeskStarted += QCadenceParadigmRefine;
        desk.CDeskStarted += QCadenceScriptRefine;
        desk.CDeskStarted += QCadenceFanqieRefine;
        timbre.CTimbreParadigmChanged += QCadenceParadigmRefine;
        timbre.CTimbreScriptChanged += QCadenceScriptRefine;
        sounding.CSoundingChanged += QCadenceFanqieRefine;
        entry.CEntryDraftChanged += QCadenceFoldRefine;
        QCadenceScript.QScriptRenewalNotice += QCadenceScriptObserve;
        QCadenceFanqie.QFanqieRenewalNotice += QCadenceFanqieObserve;
        QCadenceFanqie.QFanqieDiweiNotice += QCadenceDiweiObserve;
        QCadenceFanqie.QFanqieRepresentativeNotice += QCadenceRepresentativeObserve;
        QCadenceFanqie.QFanqieSwitch.Click += QCadenceSpellingObserve;
        QCadenceScript.QScriptSwitch.Click += QCadenceWritingObserve;
    }

    private void QCadenceFoldRefine(CEntryDraft _)
    {
        QCadenceFanqie.QFanqieFoldRefine(_cFold.CFoldFanqieOpened);
        QCadenceScript.QScriptFoldRefine(_cFold.CFoldScriptOpened);
    }

    private void QCadenceSpellingObserve(object sender, RoutedEventArgs e)
    {
        ToggleButton shown = QCadenceFanqie.QFanqieSwitch;
        QLook.QLookCheckedRefine(shown, _cFold.CFoldFanqieSpread(QLook.QLookCheckedRead(shown.IsChecked)));
    }

    private void QCadenceWritingObserve(object sender, RoutedEventArgs e)
    {
        ToggleButton shown = QCadenceScript.QScriptSwitch;
        QLook.QLookCheckedRefine(shown, _cFold.CFoldScriptSpread(QLook.QLookCheckedRead(shown.IsChecked)));
    }

    private void QCadenceParadigmRefine()
    {
        CLecternParadigm paradigm = _cSounding.CSoundingParadigmRead();
        QFontFace.QFontRefine(paradigm.CLecternParadigmFont, QCadenceParadigm);
        QCadenceParadigm.QParadigmItems = QParadigmItem.QParadigmItemScan(paradigm.CLecternParadigmSlots);
        QCadenceParadigm.QParadigmSheet = QParadigmSheet.QParadigmSheetCreate(paradigm.CLecternParadigmView);
    }

    private void QCadenceScriptRefine()
    {
        CSoundingScript script = _cSounding.CSoundingScriptRead();
        QFontFace.QFontRefine(script.CSoundingScriptFont, QCadenceScript);
        QCadenceScript.QScriptItems = QScriptItem.QScriptItemScan(
            script.CSoundingScriptGroups, QCadenceScript.QScriptFailureRefine);
        QCadenceScript.QScriptPending = script.CSoundingScriptPending;
        QCadenceScript.QScriptRenewable = script.CSoundingScriptRebuildable;
    }

    private void QCadenceScriptObserve()
    {
        _cSounding.CSoundingScriptResolve();
    }

    private void QCadenceFanqieRefine()
    {
        CSoundingFanqie fanqie = _cSounding.CSoundingFanqieRead();
        QFontFace.QFontRefine(fanqie.CSoundingFanqieFont, QCadenceFanqie);
        QCadenceFanqie.QFanqieItems = QFanqieItem.QFanqieItemScan(fanqie.CSoundingFanqieGroups);
        QCadenceFanqie.QFanqiePending = fanqie.CSoundingFanqiePending;
        QCadenceFanqie.QFanqieRenewable = fanqie.CSoundingFanqieRebuildable;
    }

    private void QCadenceFanqieObserve()
    {
        _cSounding.CSoundingFanqieResolve();
    }

    private void QCadenceDiweiObserve(bool initial, string key)
    {
        _cSounding.CSoundingDiweiOpen(initial, key);
    }

    private void QCadenceRepresentativeObserve(long fanqieId, int rank, bool raise)
    {
        _cSounding.CSoundingFanqieSet(fanqieId, rank, raise);
    }

    internal void QCadenceReadingRefine(string headword)
    {
        QCadenceReading.Text = _cSounding.CSoundingReadingRead(headword);
    }
}
