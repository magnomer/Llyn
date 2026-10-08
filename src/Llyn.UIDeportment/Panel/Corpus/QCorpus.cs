using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCorpus : QChronicleHost
{
    private readonly UserControl _qCorpusSurface;

    private readonly QEditor _qCorpusEditor;

    private readonly QDisplay _qCorpusDisplay;

    private readonly QPanelRail _qCorpusRail;

    private readonly QChoiceOrder _qCorpusOrder;

    private readonly QChoiceFilter _qCorpusFilter;

    private readonly QAnthology _qCorpusAnthology;

    private readonly QQuotation _qCorpusQuotation;

    private readonly QExcerpt _qCorpusExcerpt;

    private readonly QTranscriptSpeaker _qCorpusSpeaker;

    private readonly QTranscript _qCorpusTranscript;

    private readonly QTranscriptGloss _qCorpusGloss;

    private readonly QTranscriptCitation _qCorpusCitation;

    private readonly QTranscriptMention _qCorpusMention;

    private readonly QCorpusPortrait _qCorpusPortrait;

    private CAtelier _cAtelier = null!;

    private CCorpus _cCorpus = null!;

    internal QCorpus(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qCorpusSurface = surface;
        _qCorpusEditor = new QEditor(QContract.QContractFind<FrameworkElement>(surface, "PEditor"));
        _qCorpusDisplay = new QDisplay(QContract.QContractFind<FrameworkElement>(surface, "PDisplay"));
        QLook.QLookStyleAttach(surface);
        QChronicle.QChronicleIntroduce(surface, this);
        _qCorpusRail = new QPanelRail(
            QContract.QContractFind<UserControl>(surface, "PCorpusRail"), QCorpusBin, QCorpusBinIcon, true, true);
        _qCorpusOrder = new QChoiceOrder(QContract.QContractFind<UserControl>(surface, "PCorpusOrder"), QRank);
        _qCorpusFilter = new QChoiceFilter(QContract.QContractFind<UserControl>(surface, "PCorpusFilter"));
        _qCorpusAnthology = new QAnthology(surface);
        _qCorpusQuotation = new QQuotation(surface);
        _qCorpusExcerpt = new QExcerpt(surface);
        _qCorpusSpeaker = new QTranscriptSpeaker(surface);
        _qCorpusTranscript = new QTranscript(surface);
        _qCorpusGloss = new QTranscriptGloss(surface);
        _qCorpusCitation = new QTranscriptCitation(surface);
        _qCorpusMention = new QTranscriptMention(surface);
        _qCorpusPortrait = new QCorpusPortrait(surface);

        _qCorpusRail.QPanelRailCreated += QCorpusFreshObserve;
        _qCorpusRail.QPanelRailStored += QCorpusStoreObserve;
        _qCorpusRail.QPanelRailToggled += QCorpusScribeObserve;
        _qCorpusRail.QPanelRailDeleted += QCorpusBinObserve;
    }

    private Border QRank => QContract.QContractFind<Border>(_qCorpusSurface, "PRank");

    private Button QCorpusBin => QContract.QContractFind<Button>(_qCorpusSurface, "PCorpusBin");

    private QIconImage QCorpusBinIcon => QContract.QContractFind<QIconImage>(_qCorpusSurface, "PCorpusBinIcon");

    internal void QCorpusIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(mentionMenu);

        _cAtelier = atelier;
        _cCorpus = CCorpus.CCorpusCreate(
            atelier,
            QCorpusShownCheck,
            envoy,
            QObserver.QObserverCreate<Action>(static run => run()));
        _qCorpusTranscript.QTranscriptDeskIntroduce(_cCorpus);
        _qCorpusAnthology.QAnthologyIntroduce(_cCorpus);
        _qCorpusQuotation.QQuotationIntroduce(_cCorpus, atelier);
        _qCorpusSpeaker.QTranscriptSpeakerIntroduce(_cCorpus, atelier, envoy);
        _qCorpusGloss.QTranscriptGlossIntroduce(_cCorpus, _qCorpusSpeaker.QTranscriptSpeakerLanguage);
        _qCorpusExcerpt.QExcerptIntroduce(_cCorpus, _qCorpusSpeaker.QTranscriptSpeakerLanguage);
        _qCorpusExcerpt.QExcerptMentionOffered += mentionMenu.QMentionOfferRefine;
        _qCorpusCitation.QTranscriptCitationIntroduce(_cCorpus);
        _qCorpusPortrait.QCorpusPortraitIntroduce(_cCorpus);

        _qCorpusRail.QPanelRailIntroduce(atelier.CAtelierNavigation, this);
        _qCorpusOrder.QChoiceOrderIntroduce(
            _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture, "Rank", CAnthology.CAnthologyOrderRead());
        _qCorpusFilter.QChoiceFilterIntroduce(_cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture, "Gauze");

        _qCorpusDisplay.QDisplayIntroduce(atelier, envoy, volume, mentionMenu, _cCorpus.CCorpusEditor.CEditorDisplay);
        _qCorpusEditor.QEditorIntroduce(atelier, envoy, volume, mentionMenu, _cCorpus.CCorpusEditor);
        _qCorpusMention.QTranscriptMentionIntroduce(
            _cCorpus, atelier, _qCorpusEditor.QEditorProspect, mentionMenu);

        CPanel anthology = _cCorpus.CCorpusAnthology.CAnthologyPanel;
        _cCorpus.CCorpusChanged += QCorpusModeUpdate;
        _cCorpus.CCorpusQueryCleared += QCorpusClearRefine;
        anthology.CPanelChanged += QCorpusModeUpdate;
        _cCorpus.CCorpusQuotation.CQuotationPanel.CPanelChanged += QCorpusModeUpdate;
    }

    internal async void QCorpusVistaRefine()
    {
        _qCorpusOrder.QChoiceOrderRefine();
        _qCorpusFilter.QChoiceFilterRefine();
        CEnsignSheet<IReadOnlyList<CCatalogExample>> sheet =
            await _cCorpus.CCorpusRowsLoad(QEnsignImage.QEnsignDraw);
        _qCorpusFilter.QChoiceFilterBuild(sheet.CEnsignSheetLanguages);
        _qCorpusSpeaker.QSpeakerRefine(sheet.CEnsignSheetLanguages);
        _qCorpusAnthology.QAnthologyRefine(sheet.CEnsignSheetRows);
    }

    internal void QCorpusExitRefine()
    {
        _qCorpusEditor.QEditorPlayerRefine();
        _qCorpusCitation.QTranscriptCitationClose();
        _qCorpusSpeaker.QTranscriptSpeakerClose();
        _qCorpusOrder.QChoiceOrderClose();
        _qCorpusFilter.QChoiceFilterClose();
    }

    public void QChronicleUndoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cCorpus.CCorpusSession.CSessionUndo);
    }

    public void QChronicleRedoObserve()
    {
        QChronicle.QChronicleCaretRefine(_cCorpus.CCorpusSession.CSessionRedo);
    }

    private bool QCorpusShownCheck()
    {
        return _qCorpusSurface.IsVisible;
    }

    private void QCorpusModeUpdate()
    {
        _qCorpusTranscript.QTranscriptVisibleRefine();
        _qCorpusExcerpt.QExcerptVisibleRefine();
        _qCorpusDisplay.QDisplayVisibleRefine(QLook.QLookVisibleRead(_cCorpus.CCorpusDisplayShown));
        _qCorpusEditor.QEditorVisibleRefine(QLook.QLookVisibleRead(_cCorpus.CCorpusDiptych.CDiptychChildEditing));
        _qCorpusRail.QPanelRailRefine(
            _cCorpus.CCorpusDiptych.CDiptychScribeChecked,
            _cCorpus.CCorpusDiptych.CDiptychModeEnabled,
            _cCorpus.CCorpusDiptych.CDiptychBinEnabled);
        _qCorpusRail.QEntryStorableRefine(_cCorpus.CCorpusStoreEnabled);
        QCorpusChronicleRefine();
    }

    private void QCorpusChronicleRefine()
    {
        (bool undo, bool redo) = _cCorpus.CCorpusSession.CSessionChronicleRead();
        _qCorpusRail.QChronicleRefine(undo, redo);
    }

    private void QCorpusClearRefine()
    {
        _qCorpusFilter.QChoiceFilterBuild(_cAtelier.CAtelierCatalog.CCatalogLanguageRead());
        _qCorpusFilter.QChoiceFilterRefine();
    }

    private void QCorpusFreshObserve()
    {
        _cCorpus.CCorpusDiptych.CDiptychEntryCreate();
    }

    private void QCorpusBinObserve()
    {
        _cCorpus.CCorpusDiptych.CDiptychEntryDelete();
    }

    private void QCorpusStoreObserve()
    {
        _cCorpus.CCorpusSession.CSessionSave();
    }

    private void QCorpusScribeObserve(bool scribe)
    {
        _cCorpus.CCorpusDiptych.CDiptychScribeToggle(scribe);
    }
}
