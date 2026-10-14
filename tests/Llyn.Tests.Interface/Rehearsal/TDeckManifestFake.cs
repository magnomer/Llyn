using Llyn.Core;

namespace Llyn.Tests;

public sealed class TDeckManifestFake : LDeckManifestVault
{
    public LDeckManifest TDeckManifestStored { get; private set; } =
        new(new Dictionary<long, (string, IReadOnlyList<string>, string)>());

    public LDeckManifest LDeckManifestRead() => TDeckManifestStored;

    public void LDeckManifestSave(LDeckManifest manifest) => TDeckManifestStored = manifest;
}
