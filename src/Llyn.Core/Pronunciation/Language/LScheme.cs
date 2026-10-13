using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LScheme(string LSchemeName, IReadOnlyList<LSourceSpec> LSchemeSources);
