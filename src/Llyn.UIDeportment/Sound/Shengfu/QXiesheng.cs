using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QXiesheng
{
    private readonly ObservableCollection<QGroveItem> _qGroveList = [];

    private readonly UserControl _qXieshengSurface;

    private readonly QEditor _qXieshengEditor;

    private readonly QDisplay _qXieshengDisplay;

    private readonly QStem _qStem;

    private readonly QPanelRail _qXieshengRail;

    private readonly QChoiceOrder _qXieshengOrder;

    private readonly QKindred _qKindred;

    private CXiesheng _cXiesheng = null!;

    internal QXiesheng(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qXieshengSurface = surface;
        _qXieshengEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qXieshengDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        _qStem = new QStem(QXieshengStem);

        surface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QXieshengPressObserve, QXieshengPressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QXieshengPortraitObserve, QXieshengPressRefine));
        _qXieshengRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PXieshengRail"),
            QXieshengBin,
            QXieshengBinIcon,
            true,
            true);
        _qXieshengOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PXieshengOrder"), QRungBar);
        _qKindred = new QKindred(surface);

        QLodestar.SetResourceReference(QField.QFieldHintProperty, "Lodestar.Search");

        QLookItem.QLookItemAttach(QGrove, QGroveItem.QGroveItemRefine);
        QGrove.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QGroveObserve));

        QLodestar.TextChanged += QLodestarObserve;
        _qXieshengRail.QPanelRailCreated += QXieshengFreshObserve;
        _qXieshengRail.QPanelRailStored += QXieshengStoreObserve;
        _qXieshengRail.QPanelRailToggled += QXieshengScribeObserve;
        _qXieshengRail.QPanelRailDeleted += QXieshengBinObserve;
    }

    private Border QRungBar => QContract.QContractFind<Border>(_qXieshengSurface, "PRungBar");

    private TextBox QLodestar => QContract.QContractFind<TextBox>(_qXieshengSurface, "PLodestar");

    private ItemsControl QGrove => QContract.QContractFind<ItemsControl>(_qXieshengSurface, "PGrove");

    private TextBlock QGroveEmpty => QContract.QContractFind<TextBlock>(_qXieshengSurface, "PGroveEmpty");

    private UserControl QXieshengStem => QContract.QContractFind<UserControl>(_qXieshengSurface, "PXieshengStem");

    private Button QXieshengBin => QContract.QContractFind<Button>(_qXieshengSurface, "PXieshengBin");

    private QIconImage QXieshengBinIcon => QContract.QContractFind<QIconImage>(_qXieshengSurface, "PXieshengBinIcon");

    internal void QXieshengIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cXiesheng = CXiesheng.CXieshengCreate(
            atelier,
            QXieshengShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        CPanel panel = _cXiesheng.CXieshengKindred.CEntryListPanel;
        _qXieshengOrder.QChoiceOrderIntroduce(_cXiesheng.CXieshengGrove, "Rung", CXiesheng.CXieshengOrderRead());
        _cXiesheng.CXieshengStemOpened += QLodestarRefine;
        _cXiesheng.CXieshengChanged += QGroveRefine;
        _cXiesheng.CXieshengChanged += QXieshengStemRefine;
        _cXiesheng.CXieshengChanged += QXieshengModeRefine;
        _cXiesheng.CXieshengWorkspaceChanged += QXieshengWorkspaceRefine;
        panel.CPanelChanged += QXieshengModeRefine;
        panel.CPanelCleared += QXieshengStemRefine;
        _cXiesheng.CXieshengKindred.CEntryListEditor.CEditorDesk.CDeskStateChanged += QXieshengStoreRefine;
        _qKindred.QKindredIntroduce(_cXiesheng);

        QGrove.ItemsSource = _qGroveList;

        _qXieshengDisplay.QDisplayIntroduce(
            atelier, envoy, volume, mentionMenu, _cXiesheng.CXieshengKindred.CEntryListEditor.CEditorDisplay);
        _qStem.QStemIntroduce(_cXiesheng);
        _qXieshengEditor.QEditorIntroduce(
            atelier, envoy, volume, mentionMenu, _cXiesheng.CXieshengKindred.CEntryListEditor);
        _qXieshengEditor.QEditorChronicleChanged += QXieshengChronicleRefine;
        _qXieshengRail.QPanelRailIntroduce(atelier.CAtelierNavigation, _qXieshengEditor);
    }

    private void QXieshengStoreRefine()
    {
        _qXieshengRail.QEntryStorableRefine(
            _cXiesheng.CXieshengKindred.CEntryListEditor.CEditorDesk.CDeskDraft.CDeskDraftStorable);
    }

    internal void QXieshengVistaRefine()
    {
        _qXieshengOrder.QChoiceOrderRefine();
        QXieshengModeRefine();
        _qKindred.QKindredRefine();
    }

    private async void QXieshengWorkspaceRefine()
    {
        _qKindred.QKindredRefine(
            (await _cXiesheng.CXieshengKindred.CEntryListLoad(QEnsignImage.QEnsignDraw)).CEnsignSheetRows);
    }

    private void QLodestarRefine()
    {
        QLodestar.Text = string.Empty;
    }

    internal void QXieshengExitRefine()
    {
        _qXieshengEditor.QEditorPlayerRefine();
    }

    private bool QXieshengShownCheck()
    {
        return _qXieshengSurface.IsVisible;
    }

    internal void QGroveRefine()
    {
        QSplice.QSpliceRefine(
            _qGroveList,
            QGroveItem.QGroveItemBuild(_cXiesheng.CXieshengGroveRead()),
            QGroveItem.QGroveItemMatch,
            QGroveItem.QGroveItemSync);
        QGroveEmpty.SetResourceReference(TextBlock.TextProperty, _cXiesheng.CXieshengGrove.CApertureKey);
        QGroveEmpty.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengGrove.CApertureEmpty);
    }

    internal void QXieshengStemRefine()
    {
        _qStem.QStemRefine(_cXiesheng.CXieshengStemRead());
    }

    private void QXieshengModeRefine()
    {
        CPanel panel = _cXiesheng.CXieshengKindred.CEntryListPanel;
        _qXieshengEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(_cXiesheng.CXieshengKindred.CEntryListEditing));
        _qXieshengDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(_cXiesheng.CXieshengDisplayShown));
        QXieshengStem.Visibility = QLook.QLookVisibleRead(_cXiesheng.CXieshengStemShown);
        _qXieshengRail.QPanelRailRefine(panel.CPanelScribeChecked, panel.CPanelModeEnabled, panel.CPanelBinEnabled);
    }

    private void QLodestarObserve(object sender, TextChangedEventArgs e)
    {
        _cXiesheng.CXieshengGrove.CApertureQuerySet(QLodestar.Text);
    }

    private void QGroveObserve(object sender, RoutedEventArgs e)
    {
        _cXiesheng.CXieshengStemSelect(QSender.QSenderSourceRead<QGroveItem>(e)?.QGroveItemId);
    }

    private void QXieshengFreshObserve()
    {
        _cXiesheng.CXieshengKindred.CEntryListPanel.CPanelEntryCreate();
    }

    private void QXieshengScribeObserve(bool scribe)
    {
        _cXiesheng.CXieshengKindred.CEntryListPanel.CPanelScribeToggle(scribe);
    }

    private void QXieshengStoreObserve()
    {
        _cXiesheng.CXieshengKindred.CEntryListEditor.CEditorEntrySave();
    }

    private void QXieshengBinObserve()
    {
        _cXiesheng.CXieshengKindred.CEntryListPanel.CPanelBin.CPanelBinDelete();
    }

    private void QXieshengPressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cXiesheng?.CXieshengKindred.CEntryListPanel.CPanelPressAllowed ?? false;
    }

    private async void QXieshengPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cXiesheng.CXieshengKindred.CEntryListPrint();
    }

    private async void QXieshengPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cXiesheng.CXieshengKindred.CEntryListExport();
    }

    private void QXieshengChronicleRefine()
    {
        (bool undo, bool redo) =
            _cXiesheng.CXieshengKindred.CEntryListEditor.CEditorDesk.CDeskChronicle.CDeskChronicleRead();
        _qXieshengRail.QChronicleRefine(undo, redo);
    }
}
