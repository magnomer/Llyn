using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LManifest(IReadOnlyDictionary<string, string> LManifestDigest);
