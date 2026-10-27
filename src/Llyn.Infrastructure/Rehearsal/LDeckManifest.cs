using System.Collections.Generic;

namespace Llyn.Infrastructure;

internal sealed record LDeckManifest(
    IReadOnlyDictionary<
            long,
            (string LDeckManifestDigest, IReadOnlyList<string> LDeckManifestMedia, string LDeckManifestWritten)>
        LDeckManifestDecks);
