using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplay
{
    [Fact]
    public void DisplayStampFormat_UnreadableText_ReturnsEmpty()
    {
        Assert.Empty(TInterfaceConduct.TDisplayStampFormat("not a moment"));
        Assert.Empty(TInterfaceConduct.TDisplayStampFormat(null));
    }

    [Fact]
    public void DisplayFoldSet_CurrentValue_KeepsFold()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);

        sound.TDisplayFoldSet(true);
        sound.TDisplayFoldSet(true);

        Assert.True(sound.LDisplayFoldOpened);
    }

    [Fact]
    public void DisplaySoundClear_ShownDraft_DropsDraftAndEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);
        sound.TDisplaySoundShow(7, TDisplayDraftCreate([]));

        sound.TDisplaySoundClear();

        Assert.Null(sound.LDisplayShown);
        Assert.Null(sound.LDisplayEntry);
    }

    [Fact]
    public void DisplayReflexRead_LoadedReflex_PrefersIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDisplaySound sound = TInterfaceConduct.TDisplaySoundCreate(engine);
        LEntryDraft draft = TDisplayDraftCreate([TInterface.TReflexDraftCreate("Korean", "", "a")]);
        long id = TExemplar.TExemplarSave(engine, draft)[0];
        sound.TDisplaySoundShow(id, TDisplayDraftCreate([]));

        sound.TDisplayReflexLoad();

        Assert.Equal("a", Assert.Single(sound.TDisplayReflexRead()).LReflexDraftText);
    }

    [Fact]
    public void DisplayFrequencyRead_NoEntry_ReturnsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);

        Assert.Null(editor.CEditorDisplay.TDisplayFrequencyRead(null, "once in {0} words"));
    }

    [Fact]
    public void DisplayFrequencyRead_EntryWithoutFrequency_ReturnsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long id = TExemplar.TExemplarSave(engine, TDisplayDraftCreate([]))[0];
        CEditor editor = TInterfaceConduct.TEditorCreate(engine);

        Assert.Null(editor.CEditorDisplay.TDisplayFrequencyRead(id, "once in {0} words"));
    }

    private static LEntryDraft TDisplayDraftCreate(IReadOnlyList<LReflexDraft> reflexes) =>
        TInterface.TEntryDraftCreate(
            "kindle",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: reflexes);
}
