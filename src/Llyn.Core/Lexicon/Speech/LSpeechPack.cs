using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LSpeechPack(
    IReadOnlyList<LSpeechValue> LSpeechPackValues,
    IReadOnlyList<LFeature> LSpeechPackFeatures,
    IReadOnlyList<LMorphology> LSpeechPackMorphology,
    IReadOnlyList<LParadigm> LSpeechPackParadigms);
