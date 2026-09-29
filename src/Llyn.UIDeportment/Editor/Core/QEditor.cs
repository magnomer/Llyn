using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEditor
{
    internal QEditor(CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        QEditorArea = editor;
    }

    internal CEditor QEditorArea { get; }

    internal void QEditorIntroduce(PWindow host, PEditor surface)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(surface);

        CEditor editor = QEditorArea;
        CDesk desk = editor.CEditorDesk;
        desk.CDeskStarted += surface.PEditorStartRefine;
        desk.CDeskStarted += surface.PReflexAnchorShow;
        desk.CDeskStarted += surface.PReadingRefine;
        desk.CDeskFailed += host.PWindowFailureRefine;
        desk.CDeskStateChanged += surface.PEditorStateRefine;
        desk.CDeskRefused += host.PWindowEnvoy.CEnvoyFailureShow;
        desk.CDeskErrand.CErrandHarvestStarted += surface.PClipSourceHandle;
        desk.CDeskErrand.CErrandRecordingAdded += surface.PClipRecordingHandle;
        desk.CDeskErrand.CErrandHarvestFinished += surface.PClipFinishHandle;
        desk.CDeskErrand.CErrandLookupStarted += surface.PNotationSourceHandle;
        desk.CDeskErrand.CErrandCandidateAdded += surface.PNotationCandidateHandle;
        desk.CDeskErrand.CErrandLookupFinished += surface.PNotationFinishHandle;
        editor.CEditorSounding.CSoundingChanged += surface.PReflexAnchorShow;
        editor.CEditorSounding.CSoundingChanged += surface.PReadingRefine;
        editor.CEditorTimbre.CTimbreReflexChanged += surface.PReflexPendingShow;
        editor.CEditorSentence.CSentenceReferenceChanged += surface.PSentenceLoad;

        editor.CEditorDraftChanged += surface.PEditorDraftRefine;
        editor.CEditorDraftChanged += surface.PPronunciationRefine;
        editor.CEditorDraftChanged += surface.PTimbreRefine;
        editor.CEditorDraftChanged += surface.PAccentShow;
        editor.CEditorDraftChanged += surface.PGlyphShow;
        editor.CEditorDraftChanged += surface.PTranscriptionShow;
        editor.CEditorDraftChanged += surface.PReflexShow;
        editor.CEditorDraftChanged += surface.PMarkerRefine;
        editor.CEditorDraftChanged += surface.PSpeakerRefine;
        editor.CEditorDraftChanged += surface.PHeadwordFontRefine;
        editor.CEditorDraftChanged += surface.PExampleFontRefine;
        editor.CEditorDraftChanged += surface.PGlyphFontRefine;
        editor.CEditorDraftChanged += surface.PSentenceFrameRefine;
        editor.CEditorDraftChanged += surface.PCategoryLoad;
        editor.CEditorDraftChanged += surface.PMeaningRefine;
        editor.CEditorDraftChanged += surface.PCollocationRefine;
        editor.CEditorDraftChanged += surface.PEtymologyShow;
        editor.CEditorDraftChanged += surface.PPlaybackRefine;
        editor.CEditorDraftChanged += surface.PReflexPrepare;

        CWorkspace workspace = host.PWindowAtelier.CAtelierWorkspace;
        workspace.CWorkspaceOpened += surface.PSentenceLoad;
        workspace.CWorkspaceOpened += surface.PSpeakerLoad;
        workspace.CWorkspaceOpened += surface.PVolumeLoad;

        editor.CEditorObserverAttach(LObserver.LObserverCreate<Action>(static run => run()));
    }
}
