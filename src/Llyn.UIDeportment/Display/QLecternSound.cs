using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternSound
{
    private readonly CDisplaySound _qLecternSoundArea;

    private readonly QFanqie _qLecternSoundFanqie;

    private readonly TextBlock _qLecternSoundReading;

    private readonly QScript _qLecternSoundScript;

    private readonly QParadigm _qLecternSoundParadigm;

    public QLecternSound(FrameworkElement surface, CDisplaySound area, CLedger ledger, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);
        ArgumentNullException.ThrowIfNull(ledger);
        ArgumentNullException.ThrowIfNull(envoy);

        _qLecternSoundArea = area;
        _qLecternSoundFanqie = QContract.QContractFind<QFanqie>(surface, "PDisplayFanqie");
        _qLecternSoundReading = QContract.QContractFind<TextBlock>(surface, "PDisplayReading");
        _qLecternSoundScript = QContract.QContractFind<QScript>(surface, "PDisplayScript");
        _qLecternSoundParadigm = QContract.QContractFind<QParadigm>(surface, "PDisplayParadigm");

        _qLecternSoundFanqie.QFanqieDiweiNotice += QLecternDiweiObserve;
        _qLecternSoundFanqie.QFanqieStemNotice += QLecternStemObserve;
        _qLecternSoundFanqie.QFanqieRepresentativeNotice += _qLecternSoundArea.CDisplayFanqieSet;
        _qLecternSoundScript.QScriptFailureNotice +=
            exception => ledger.CLedgerFailureShow(envoy, "Display.ScriptFailed", exception);
    }

    public void QLecternFanqieRefine()
    {
        CLecternFanqie fanqie = _qLecternSoundArea.CDisplayFanqieRead();
        QFontFace.QFontRefine(fanqie.CLecternFanqieFont, _qLecternSoundFanqie);
        _qLecternSoundFanqie.SetCurrentValue(
            QFanqie.QFanqieItemsProperty, QFanqieItem.QFanqieItemScan(fanqie.CLecternFanqieGroups));
        _qLecternSoundFanqie.SetCurrentValue(QFanqie.QFanqiePendingProperty, fanqie.CLecternFanqiePending);
        _qLecternSoundReading.Text = fanqie.CLecternFanqieReading;
    }

    public void QLecternScriptRefine()
    {
        CLecternScript script = _qLecternSoundArea.CDisplayScriptRead();
        QFontFace.QFontRefine(script.CLecternScriptFont, _qLecternSoundScript);
        _qLecternSoundScript.SetCurrentValue(
            QScript.QScriptItemsProperty,
            QScriptItem.QScriptItemScan(script.CLecternScriptGroups, _qLecternSoundScript.QScriptFailureRefine));
        _qLecternSoundScript.SetCurrentValue(QScript.QScriptPendingProperty, script.CLecternScriptPending);
    }

    public void QLecternParadigmRefine()
    {
        CLecternParadigm paradigm = _qLecternSoundArea.CDisplayParadigmRead();
        QFontFace.QFontRefine(paradigm.CLecternParadigmFont, _qLecternSoundParadigm);
        _qLecternSoundParadigm.SetCurrentValue(
            QParadigm.QParadigmItemsProperty, QParadigmItem.QParadigmItemScan(paradigm.CLecternParadigmSlots));
        _qLecternSoundParadigm.SetCurrentValue(
            QParadigm.QParadigmSheetProperty, QParadigmSheet.QParadigmSheetCreate(paradigm.CLecternParadigmView));
    }

    private void QLecternDiweiObserve(bool initial, string key)
    {
        _qLecternSoundArea.CDisplayDiweiOpen(initial, key);
    }

    private void QLecternStemObserve(string? key)
    {
        _qLecternSoundArea.CDisplayStemOpen(key);
    }
}
