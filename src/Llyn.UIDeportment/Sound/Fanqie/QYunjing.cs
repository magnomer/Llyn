using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QYunjing
{
    private readonly UserControl _qYunjingSurface;

    private readonly QEditor _qYunjingEditor;

    private readonly QDisplay _qYunjingDisplay;

    private readonly QDiwei _qDiwei;

    private readonly QPanelRail _qYunjingRail;

    private readonly QChoiceOrder _qYunjingOrder;

    private readonly QChoiceOrder _qYunmuOrder;

    private readonly QDiweiIndex _qDiweiIndex;

    private readonly QXiaoyun _qXiaoyun;

    private CYunjing _cYunjing = null!;

    internal QYunjing(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qYunjingSurface = surface;
        _qYunjingEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qYunjingDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        _qDiwei = new QDiwei(QYunjingDiwei);
        _qYunjingRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PYunjingRail"),
            QYunjingBin,
            QYunjingBinIcon,
            true,
            true);
        _qYunjingOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PYunjingOrder"), QLadder);
        _qYunmuOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PYunmuOrder"), QStair);
        _qDiweiIndex = new QDiweiIndex(surface);
        _qXiaoyun = new QXiaoyun(surface);

        _qYunjingRail.QPanelRailCreated += QYunjingFreshObserve;
        _qYunjingRail.QPanelRailStored += QYunjingStoreObserve;
        _qYunjingRail.QPanelRailToggled += QYunjingScribeObserve;
        _qYunjingRail.QPanelRailDeleted += QYunjingBinObserve;
    }

    private Border QLadder => QContract.QContractFind<Border>(_qYunjingSurface, "PLadder");

    private Border QStair => QContract.QContractFind<Border>(_qYunjingSurface, "PStair");

    private UserControl QYunjingDiwei => QContract.QContractFind<UserControl>(_qYunjingSurface, "PYunjingDiwei");

    private Button QYunjingBin => QContract.QContractFind<Button>(_qYunjingSurface, "PYunjingBin");

    private QIconImage QYunjingBinIcon => QContract.QContractFind<QIconImage>(_qYunjingSurface, "PYunjingBinIcon");

    internal void QYunjingIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cYunjing = CYunjing.CYunjingCreate(
            atelier,
            QYunjingShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        CPanel panel = _cYunjing.CYunjingXiaoyun.CEntryListPanel;
        _qYunjingOrder.QChoiceOrderIntroduce(_cYunjing.CYunjingShengmu, "Ladder", CYunjing.CYunjingOrderRead());
        _qYunmuOrder.QChoiceOrderIntroduce(_cYunjing.CYunjingYunmu, "Stair", CYunjing.CYunjingOrderRead());
        _qDiweiIndex.QDiweiIndexIntroduce(_cYunjing, atelier);
        _cYunjing.CYunjingChanged += QYunjingDiweiRefine;
        _cYunjing.CYunjingChanged += QYunjingModeRefine;
        _cYunjing.CYunjingWorkspaceChanged += QYunjingWorkspaceRefine;
        panel.CPanelChanged += QYunjingModeRefine;
        _qXiaoyun.QXiaoyunIntroduce(_cYunjing);
        panel.CPanelCleared += QYunjingDiweiRefine;
        _cYunjing.CYunjingXiaoyun.CEntryListEditor.CEditorDesk.CDeskStateChanged += QYunjingStoreRefine;

        _qYunjingDisplay.QDisplayIntroduce(
            atelier, envoy, volume, mentionMenu, _cYunjing.CYunjingXiaoyun.CEntryListEditor.CEditorDisplay);
        _qDiwei.QDiweiIntroduce(_cYunjing);
        _qYunjingEditor.QEditorIntroduce(
            atelier, envoy, volume, mentionMenu, _cYunjing.CYunjingXiaoyun.CEntryListEditor);
        _qYunjingEditor.QEditorChronicleChanged += QYunjingChronicleRefine;
        _qYunjingRail.QPanelRailIntroduce(atelier.CAtelierNavigation, _qYunjingEditor);

        _qYunjingSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QYunjingPressObserve, QYunjingPressRefine));
        _qYunjingSurface.CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, QYunjingPortraitObserve, QYunjingPressRefine));
    }

    private void QYunjingStoreRefine()
    {
        _qYunjingRail.QEntryStorableRefine(
            _cYunjing.CYunjingXiaoyun.CEntryListEditor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
    }

    internal void QYunjingVistaRefine()
    {
        _qYunjingOrder.QChoiceOrderRefine();
        _qYunmuOrder.QChoiceOrderRefine();
        QYunjingModeRefine();
        _qXiaoyun.QXiaoyunRefine();
    }

    private async void QYunjingWorkspaceRefine()
    {
        _qXiaoyun.QXiaoyunRefine(
            (await _cYunjing.CYunjingXiaoyun.CEntryListLoad(QEnsignImage.QEnsignDraw)).CEnsignSheetRows);
    }

    internal void QYunjingExitRefine()
    {
        _qYunjingEditor.QEditorPlayerRefine();
    }

    private bool QYunjingShownCheck()
    {
        return _qYunjingSurface.IsVisible;
    }

    internal void QYunjingDiweiRefine()
    {
        _qDiwei.QDiweiRefine(_cYunjing.CYunjingDiweiRead(), _cYunjing.CYunjingDiweiKey);
    }

    private void QYunjingModeRefine()
    {
        CPanel panel = _cYunjing.CYunjingXiaoyun.CEntryListPanel;
        _qYunjingEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(_cYunjing.CYunjingXiaoyun.CEntryListEditing));
        _qYunjingDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(_cYunjing.CYunjingDisplayShown));
        QYunjingDiwei.Visibility = QLook.QLookVisibleRead(_cYunjing.CYunjingDiweiShown);
        _qYunjingRail.QPanelRailRefine(panel.CPanelScribeChecked, panel.CPanelModeEnabled, panel.CPanelBinEnabled);
    }

    private void QYunjingFreshObserve()
    {
        _cYunjing.CYunjingXiaoyun.CEntryListPanel.CPanelEntryCreate();
    }

    private void QYunjingScribeObserve(bool scribe)
    {
        _cYunjing.CYunjingXiaoyun.CEntryListPanel.CPanelScribeToggle(scribe);
    }

    private void QYunjingStoreObserve()
    {
        _cYunjing.CYunjingXiaoyun.CEntryListEditor.CEditorEntrySave();
    }

    private void QYunjingBinObserve()
    {
        _cYunjing.CYunjingXiaoyun.CEntryListPanel.CPanelBin.CPanelBinDelete();
    }

    private void QYunjingPressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cYunjing.CYunjingXiaoyun.CEntryListPanel.CPanelPressAllowed;
    }

    private async void QYunjingPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cYunjing.CYunjingXiaoyun.CEntryListPrint();
    }

    private async void QYunjingPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cYunjing.CYunjingXiaoyun.CEntryListExport();
    }

    private void QYunjingChronicleRefine()
    {
        (bool undo, bool redo) =
            _cYunjing.CYunjingXiaoyun.CEntryListEditor.CEditorDesk.CDeskChronicle.CDeskChronicleRead();
        _qYunjingRail.QChronicleRefine(undo, redo);
    }
}
