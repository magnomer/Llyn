using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLiveryStem(LStemPage LLiveryStemPage, IReadOnlyList<LEntry> LLiveryStemEntry)
{
    public const string LLiveryStemKind = "stem";
}
