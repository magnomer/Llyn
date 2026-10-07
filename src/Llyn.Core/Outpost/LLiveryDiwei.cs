using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLiveryDiwei(
    string LLiveryDiweiKind,
    LDiweiPage LLiveryDiweiPage,
    IReadOnlyList<LEntry> LLiveryDiweiEntry);
