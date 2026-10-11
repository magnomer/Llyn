using System.Buffers.Binary;
using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TGalleyClerk
{
    [Fact]
    public async Task GalleyClerkDraw_TwoDrawsBackToBack_KeepsNewerProof()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TPressBoard board = new() { TPressBoardHold = new TaskCompletionSource() };
        await using LGalleyClerk galley = await TGalleyClerkOpen(workspace, board);
        LPressSetting upright = TInterface.TPressSettingCreate(null) with
        {
            LPressSettingOrientation = LPressOrientation.LPressOrientationPortrait,
        };
        LPressSetting wide = upright with { LPressSettingOrientation = LPressOrientation.LPressOrientationLandscape };

        Task<bool> older = galley.TGalleyClerkDraw(upright);
        Task<bool> newer = galley.TGalleyClerkDraw(wide);
        board.TPressBoardHold!.SetResult();

        Assert.False(await older);
        Assert.True(await newer);
        LPressPaper paper = galley.TGalleyClerkRead(0)!;
        Assert.True(paper.LPressPaperWidth > paper.LPressPaperHeight);
        Assert.Equal(2, board.TPressBoardProof.Count);
        Assert.True(board.TPressBoardProof[0].TPressProofDisposed);
        Assert.False(board.TPressBoardProof[1].TPressProofDisposed);
    }

    [Fact]
    public async Task GalleyClerkDraw_NoResolution_DrawsDefaultResolution()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        await using LGalleyClerk galley = await TGalleyClerkOpen(workspace, new TPressBoard());

        Assert.True(await galley.TGalleyClerkDraw(TInterface.TPressSettingCreate(null)));
        LPressImage? image = await galley.TGalleyClerkDraw(1, null);

        LPressPaper paper = galley.TGalleyClerkRead(1)!;
        Assert.Equal(
            (int)Math.Round(paper.LPressPaperWidth * 300),
            BinaryPrimitives.ReadInt32BigEndian(image!.LPressImagePng.AsSpan(16)));
    }

    [Fact]
    public async Task GalleyClerkDraw_GivenResolution_DrawsAtResolution()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        await using LGalleyClerk galley = await TGalleyClerkOpen(workspace, new TPressBoard());

        Assert.True(await galley.TGalleyClerkDraw(TInterface.TPressSettingCreate(null)));
        LPressImage? image = await galley.TGalleyClerkDraw(1, 50);

        LPressPaper paper = galley.TGalleyClerkRead(1)!;
        int across = BinaryPrimitives.ReadInt32BigEndian(image!.LPressImagePng.AsSpan(16));
        int down = BinaryPrimitives.ReadInt32BigEndian(image.LPressImagePng.AsSpan(20));
        Assert.Equal((int)Math.Round(paper.LPressPaperWidth * 50), across);
        Assert.True(down > across);
    }

    [Fact]
    public async Task GalleyClerkDraw_BeforeProof_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        await using LGalleyClerk galley = await TGalleyClerkOpen(workspace, new TPressBoard());

        Assert.Null(await galley.TGalleyClerkDraw(0, null));
    }

    [Fact]
    public async Task GalleyClerkDraw_PageBeyondCount_Throws()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        await using LGalleyClerk galley = await TGalleyClerkOpen(workspace, new TPressBoard());

        Assert.True(await galley.TGalleyClerkDraw(TInterface.TPressSettingCreate(null)));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => galley.TGalleyClerkDraw(3, null));
    }

    [Fact]
    public async Task GalleyClerkDraw_ProofReplacedMidPage_ReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TPressBoard board = new();
        await using LGalleyClerk galley = await TGalleyClerkOpen(workspace, board);
        LPressSetting upright = TInterface.TPressSettingCreate(null) with
        {
            LPressSettingOrientation = LPressOrientation.LPressOrientationPortrait,
        };
        Assert.True(await galley.TGalleyClerkDraw(upright));
        TPressProof held = board.TPressBoardProof[0];
        held.TPressProofHold = new TaskCompletionSource();

        Task<LPressImage?> page = galley.TGalleyClerkDraw(0, null);
        Assert.True(await galley.TGalleyClerkDraw(
            upright with { LPressSettingOrientation = LPressOrientation.LPressOrientationLandscape }));
        Assert.False(held.TPressProofDisposed);
        held.TPressProofHold.SetResult();

        Assert.Null(await page);
        Assert.True(held.TPressProofDisposed);
    }

    private static async Task<LGalleyClerk> TGalleyClerkOpen(TWorkspace workspace, TPressBoard board)
    {
        workspace.TWorkspacePress.TPressBoard = board;
        LRig rig = workspace.TWorkspaceRigCreate();
        LEntry water = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(
            TInterface.TEntryDraftCreate("water", "English", string.Empty, string.Empty, [], []));
        LPortraitPage page = TInterface.TPortraitClerkCreate(rig)
            .TPortraitClerkRead(water.LEntryId, TInterface.TPortraitLabelRead());
        return await TInterface.TPortraitPressCreate(rig).TPortraitClerkOpen(page);
    }
}
