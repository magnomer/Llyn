using System.IO;
using Llyn.UIDeportment.Capsule;
using Xunit;

namespace Llyn.Tests;

public sealed class TCapsule
{
    [Fact]
    public void CapsuleSave_ThenRead_RoundTripsState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LCapsuleContent state = TInterfaceCapsule.TCapsuleContentCreate(
            TInterfaceCapsule.TCapsuleWindowCreate(10, 20, 800, 600, true),
            false,
            TInterfaceCapsule.TCapsuleColumnCreate("library", 400, null),
            TInterfaceCapsule.TCapsuleColumnCreate("tenor", 320, 300));

        TInterfaceCapsule.TCapsuleSave(workspace.TWorkspaceFolder, state);
        LCapsuleContent read = TInterfaceCapsule.TCapsuleRead(workspace.TWorkspaceFolder);

        Assert.Equal(state.LCapsuleContentWindow, read.LCapsuleContentWindow);
        Assert.False(read.LCapsuleContentLinked);
        Assert.Equal(state.LCapsuleContentColumn, read.LCapsuleContentColumn);
        Assert.False(File.Exists(Path.Combine(workspace.TWorkspaceFolder, "capsule.json.tmp")));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("{ not json")]
    [InlineData("[1, 2]")]
    public void CapsuleRead_AbsentOrUnusable_ReadsDefaults(string? text)
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        if (text is not null)
        {
            File.WriteAllText(Path.Combine(workspace.TWorkspaceFolder, "capsule.json"), text);
        }

        LCapsuleContent state = TInterfaceCapsule.TCapsuleRead(workspace.TWorkspaceFolder);

        Assert.Null(state.LCapsuleContentWindow);
        Assert.True(state.LCapsuleContentLinked);
        Assert.Empty(state.LCapsuleContentColumn ?? []);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CapsuleRead_BlankRoot_ReadsDefaults(string? root)
    {
        LCapsuleContent state = TInterfaceCapsule.TCapsuleRead(root);

        Assert.Null(state.LCapsuleContentWindow);
        Assert.True(state.LCapsuleContentLinked);
        Assert.Empty(state.LCapsuleContentColumn ?? []);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CapsuleSave_BlankRoot_WritesNothing(string? root)
    {
        LCapsuleContent state = TInterfaceCapsule.TCapsuleContentCreate(null, false);

        TInterfaceCapsule.TCapsuleSave(root, state);

        Assert.False(File.Exists("capsule.json"));
        Assert.False(File.Exists(Path.Combine(root ?? string.Empty, "capsule.json")));
    }
}
