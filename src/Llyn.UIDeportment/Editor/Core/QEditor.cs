using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEditor
{
    internal QEditor(CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        QEditorArea = editor;
        QEditorAnchor = CSoundingAnchor.CSoundingAnchorCreate(editor);
    }

    internal CEditor QEditorArea { get; }

    internal CSoundingAnchor QEditorAnchor { get; }

    internal void QEditorIntroduce(PWindow host, PEditor surface)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(surface);

        CEditor editor = QEditorArea;
        CDesk desk = editor.CEditorDesk;
        desk.CDeskStarted += surface.PEditorStartRefine;
        desk.CDeskStarted += surface.PReflexAnchorRefine;
        desk.CDeskStarted += surface.PReadingRefine;
        desk.CDeskFailed += host.PWindowFailureRefine;
        desk.CDeskStateChanged += surface.PEditorStateRefine;
        desk.CDeskRefused += host.PWindowEnvoy.CEnvoyFailureShow;
        desk.CDeskErrand.CErrandClipChanged += surface.PClipRefine;
        desk.CDeskErrand.CErrandNotationChanged += surface.PNotationRefine;
        editor.CEditorSounding.CSoundingChanged += surface.PReflexAnchorRefine;
        editor.CEditorSounding.CSoundingChanged += surface.PReadingRefine;
        editor.CEditorTimbre.CTimbreReflexChanged += surface.PReflexPendingRefine;
        editor.CEditorDisplay.CDisplaySound.CDisplayFoldChanged += surface.PReflexFoldRefine;
        editor.CEditorSentence.CSentenceReferenceChanged += surface.PSentenceCitationRefine;

        editor.CEditorDraftChanged += surface.PEditorDraftRefine;
        editor.CEditorDraftChanged += surface.PPronunciationRefine;
        editor.CEditorDraftChanged += surface.PTimbreRefine;
        editor.CEditorDraftChanged += surface.PAccentRefine;
        editor.CEditorDraftChanged += surface.PGlyphRefine;
        editor.CEditorDraftChanged += surface.PTranscriptionShow;
        editor.CEditorDraftChanged += surface.PReflexRefine;
        editor.CEditorDraftChanged += surface.PMarkerRefine;
        editor.CEditorDraftChanged += surface.PSpeakerRefine;
        editor.CEditorDraftChanged += surface.PHeadwordFontRefine;
        editor.CEditorDraftChanged += surface.PExampleFontRefine;
        editor.CEditorDraftChanged += surface.PGlyphFontRefine;
        editor.CEditorDraftChanged += surface.PSentenceFrameRefine;
        editor.CEditorDraftChanged += surface.PMeaningRefine;
        editor.CEditorDraftChanged += surface.PCollocationRefine;
        editor.CEditorDraftChanged += surface.PSentenceMentionRefine;
        editor.CEditorDraftChanged += surface.PEtymologyRefine;
        editor.CEditorDraftChanged += surface.PEtymologyMentionRefine;
        editor.CEditorDraftChanged += surface.PPlaybackRefine;

        CWorkspace workspace = host.PWindowAtelier.CAtelierWorkspace;
        workspace.CWorkspaceOpened += surface.PSentenceCitationRefine;
        workspace.CWorkspaceOpened += surface.PLanguageRefine;
        workspace.CWorkspaceOpened += surface.PVolumeRefine;

        editor.CEditorObserverAttach(LObserver.LObserverCreate<Action>(static run => run()));
    }
}
