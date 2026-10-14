using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeckManifestFile
{
    [Fact]
    public void DeckManifestSave_ThenRead_RoundTripsDecks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDeckManifestFile file = TInterface.TDeckManifestCreate(workspace.TWorkspaceFolder);
        LDeckManifest manifest = new(
            new Dictionary<long, (string, IReadOnlyList<string>, string)>
            {
                [3] = ("a1b2", ["b.png", "a.mp3"], "2026-10-14 09:30:00"),
                [7] = ("c3d4", [], "2026-10-14 10:00:00"),
            });

        file.TDeckManifestSave(manifest);

        LDeckManifest read = file.TDeckManifestRead();
        Assert.Equal([3L, 7L], read.LDeckManifestDecks.Keys.Order());
        Assert.Equal("a1b2", read.LDeckManifestDecks[3].LDeckManifestDigest);
        Assert.Equal(["a.mp3", "b.png"], read.LDeckManifestDecks[3].LDeckManifestMedia);
        Assert.Equal("2026-10-14 09:30:00", read.LDeckManifestDecks[3].LDeckManifestWritten);
        Assert.Equal("c3d4", read.LDeckManifestDecks[7].LDeckManifestDigest);
        Assert.Empty(read.LDeckManifestDecks[7].LDeckManifestMedia);
        Assert.Equal("2026-10-14 10:00:00", read.LDeckManifestDecks[7].LDeckManifestWritten);
    }
}
