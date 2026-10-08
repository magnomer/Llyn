using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QReference : QChronicleHost
{
    private readonly UserControl _qReferenceSurface;

    private readonly QShelf _qReferenceShelf;

    private readonly QEditor _qReferenceEditor;

    private readonly QDisplay _qReferenceDisplay;

    private readonly QImprint _qImprint;

    private readonly QColophon _qColophon;

    private readonly QFootnote _qReferenceFootnote;

    private readonly QPanelRail _qReferenceRail;

    private readonly QChoiceOrder _qReferenceOrder;

    private readonly QChoiceFilter _qReferenceFilter;

    private CShelf _cShelf = null!;

    internal QReference(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qReferenceSurface = surface;
        _qReferenceEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qReferenceDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        _qImprint = new QImprint(QReferenceImprint);
        _qColophon = new QColophon(QReferenceColophon);
        _qReferenceShelf = new QShelf(surface);
        _qReferenceFootnote = new QFootnote(surface);
        _qReferenceRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PReferenceRail"),
            QReferenceBin, QReferenceBinIcon, true, true);
        _qReferenceOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PReferenceOrder"), QGrade);
        _qReferenceFilter = new QChoiceFilter(QContract.QContractFind<UserControl>(surface, "PReferenceFilter"));

        _qReferenceRail.QPanelRailCreated += QReferenceFreshObserve;
        _qReferenceRail.QPanelRailStored += QReferenceStoreObserve;
        _qReferenceRail.QPanelRailToggled += QReferenceScribeObserve;
        _qReferenceRail.QPanelRailDeleted += QReferenceBinObserve;
    }

    private Border QGrade => QContract.QContractFind<Border>(_qReferenceSurface, "PGrade");

    private UserControl QReferenceColophon => QContract.QContractFind<UserControl>(_qReferenceSurface, "PColophon");

    private UserControl QReferenceImprint => QContract.QContractFind<UserControl>(_qReferenceSurface, "PImprint");

    private Button QReferenceBin => QContract.QContractFind<Button>(_qReferenceSurface, "PReferenceBin");

    private QIconImage QReferenceBinIcon =>
        QContract.QContractFind<QIconImage>(_qReferenceSurface, "PReferenceBinIcon");

    internal void QReferenceIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cShelf = CShelf.CShelfCreate(
            atelier,
            QReferenceShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        _cShelf.CShelfChanged += QReferenceModeRefine;
        _cShelf.CShelfPanel.CPanelChanged += QReferenceModeRefine;
        _qReferenceShelf.QShelfIntroduce(_cShelf, _qColophon);
        _cShelf.CShelfPanel.CPanelAperture.CApertureRowsChanged += _qImprint.QImprintTallyRefine;
        _cShelf.CShelfPanel.CPanelCleared += _qImprint.QImprintClearRefine;
        _cShelf.CShelfPanel.CPanelCleared += _qColophon.QColophonClearRefine;
        _cShelf.CShelfColophonChanged += _qColophon.QColophonRefine;
        _cShelf.CShelfFootnote.CFootnotePanel.CPanelChanged += QReferenceModeRefine;
        _qReferenceFootnote.QFootnoteIntroduce(_cShelf, atelier);

        _qReferenceRail.QPanelRailIntroduce(atelier.CAtelierNavigation, this);
        _qReferenceOrder.QChoiceOrderIntroduce(_cShelf.CShelfPanel.CPanelAperture, "Grade", CShelf.CShelfOrderRead());
        _qReferenceFilter.QChoiceFilterIntroduce(_cShelf.CShelfPanel.CPanelAperture, "Trellis");

        _qImprint.QImprintIntroduce(_cShelf.CShelfImprint);
        _qReferenceDisplay.QDisplayIntroduce(atelier, envoy, volume, mentionMenu, _cShelf.CShelfEditor.CEditorDisplay);
        _qReferenceEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, _cShelf.CShelfEditor);

        _qReferenceSurface.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QReferencePressObserve, QReferencePressRefine));
        _qReferenceSurface.CommandBindings.Add(
            new CommandBinding(PDisplayCommand.PDisplayCommandPortrait, QReferencePortraitObserve,
            QReferencePortraitRefine));
    }

    internal async void QReferenceVistaRefine()
    {
        _qReferenceOrder.QChoiceOrderRefine();
        _qReferenceFilter.QChoiceFilterRefine();
        CEnsignSheet<CShelfRoll> sheet =
            await _cShelf.CShelfRollLoad(QEnsignImage.QEnsignDraw);
        _qReferenceFilter.QChoiceFilterBuild(sheet.CEnsignSheetLanguages);
        _qReferenceShelf.QShelfRefine(sheet.CEnsignSheetRows);
    }

    internal void QReferenceExitRefine()
    {
        _qImprint.QImprintCloseRefine();
        _qReferenceEditor.QEditorPlayerRefine();
        _qReferenceOrder.QChoiceOrderClose();
        _qReferenceFilter.QChoiceFilterClose();
    }

    private bool QReferenceShownCheck()
    {
        return _qReferenceSurface.IsVisible;
    }

    private void QReferenceModeRefine()
    {
        _qReferenceEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychChildEditing));
        _qReferenceDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychChildShown));
        QReferenceImprint.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychParentEditing);
        QReferenceColophon.Visibility = QLook.QLookVisibleRead(_cShelf.CShelfDiptych.CDiptychParentShown);
        _qReferenceRail.QPanelRailRefine(
            _cShelf.CShelfDiptych.CDiptychScribeChecked,
            _cShelf.CShelfDiptych.CDiptychModeEnabled,
            _cShelf.CShelfDiptych.CDiptychBinEnabled);
        _qReferenceRail.QEntryStorableRefine(_cShelf.CShelfStoreEnabled);
        QReferenceChronicleRefine();
    }

    private void QReferenceChronicleRefine()
    {
        (bool undo, bool redo) = _cShelf.CShelfSession.CSessionChronicleRead();
        _qReferenceRail.QChronicleRefine(undo, redo);
    }

    private void QReferenceFreshObserve()
    {
        _cShelf.CShelfDiptych.CDiptychEntryCreate();
    }

    private void QReferenceScribeObserve(bool scribe)
    {
        _cShelf.CShelfScribeToggle(scribe);
    }

    private void QReferenceStoreObserve()
    {
        _cShelf.CShelfSession.CSessionSave();
    }

    public void QChronicleUndoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cShelf.CShelfSession.CSessionUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cShelf.CShelfSession.CSessionRedo);
    }

    private void QReferenceBinObserve()
    {
        _cShelf.CShelfDiptych.CDiptychEntryDelete();
    }

    private void QReferencePressRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cShelf.CShelfPressAllowed;
    }

    private async void QReferencePressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cShelf.CShelfPortraitPrint();
    }

    private void QReferencePortraitRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cShelf.CShelfPortraitAllowed;
    }

    private async void QReferencePortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cShelf.CShelfPortraitExport();
    }
}
