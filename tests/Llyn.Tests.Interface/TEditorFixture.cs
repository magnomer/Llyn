using System;
using Llyn.Conduct;

namespace Llyn.Tests;

internal sealed class TEditorFixture
{
    internal TEditorFixture(CEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        TEditorFixtureEditor = editor;
    }

    internal static TEditorFixture TEditorFixtureCreate(CAtelier atelier, CEnvoy envoy) =>
        new(CEditor.CEditorCreate(atelier, envoy));

    internal CEditor TEditorFixtureEditor { get; }

    internal CDesk TEditorFixtureDesk => TEditorFixtureEditor.CEditorDesk;

    internal CEntry TEditorFixtureEntry => TEditorFixtureEditor.CEditorEntry;

    internal CDisplay TEditorFixtureDisplay => TEditorFixtureEditor.CEditorDisplay;

    internal CCard TEditorFixtureCard => TEditorFixtureEditor.CEditorCard;

    internal CSentence TEditorFixtureSentence => TEditorFixtureEditor.CEditorSentence;

    internal CSounding TEditorFixtureSounding => TEditorFixtureEditor.CEditorSounding;

    internal CFold TEditorFixtureFold => TEditorFixtureEditor.CEditorFold;

    internal CEsteem TEditorFixtureEsteem => TEditorFixtureEditor.CEditorEsteem;

    internal CTimbre TEditorFixtureTimbre => TEditorFixtureEditor.CEditorTimbre;

    internal CKindred TEditorFixtureKindred => TEditorFixtureEditor.CEditorKindred;

    internal CPlayback TEditorFixturePlayback => TEditorFixtureEditor.CEditorPlayback;

    internal CTranscription TEditorFixtureTranscription => TEditorFixtureEditor.CEditorTranscription;

    internal CCardSpeech TEditorFixtureSpeech => TEditorFixtureEditor.CEditorSpeech;

    internal CCardField TEditorFixtureField => TEditorFixtureEditor.CEditorField;

    internal CImage TEditorFixtureImage => TEditorFixtureEditor.CEditorImage;

    internal CVideo TEditorFixtureVideo => TEditorFixtureEditor.CEditorVideo;

    internal bool TEditorFixtureOwned => TEditorFixtureEditor.CEditorOwned;

    internal void TEditorFixtureOpen(long? id) => TEditorFixtureEditor.CEditorEntryOpen(id);

    internal void TEditorFixtureClose() => TEditorFixtureEditor.CEditorClose();

    internal void TEditorFixtureSave() => TEditorFixtureEditor.CEditorEntrySave();

    internal void TEditorFixtureUndo() => TEditorFixtureEditor.CEditorEntryUndo();
}
