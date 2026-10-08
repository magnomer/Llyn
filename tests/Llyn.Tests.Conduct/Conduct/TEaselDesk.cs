using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEaselDesk
{
    private const int TEaselDeskHold = 600000;

    [Fact]
    public void ImageAdd_ScenarioWithARow_AppendsABlankRowAfterIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TEaselScenarioPrepare(atelier);
        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageAdd(0);
        long first = TEaselImageRead(repertoire)[0].CImageDraftId;
        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageFileSet(first, "cat.png");

        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageAdd(0);

        IReadOnlyList<CImageDraft> rows = TEaselImageRead(repertoire);
        Assert.Equal(2, rows.Count);
        Assert.Equal(first, rows[0].CImageDraftId);
        Assert.Null(rows[1].CImageDraftLocation.CStateValueShown);
    }

    [Fact]
    public void ImageFileSet_ChosenFile_WritesTheScenarioRowAtOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TEaselScenarioPrepare(atelier);
        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageAdd(0);
        long image = TEaselImageRead(repertoire)[0].CImageDraftId;

        engine.TEngineDelaySet(TEaselDeskHold);
        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageFileSet(image, "cat.png");

        Assert.Equal("cat.png", TEaselImageRead(repertoire)[0].CImageDraftLocation.CStateValueShown);
    }

    [Fact]
    public void ImageFileSet_CancelledDialog_KeepsTheRowBlank()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TEaselScenarioPrepare(atelier);
        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageAdd(0);
        long image = TEaselImageRead(repertoire)[0].CImageDraftId;

        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageFileSet(image, null);

        Assert.Null(TEaselImageRead(repertoire)[0].CImageDraftLocation.CStateValueShown);
    }

    [Fact]
    public void ImageRemove_ScenarioRow_DropsTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TEaselScenarioPrepare(atelier);
        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageAdd(0);
        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageAdd(0);
        long kept = TEaselImageRead(repertoire)[1].CImageDraftId;

        repertoire.CRepertoirePlaywright.CPlaywrightImage.CImageRemove(TEaselImageRead(repertoire)[0].CImageDraftId);

        Assert.Equal([kept], TEaselImageRead(repertoire).Select(static row => row.CImageDraftId));
    }

    [Fact]
    public void VideoAdd_ScenarioWithARow_AppendsABlankRowAfterIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TEaselScenarioPrepare(atelier);
        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoAdd(0);
        long first = TEaselVideoRead(repertoire)[0].CVideoDraftId;
        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoFileSet(first, "cat.mp4");

        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoAdd(0);

        IReadOnlyList<CVideoDraft> rows = TEaselVideoRead(repertoire);
        Assert.Equal(2, rows.Count);
        Assert.Equal(first, rows[0].CVideoDraftId);
        Assert.Null(rows[1].CVideoDraftLocation.CStateValueShown);
    }

    [Fact]
    public void VideoFileSet_ChosenFile_WritesTheScenarioRowAtOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TEaselScenarioPrepare(atelier);
        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoAdd(0);
        long video = TEaselVideoRead(repertoire)[0].CVideoDraftId;

        engine.TEngineDelaySet(TEaselDeskHold);
        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoFileSet(video, "cat.mp4");

        Assert.Equal("cat.mp4", TEaselVideoRead(repertoire)[0].CVideoDraftLocation.CStateValueShown);
    }

    [Fact]
    public void VideoFileSet_CancelledDialog_KeepsTheRowBlank()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TEaselScenarioPrepare(atelier);
        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoAdd(0);
        long video = TEaselVideoRead(repertoire)[0].CVideoDraftId;

        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoFileSet(video, null);

        Assert.Null(TEaselVideoRead(repertoire)[0].CVideoDraftLocation.CStateValueShown);
    }

    [Fact]
    public void VideoRemove_ScenarioRow_DropsTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CRepertoire repertoire = TEaselScenarioPrepare(atelier);
        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoAdd(0);
        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoAdd(0);
        long kept = TEaselVideoRead(repertoire)[1].CVideoDraftId;

        repertoire.CRepertoirePlaywright.CPlaywrightVideo.CVideoRemove(TEaselVideoRead(repertoire)[0].CVideoDraftId);

        Assert.Equal([kept], TEaselVideoRead(repertoire).Select(static row => row.CVideoDraftId));
    }

    [Fact]
    public void ImageAdd_SecondCard_AppendsTheRowToThatCardOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        long other = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureImage.CImageAdd(other);
        long first = TEditorField.TEditorCardRead(editor, other).CCardDraftImage[0].CImageDraftId;

        editor.TEditorFixtureImage.CImageAdd(other);

        Assert.Empty(TEditorField.TEditorCardRead(editor, sheet).CCardDraftImage);
        IReadOnlyList<CImageDraft> rows = TEditorField.TEditorCardRead(editor, other).CCardDraftImage;
        Assert.Equal(2, rows.Count);
        Assert.Equal(first, rows[0].CImageDraftId);
    }

    [Fact]
    public void ImageRemove_RowOnSecondCard_DropsItFromItsCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        long other = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureImage.CImageAdd(sheet);
        editor.TEditorFixtureImage.CImageAdd(other);
        long image = TEditorField.TEditorCardRead(editor, other).CCardDraftImage[0].CImageDraftId;

        editor.TEditorFixtureImage.CImageRemove(image);

        Assert.Empty(TEditorField.TEditorCardRead(editor, other).CCardDraftImage);
        Assert.Single(TEditorField.TEditorCardRead(editor, sheet).CCardDraftImage);
    }

    [Fact]
    public void ImageFileSet_ChosenFile_WritesTheCardRowAtOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureImage.CImageAdd(sheet);
        long image = TEditorField.TEditorCardRead(editor, sheet).CCardDraftImage[0].CImageDraftId;

        engine.TEngineDelaySet(TEaselDeskHold);
        editor.TEditorFixtureImage.CImageFileSet(image, "cat.png");

        CImageDraft row = TEditorField.TEditorCardRead(editor, sheet).CCardDraftImage[0];
        Assert.Equal("cat.png", row.CImageDraftLocation.CStateValueShown);
    }

    [Fact]
    public void ImageFileSet_FileThatExists_CarriesItsAddress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureImage.CImageAdd(sheet);
        long image = TEditorField.TEditorCardRead(editor, sheet).CCardDraftImage[0].CImageDraftId;
        string file = Path.Combine(workspace.TWorkspaceFolder, "cat.png");
        File.WriteAllBytes(file, [0]);

        editor.TEditorFixtureImage.CImageFileSet(image, file);

        Assert.Equal(new Uri(file), TEditorField.TEditorCardRead(editor, sheet).CCardDraftImage[0].CImageDraftAddress);
    }

    [Fact]
    public void ImageFileSet_FileThatIsMissing_CarriesNoAddress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureImage.CImageAdd(sheet);
        long image = TEditorField.TEditorCardRead(editor, sheet).CCardDraftImage[0].CImageDraftId;

        editor.TEditorFixtureImage.CImageFileSet(image, Path.Combine(workspace.TWorkspaceFolder, "gone.png"));

        Assert.Null(TEditorField.TEditorCardRead(editor, sheet).CCardDraftImage[0].CImageDraftAddress);
    }

    [Fact]
    public void VideoFileSet_FileThatExists_CarriesItsScreen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureVideo.CVideoAdd(sheet);
        long video = TEditorField.TEditorCardRead(editor, sheet).CCardDraftVideo[0].CVideoDraftId;
        string file = Path.Combine(workspace.TWorkspaceFolder, "cat.mp4");
        File.WriteAllBytes(file, [0]);

        editor.TEditorFixtureVideo.CVideoFileSet(video, file);

        CScreen? screen = TEditorField.TEditorCardRead(editor, sheet).CCardDraftVideo[0].CVideoDraftScreen;
        Assert.Equal(new CScreen(new Uri(file), null), screen);
    }

    [Fact]
    public void VideoFileSet_FileThatIsMissing_CarriesNoScreen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureVideo.CVideoAdd(sheet);
        long video = TEditorField.TEditorCardRead(editor, sheet).CCardDraftVideo[0].CVideoDraftId;

        editor.TEditorFixtureVideo.CVideoFileSet(video, Path.Combine(workspace.TWorkspaceFolder, "gone.mp4"));

        Assert.Null(TEditorField.TEditorCardRead(editor, sheet).CCardDraftVideo[0].CVideoDraftScreen);
    }

    [Fact]
    public void VideoAdd_SecondCard_AppendsTheRowToThatCardOnly()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        long other = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureVideo.CVideoAdd(other);
        long first = TEditorField.TEditorCardRead(editor, other).CCardDraftVideo[0].CVideoDraftId;

        editor.TEditorFixtureVideo.CVideoAdd(other);

        Assert.Empty(TEditorField.TEditorCardRead(editor, sheet).CCardDraftVideo);
        IReadOnlyList<CVideoDraft> rows = TEditorField.TEditorCardRead(editor, other).CCardDraftVideo;
        Assert.Equal(2, rows.Count);
        Assert.Equal(first, rows[0].CVideoDraftId);
    }

    [Fact]
    public void VideoRemove_RowOnSecondCard_DropsItFromItsCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        long other = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureVideo.CVideoAdd(sheet);
        editor.TEditorFixtureVideo.CVideoAdd(other);
        long video = TEditorField.TEditorCardRead(editor, other).CCardDraftVideo[0].CVideoDraftId;

        editor.TEditorFixtureVideo.CVideoRemove(video);

        Assert.Empty(TEditorField.TEditorCardRead(editor, other).CCardDraftVideo);
        Assert.Single(TEditorField.TEditorCardRead(editor, sheet).CCardDraftVideo);
    }

    [Fact]
    public void VideoFileSet_ChosenFile_WritesTheCardRowAtOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TEditorFixture editor = TEditorField.TEditorFieldPrepare(engine);
        long sheet = TEditorField.TEditorSheetAdd(editor);
        editor.TEditorFixtureVideo.CVideoAdd(sheet);
        long video = TEditorField.TEditorCardRead(editor, sheet).CCardDraftVideo[0].CVideoDraftId;

        engine.TEngineDelaySet(TEaselDeskHold);
        editor.TEditorFixtureVideo.CVideoFileSet(video, "cat.mp4");

        CVideoDraft row = TEditorField.TEditorCardRead(editor, sheet).CCardDraftVideo[0];
        Assert.Equal("cat.mp4", row.CVideoDraftLocation.CStateValueShown);
    }

    private static CRepertoire TEaselScenarioPrepare(CAtelier atelier)
    {
        CRepertoire repertoire = TRepertoire.TRepertoirePrepare(atelier, TEnvoyFake.TEnvoyCreate(false, []));
        repertoire.CRepertoireDiptych.CDiptychEntryCreate();
        return repertoire;
    }

    private static IReadOnlyList<CImageDraft> TEaselImageRead(CRepertoire repertoire)
    {
        return repertoire.TRepertoireScenarioRead()!.CSituationDraftImage;
    }

    private static IReadOnlyList<CVideoDraft> TEaselVideoRead(CRepertoire repertoire)
    {
        return repertoire.TRepertoireScenarioRead()!.CSituationDraftVideo;
    }
}
