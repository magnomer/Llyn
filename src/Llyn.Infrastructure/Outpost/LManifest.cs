using System.Collections.Generic;

namespace Llyn.Infrastructure;

internal sealed record LManifest(IReadOnlyDictionary<string, string> LManifestDigest, string LManifestRealm);
