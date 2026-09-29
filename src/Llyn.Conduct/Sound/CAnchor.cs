using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CAnchor(IReadOnlyList<CAnchorRow> CAnchorRows, bool CAnchorEmpty);
