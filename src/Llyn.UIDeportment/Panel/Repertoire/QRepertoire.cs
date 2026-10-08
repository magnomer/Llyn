using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QRepertoire : QChronicleHost
{
    private readonly UserControl _qRepertoireSurface;

    private readonly QEditor _qRepertoireEditor;

    private readonly QDisplay _qRepertoireDisplay;

    private readonly QPanelRail _qRepertoireRail;

    private readonly QChoiceOrder _qRepertoireOrder;

    private readonly QChoiceFilter _qRepertoireFilter;

    private readonly QAtlas _qRepertoireAtlas;

    private readonly QOccurrence _qRepertoireOccurrence;

    private readonly QVignette _qRepertoireVignette;

    private readonly QScenario _qRepertoireScenario;

    private CAtelier _cAtelier = null!;

    private CEnvoy _cEnvoy = null!;

    private CRepertoire _cRepertoire = null!;

    internal QRepertoire(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qRepertoireSurface = surface;
        _qRepertoireEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qRepertoireDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        QLook.QLookStyleAttach(surface);
        QChronicle.QChronicleIntroduce(surface, this);
        _qRepertoireRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PRepertoireRail"),
            QRepertoireBin,
            QRepertoireBinIcon,
            true,
            true);
        _qRepertoireOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PRepertoireOrder"), QTier);
        _qRepertoireFilter = new QChoiceFilter(QContract.QContractFind<UserControl>(surface, "PRepertoireFilter"));
        _qRepertoireAtlas = new QAtlas(surface);
        _qRepertoireOccurrence = new QOccurrence(surface);
        _qRepertoireVignette = new QVignette(surface);
        _qRepertoireScenario = new QScenario(surface);

        surface.CommandBindings.Add(new CommandBinding(
            ApplicationCommands.Print, QRepertoirePressObserve, QRepertoirePressRefine));
        surface.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QRepertoirePortraitObserve, QRepertoirePortraitRefine));

        _qRepertoireRail.QPanelRailCreated += QRepertoireFreshObserve;
        _qRepertoireRail.QPanelRailStored += QRepertoireStoreObserve;
        _qRepertoireRail.QPanelRailToggled += QRepertoireScribeObserve;
        _qRepertoireRail.QPanelRailDeleted += QRepertoireBinObserve;
    }

    private Border QTier => QContract.QContractFind<Border>(_qRepertoireSurface, "PTier");

    private Button QRepertoireBin => QContract.QContractFind<Button>(_qRepertoireSurface, "PRepertoireBin");

    private QIconImage QRepertoireBinIcon =>
        QContract.QContractFind<QIconImage>(_qRepertoireSurface, "PRepertoireBinIcon");

    internal void QRepertoireIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cAtelier = atelier;
        _cEnvoy = envoy;
        _cRepertoire = CRepertoire.CRepertoireCreate(
            atelier,
            QRepertoireShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        _qRepertoireScenario.QScenarioDeskIntroduce(_cRepertoire);
        _qRepertoireVignette.QVignetteIntroduce(_cRepertoire, atelier);
        _qRepertoireAtlas.QAtlasIntroduce(_cRepertoire);
        _qRepertoireOccurrence.QOccurrenceIntroduce(_cRepertoire, atelier);

        _qRepertoireRail.QPanelRailIntroduce(atelier.CAtelierNavigation, this);
        _qRepertoireOrder.QChoiceOrderIntroduce(
            _cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture, "Tier", CAtlas.CAtlasOrderRead());
        _qRepertoireFilter.QChoiceFilterIntroduce(_cRepertoire.CRepertoireAtlas.CAtlasPanel.CPanelAperture, "Mesh");

        _qRepertoireDisplay.QDisplayIntroduce(
            atelier, envoy, volume, mentionMenu, _cRepertoire.CRepertoireEditor.CEditorDisplay);
        _qRepertoireEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, _cRepertoire.CRepertoireEditor);

        CPanel atlas = _cRepertoire.CRepertoireAtlas.CAtlasPanel;
        _cRepertoire.CRepertoireChanged += QRepertoireModeRefine;
        _cRepertoire.CRepertoireQueryCleared += QRepertoireClearRefine;
        _cRepertoire.CRepertoireWorkspaceChanged += QRepertoireWorkspaceRefine;
        atlas.CPanelChanged += QRepertoireModeRefine;
        _cRepertoire.CRepertoireOccurrence.COccurrencePanel.CPanelChanged += QRepertoireModeRefine;
    }

    internal async void QRepertoireVistaRefine()
    {
        _qRepertoireOrder.QChoiceOrderRefine();
        _qRepertoireFilter.QChoiceFilterRefine();
        CEnsignSheet<IReadOnlyList<CCatalogSituation>> sheet =
            await _cRepertoire.CRepertoireRowsLoad(QEnsignImage.QEnsignDraw);
        _qRepertoireFilter.QChoiceFilterBuild(sheet.CEnsignSheetLanguages);
        _qRepertoireAtlas.QAtlasRefine(sheet.CEnsignSheetRows);
    }

    internal void QRepertoireExitRefine()
    {
        _qRepertoireEditor.QEditorPlayerRefine();
        _qRepertoireOrder.QChoiceOrderClose();
        _qRepertoireFilter.QChoiceFilterClose();
    }

    public void QChronicleUndoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cRepertoire.CRepertoireSession.CSessionUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cRepertoire.CRepertoireSession.CSessionRedo);
    }

    private bool QRepertoireShownCheck()
    {
        return _qRepertoireSurface.IsVisible;
    }

    private void QRepertoireModeRefine()
    {
        _qRepertoireScenario.QScenarioVisibleRefine();
        _qRepertoireVignette.QVignetteVisibleRefine();
        _qRepertoireDisplay.QDisplayVisibleRefine(
            QLook.QLookVisibleRead(_cRepertoire.CRepertoireDiptych.CDiptychChildShown));
        _qRepertoireEditor.QEditorVisibleRefine(
            QLook.QLookVisibleRead(_cRepertoire.CRepertoireDiptych.CDiptychChildEditing));
        _qRepertoireRail.QPanelRailRefine(
            _cRepertoire.CRepertoireDiptych.CDiptychScribeChecked,
            _cRepertoire.CRepertoireDiptych.CDiptychModeEnabled,
            _cRepertoire.CRepertoireDiptych.CDiptychBinEnabled);
        _qRepertoireRail.QEntryStorableRefine(_cRepertoire.CRepertoireStoreEnabled);
        QRepertoireChronicleRefine();
    }

    private void QRepertoireChronicleRefine()
    {
        (bool undo, bool redo) = _cRepertoire.CRepertoireSession.CSessionChronicleRead();
        _qRepertoireRail.QChronicleRefine(undo, redo);
    }

    private void QRepertoireClearRefine()
    {
        _qRepertoireFilter.QChoiceFilterBuild(_cRepertoire.CRepertoireAtlas.CAtlasLanguageRead());
        _qRepertoireFilter.QChoiceFilterRefine();
    }

    private async void QRepertoireWorkspaceRefine()
    {
        await _cAtelier.CAtelierCatalog.CCatalogEnsignLoad(_cEnvoy, QEnsignImage.QEnsignDraw);
    }

    private void QRepertoireFreshObserve()
    {
        _cRepertoire.CRepertoireDiptych.CDiptychEntryCreate();
    }

    private void QRepertoireBinObserve()
    {
        _cRepertoire.CRepertoireDiptych.CDiptychEntryDelete();
    }

    private void QRepertoireStoreObserve()
    {
        _cRepertoire.CRepertoireSession.CSessionSave();
    }

    private void QRepertoireScribeObserve(bool scribe)
    {
        _cRepertoire.CRepertoireDiptych.CDiptychScribeToggle(scribe);
    }

    private void QRepertoirePressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cRepertoire?.CRepertoirePressAllowed ?? false;
    }

    private async void QRepertoirePressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cRepertoire.CRepertoirePortraitPrint();
    }

    private void QRepertoirePortraitRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cRepertoire?.CRepertoirePortraitAllowed ?? false;
    }

    private async void QRepertoirePortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cRepertoire.CRepertoirePortraitExport();
    }
}
