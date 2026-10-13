using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CFanqieGroup(
    string CFanqieGroupHeading,
    string CFanqieGroupLabel,
    string CFanqieGroupSource,
    IReadOnlyList<string> CFanqieGroupStems,
    IReadOnlyList<CFanqieRow> CFanqieGroupRows);
