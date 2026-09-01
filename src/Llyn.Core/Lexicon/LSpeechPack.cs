using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSpeechPack(
    IReadOnlyList<LSpeechValue> LSpeechPackValues,
    IReadOnlyList<LMorphology> LSpeechPackMorphology);
