using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CMentionSense(string CMentionSenseKey, IReadOnlyList<CMeaning> CMentionSenseRow);
