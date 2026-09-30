using System;
using System.Collections.Generic;
using System.Linq;
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
        CEditor editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);

        editor.CEditorField.CCardTitleSet(sheet, "Heat");
        editor.CEditorField.CCardExpressionSet(sheet, "on fire");
        editor.CEditorField.CCardMeaningSet(sheet, "burning");
        editor.CEditorDesk.CDeskPersist();

        CCardDraft card = editor.CEditorDraftRead()!.CEntryDraftMeanings.Single(row => row.CCardDraftId == sheet);
        Assert.Equal("Heat", card.CCardDraftTitle.CStateValueShown);
        Assert.Equal("on fire", card.CCardDraftExpression.CStateValueShown);
        Assert.Equal("burning", card.CCardDraftMeaning.CStateValueShown);
    }

    [Fact]
    public void DraftRead_LinkedEntries_CarriesThemOnTheCardInItsOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long chat = engine.TEngineTranslationCreate("chat", "French").LEntryId;
        long gato = engine.TEngineTranslationCreate("gato", "Spanish").LEntryId;
        CEditor editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long bare = TEditorSheetAdd(editor);
        editor.CEditorCard.CCardTranslationInsert(sheet, gato, "gato", "Spanish", 0);
        editor.CEditorCard.CCardTranslationInsert(sheet, chat, "chat", "French", 0);

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
        CEditor editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        editor.CEditorCard.CCardTranslationInsert(sheet, chat, "chat", "French", 0);
        List<CEntryDraft> shown = [];
        editor.CEditorDraftChanged += shown.Add;

        editor.CEditorDesk.CDeskDraftResonate();

        CCardDraft card = shown[^1].CEntryDraftMeanings.Single(row => row.CCardDraftId == sheet);
        Assert.Equal(["chat"], card.CCardDraftTranslation.Select(static target => target.CTranslationTargetHeadword));
    }

    [Fact]
    public void DraftRead_EmptyDesk_ReadsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);

        Assert.Null(editor.CEditorDraftRead());
    }

    [Fact]
    public void ImageLocationSet_TypedLocation_WritesTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        editor.CEditorDesk.TDeskDefer(TInterface.TImageAdditionCreate(editor.CEditorDesk.CDeskId, sheet, 0));
        long image = TEditorCardRead(editor, sheet).CCardDraftImage[0].CImageDraftId;

        editor.CEditorImage.CImageLocationSet(image, "cat.png");
        editor.CEditorDesk.CDeskPersist();

        Assert.Equal("cat.png", TEditorCardRead(editor, sheet).CCardDraftImage[0].CImageDraftLocation.CStateValueShown);
    }

    [Fact]
    public void VideoSet_TypedLocationAndSpan_WritesTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        editor.CEditorDesk.TDeskDefer(TInterface.TVideoAdditionCreate(editor.CEditorDesk.CDeskId, sheet, 0));
        long video = TEditorCardRead(editor, sheet).CCardDraftVideo[0].CVideoDraftId;

        editor.CEditorVideo.CVideoLocationSet(video, "cat.mp4");
        editor.CEditorVideo.CVideoSpanSet(video, "0:01-0:03");
        editor.CEditorDesk.CDeskPersist();

        CVideoDraft row = TEditorCardRead(editor, sheet).CCardDraftVideo[0];
        Assert.Equal("cat.mp4", row.CVideoDraftLocation.CStateValueShown);
        Assert.Equal("0:01-0:03", row.CVideoDraftSpan.CStateValueShown);
    }

    [Fact]
    public void GlossSet_TypedText_WritesTheGloss()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long sentence = TEditorSentenceAdd(editor, sheet);
        editor.CEditorDesk.TDeskDefer(
            TInterface.TGlossAdditionCreate(editor.CEditorDesk.CDeskId, sheet, sentence, "French", 0));
        long gloss = TEditorGlossRead(editor, sheet)[0].CGlossDraftId;

        editor.CEditorSentence.CSentenceGlossSet(sheet, sentence, gloss, "le chat");
        editor.CEditorDesk.CDeskPersist();

        Assert.Equal("le chat", TEditorGlossRead(editor, sheet)[0].CGlossDraftText.CStateValueShown);
    }

    [Fact]
    public void GlossAdd_PressedRow_AppendsAGlossInTheGlossLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long sentence = TEditorSentenceAdd(editor, sheet);
        editor.CEditorDesk.TDeskDefer(
            TInterface.TGlossAdditionCreate(editor.CEditorDesk.CDeskId, sheet, sentence, "French", 0));

        editor.CEditorSentence.CSentenceGlossAdd(sheet, sentence);

        IReadOnlyList<CGlossDraft> glosses = TEditorGlossRead(editor, sheet);
        Assert.Equal(["French", engine.TEngineGlossRead()], glosses.Select(static row => row.CGlossDraftLanguage));
    }

    [Fact]
    public void GlossRead_BlankAndNamedLanguage_CarriesTheHintOnlyForTheBlankOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long sentence = TEditorSentenceAdd(editor, sheet);
        long draft = editor.CEditorDesk.CDeskId;
        editor.CEditorDesk.TDeskDefer(TInterface.TGlossAdditionCreate(draft, sheet, sentence, string.Empty, 0));
        editor.CEditorDesk.TDeskDefer(TInterface.TGlossAdditionCreate(draft, sheet, sentence, "French", 1));

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
        CEditor editor = TEditorFieldPrepare(engine);
        long sheet = TEditorSheetAdd(editor);
        long sentence = TEditorSentenceAdd(editor, sheet);
        editor.CEditorSentence.CSentenceGlossAdd(sheet, sentence);
        editor.CEditorSentence.CSentenceGlossAdd(sheet, sentence);
        IReadOnlyList<CGlossDraft> added = TEditorGlossRead(editor, sheet);

        editor.CEditorSentence.CSentenceGlossRemove(sheet, sentence, added[0].CGlossDraftId);
        editor.CEditorSentence.CSentenceLanguageSet(sheet, sentence, added[1].CGlossDraftId, "German");

        CGlossDraft kept = Assert.Single(TEditorGlossRead(editor, sheet));
        Assert.Equal(added[1].CGlossDraftId, kept.CGlossDraftId);
        Assert.Equal("German", kept.CGlossDraftLanguage);
    }

    [Fact]
    public void EditorClose_HeldDraftAndSearch_LetsBothGo()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorFieldPrepare(engine);
        editor.CEditorHeadwordSet("water");
        editor.CEditorDesk.CDeskErrand.CErrandRecordingStart("water", 0, static _ => { });

        editor.CEditorClose();

        Assert.False(editor.CEditorDesk.CDeskHeld);
        Assert.False(editor.CEditorDesk.CDeskErrand.CErrandRecordingHeld);
        Assert.Null(editor.CEditorEntry);
        Assert.Equal(string.Empty, editor.CEditorLanguage);
    }

    [Fact]
    public void EditorLanguage_HeldDraft_ReadsTheDraftsLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorFieldPrepare(engine);

        editor.CEditorLanguageSet("English");

        Assert.Equal("English", editor.CEditorLanguage);
    }

    [Fact]
    public void EditorCreate_WorkspaceOpened_OpensAFreshDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CEditor editor = atelier.CAtelierInputCreate(TEnvoyFake.TEnvoyCreate(false, []));

        atelier.CAtelierOpen();

        Assert.True(editor.CEditorDesk.CDeskHeld);
        Assert.Null(editor.CEditorEntry);
    }

    [Fact]
    public void ObserverAttach_DraftEdited_ShowsTheDraftThroughTheMarshal()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TEditorFieldPrepare(engine);
        int marshalled = 0;
        int shown = 0;
        editor.CEditorObserverAttach(run =>
        {
            marshalled++;
            run();
        });
        editor.CEditorDraftChanged += _ => shown++;

        editor.CEditorHeadwordSet("water");
        editor.CEditorDesk.CDeskPersist();

        Assert.True(marshalled > 0);
        Assert.True(shown > 0);
    }

    private static CEditor TEditorFieldPrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);
        editor.TEditorVistaRestore(engine.TEngineVistaStart("input", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(null);
        return editor;
    }

    private static long TEditorSheetAdd(CEditor editor)
    {
        editor.CEditorDesk.TDeskDefer(TInterface.TRequestAdditionCreate(
            editor.CEditorDesk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        return editor.CEditorDraftRead()!.CEntryDraftMeanings[^1].CCardDraftId;
    }

    private static CCardDraft TEditorCardRead(CEditor editor, long sheet)
    {
        return editor.CEditorDraftRead()!.CEntryDraftMeanings.Single(row => row.CCardDraftId == sheet);
    }

    private static long TEditorSentenceAdd(CEditor editor, long sheet)
    {
        editor.CEditorDesk.TDeskDefer(TInterface.TSentenceAdditionCreate(editor.CEditorDesk.CDeskId, sheet, 0));
        long sentence = TEditorCardRead(editor, sheet).CCardDraftSentence[0].CSentenceDraftId;
        editor.CEditorSentence.CSentenceTextSet(sheet, sentence, "the cat sat");
        editor.CEditorDesk.CDeskPersist();
        return sentence;
    }

    private static IReadOnlyList<CGlossDraft> TEditorGlossRead(CEditor editor, long sheet)
    {
        return TEditorCardRead(editor, sheet).CCardDraftSentence[0].CSentenceDraftExample!.CExampleDraftGloss;
    }
}
