using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LDeckManifest(
    IReadOnlyDictionary<
            long,
            (string LDeckManifestDigest, IReadOnlyList<string> LDeckManifestMedia, string LDeckManifestWritten)>
        LDeckManifestDecks);
