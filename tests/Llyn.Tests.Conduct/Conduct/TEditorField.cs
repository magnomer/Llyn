using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEditorField
{
    [Fact]
    public void CardFieldSet_TypedTexts_WritesTitleExpressionAndDefinition()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);

        editor.TEditorFixtureField.CCardTitleSet(sheet, "Heat");
        editor.TEditorFixtureField.CCardExpressionSet(sheet, "on fire");
        editor.TEditorFixtureField.CCardMeaningSet(sheet, "burning");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        CCardDraft card = editor.TEditorDraftRead()!.CEntryDraftMeanings.Single(row => row.CCardDraftId == sheet);
        Assert.Equal("Heat", card.CCardDraftTitle.CStateWordingText);
        Assert.Equal("on fire", card.CCardDraftExpression.CStateWordingText);
        Assert.Equal("burning", card.CCardDraftMeaning.CStateWordingText);
    }

    [Fact]
    public void DraftRead_LinkedEntries_CarriesThemOnTheCardInItsOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        long gato = engine.TEngineTranslationCreate("gato", "Spanish").LEntryId;
        TEditorFixture editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long bare = TEditorSheetAdd(editor);
        editor.TEditorFixtureCard.CCardTranslationInsert(sheet, gato, string.Empty, string.Empty, 0);
        editor.TEditorFixtureCard.CCardTranslationInsert(sheet, chat, string.Empty, string.Empty, 0);

        IReadOnlyList<CTranslationTarget> targets = TEditorCardRead(editor, sheet).CCardDraftTranslation;

        Assert.Equal(["chat", "gato"], targets.Select(static target => target.CTranslationTargetHeadword));
        Assert.Empty(TEditorCardRead(editor, bare).CCardDraftTranslation);
    }

    [Fact]
    public void DraftChanged_LinkedEntry_CarriesTheLinkOnItsCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        TEditorFixture editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        editor.TEditorFixtureCard.CCardTranslationInsert(sheet, chat, string.Empty, string.Empty, 0);
        List<CEntryDraft> shown = [];
        editor.TEditorFixtureEntry.CEntryDraftChanged += shown.Add;

        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftResonate();

        CCardDraft card = shown[^1].CEntryDraftMeanings.Single(row => row.CCardDraftId == sheet);
        Assert.Equal(["chat"], card.CCardDraftTranslation.Select(static target => target.CTranslationTargetHeadword));
    }

    [Fact]
    public void DraftRead_EmptyDesk_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));

        Assert.Null(editor.TEditorDraftRead());
    }

    [Fact]
    public void ImageLocationSet_TypedLocation_WritesTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        CDesk desk = editor.TEditorFixtureDesk;
        desk.TDeskDefer(TInterface.TImageAdditionCreate(desk.CDeskId, sheet, 0));
        long image = TEditorCardRead(editor, sheet).CCardDraftImage[0].CImageDraftId;

        editor.TEditorFixtureImage.CImageLocationSet(image, "cat.png");
        desk.CDeskDraft.CDeskDraftPersist();

        Assert.Equal("cat.png", TEditorCardRead(editor, sheet).CCardDraftImage[0].CImageDraftLocation.CStateValueShown);
    }

    [Fact]
    public void VideoSet_TypedLocationAndSpan_WritesTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        CDesk desk = editor.TEditorFixtureDesk;
        desk.TDeskDefer(TInterface.TVideoAdditionCreate(desk.CDeskId, sheet, 0));
        long video = TEditorCardRead(editor, sheet).CCardDraftVideo[0].CVideoDraftId;

        editor.TEditorFixtureVideo.CVideoLocationSet(video, "cat.mp4");
        editor.TEditorFixtureVideo.CVideoSpanSet(video, "0:01-0:03");
        desk.CDeskDraft.CDeskDraftPersist();

        CVideoDraft row = TEditorCardRead(editor, sheet).CCardDraftVideo[0];
        Assert.Equal("cat.mp4", row.CVideoDraftLocation.CStateValueShown);
        Assert.Equal("0:01-0:03", row.CVideoDraftSpan.CStateValueShown);
    }

    [Fact]
    public void GlossSet_TypedText_WritesTheGloss()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long sentence = TEditorSentenceAdd(editor, sheet);
        editor.TEditorFixtureDesk.TDeskDefer(
            TInterface.TGlossAdditionCreate(editor.TEditorFixtureDesk.CDeskId, sheet, sentence, "French", 0));
        long gloss = TEditorGlossRead(editor, sheet)[0].CGlossDraftId;

        editor.TEditorFixtureSentence.CSentenceGlossSet(sheet, sentence, gloss, "le chat");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        Assert.Equal("le chat", TEditorGlossRead(editor, sheet)[0].CGlossDraftText.CStateValueShown);
    }

    [Fact]
    public void GlossAdd_PressedRow_AppendsAGlossInTheGlossLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long sentence = TEditorSentenceAdd(editor, sheet);
        editor.TEditorFixtureDesk.TDeskDefer(
            TInterface.TGlossAdditionCreate(editor.TEditorFixtureDesk.CDeskId, sheet, sentence, "French", 0));

        editor.TEditorFixtureSentence.CSentenceGlossAdd(sheet, sentence);

        IReadOnlyList<CGlossDraft> glosses = TEditorGlossRead(editor, sheet);
        Assert.Equal(["French", engine.TEngineGlossRead()], glosses.Select(static row => row.CGlossDraftLanguage));
    }

    [Fact]
    public void GlossRead_BlankAndNamedLanguage_CarriesTheHintOnlyForTheBlankOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long sentence = TEditorSentenceAdd(editor, sheet);
        long draft = editor.TEditorFixtureDesk.CDeskId;
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TGlossAdditionCreate(draft, sheet, sentence, string.Empty, 0));
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TGlossAdditionCreate(draft, sheet, sentence, "French", 1));

        IReadOnlyList<CGlossDraft> glosses = TEditorGlossRead(editor, sheet);

        Assert.False(glosses[0].CGlossDraftNamed);
        Assert.Equal("Example.Language", glosses[0].CGlossDraftHint);
        Assert.True(glosses[1].CGlossDraftNamed);
        Assert.Null(glosses[1].CGlossDraftHint);
    }

    [Fact]
    public void GlossRemoveAndLanguageSet_PickedGloss_DropsOneAndRetagsTheOther()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long sentence = TEditorSentenceAdd(editor, sheet);
        editor.TEditorFixtureSentence.CSentenceGlossAdd(sheet, sentence);
        editor.TEditorFixtureSentence.CSentenceGlossAdd(sheet, sentence);
        IReadOnlyList<CGlossDraft> added = TEditorGlossRead(editor, sheet);

        editor.TEditorFixtureSentence.CSentenceGlossRemove(sheet, sentence, added[0].CGlossDraftId);
        editor.TEditorFixtureSentence.CSentenceLanguageSet(sheet, sentence, added[1].CGlossDraftId, "German");

        CGlossDraft kept = Assert.Single(TEditorGlossRead(editor, sheet));
        Assert.Equal(added[1].CGlossDraftId, kept.CGlossDraftId);
        Assert.Equal("German", kept.CGlossDraftLanguage);
    }

    [Fact]
    public async Task EditorClose_HeldDraftAndSearch_LetsBothGo()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorFieldPrepare(engine);
        editor.TEditorFixtureEntry.CEntryHeadwordSet("water");
        editor.TEditorFixtureDesk.CDeskErrand.CErrandRecordingStart(0);

        editor.TEditorFixtureClose();

        Assert.False(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.Null(await editor.TEditorFixtureDesk.CDeskErrand.CErrandPreviewStart(
            new CRecording("Tagged", "https://example.test/gb.mp3", 0, true, "British")));
        Assert.Null(editor.TEditorFixtureDesk.CDeskStoredRead());
        Assert.Equal(string.Empty, editor.TEditorFixtureEntry.CEntryLanguage);
    }

    [Fact]
    public void EditorEntryOpen_ClosedThenReopened_HearsTheBoxFoldAgain()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long water = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("water", "English", string.Empty, string.Empty, [], [])).LEntryId;
        TEditorFixture toggled = TEditorFieldPrepare(engine);
        toggled.TEditorFixtureOpen(water);
        TEditorFixture editor = TEditorFieldPrepare(engine);
        editor.TEditorFixtureOpen(water);
        editor.TEditorFixtureClose();
        editor.TEditorFixtureOpen(water);
        int changed = 0;
        editor.TEditorFixtureEntry.CEntryDraftChanged += _ => changed++;

        toggled.TEditorFixtureFold.CFoldScriptSpread(true);

        Assert.Equal(1, changed);
        Assert.True(editor.TEditorFixtureFold.CFoldScriptOpened);
    }

    [Fact]
    public void EditorLanguage_HeldDraft_ReadsTheDraftsLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEntry entry = TEditorFieldPrepare(engine).TEditorFixtureEntry;

        entry.CEntryLanguageSet("English");

        Assert.Equal("English", entry.CEntryLanguage);
    }

    [Fact]
    public void EditorCreate_WorkspaceOpened_OpensAFreshDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        TEditorFixture editor = new(atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, [])));

        atelier.TAtelierStubOpen();

        Assert.True(editor.TEditorFixtureDesk.CDeskHeld);
        Assert.Null(editor.TEditorFixtureDesk.CDeskStoredRead());
    }

    [Fact]
    public void ObserverAttach_DraftEdited_ShowsTheDraftThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        int marshalled = 0;
        int shown = 0;
        TEditorFixture editor = TEditorFieldPrepare(engine, run =>
        {
            marshalled++;
            run();
        });
        editor.TEditorFixtureEntry.CEntryDraftChanged += _ => shown++;

        editor.TEditorFixtureEntry.CEntryHeadwordSet("water");
        editor.TEditorFixtureDesk.CDeskDraft.CDeskDraftPersist();

        Assert.True(marshalled > 0);
        Assert.True(shown > 0);
    }

    internal static TEditorFixture TEditorFieldPrepare(LEngine engine)
    {
        return TEditorFieldPrepare(engine, static run => run());
    }

    internal static TEditorFixture TEditorFieldPrepare(LEngine engine, Action<Action> marshal)
    {
        engine.TEngineDelaySet(0);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine, marshal));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        editor.TEditorFixtureOpen(null);
        return editor;
    }

    internal static long TEditorSheetAdd(TEditorFixture editor)
    {
        editor.TEditorFixtureDesk.TDeskDefer(TInterface.TRequestAdditionCreate(
            editor.TEditorFixtureDesk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        return editor.TEditorDraftRead()!.CEntryDraftMeanings[^1].CCardDraftId;
    }

    internal static CCardDraft TEditorCardRead(TEditorFixture editor, long sheet)
    {
        return editor.TEditorDraftRead()!.CEntryDraftMeanings.Single(row => row.CCardDraftId == sheet);
    }

    private static long TEditorSentenceAdd(TEditorFixture editor, long sheet)
    {
        CDesk desk = editor.TEditorFixtureDesk;
        desk.TDeskDefer(TInterface.TSentenceAdditionCreate(desk.CDeskId, sheet, 0));
        long sentence = TEditorCardRead(editor, sheet).CCardDraftSentence[0].CSentenceDraftId;
        editor.TEditorFixtureSentence.CSentenceTextSet(sheet, sentence, "the cat sat");
        desk.CDeskDraft.CDeskDraftPersist();
        return sentence;
    }

    private static IReadOnlyList<CGlossDraft> TEditorGlossRead(TEditorFixture editor, long sheet)
    {
        return TEditorCardRead(editor, sheet).CCardDraftSentence[0].CSentenceDraftExample!.CExampleDraftGloss;
    }
}
