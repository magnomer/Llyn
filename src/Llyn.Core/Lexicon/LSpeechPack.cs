using System.Collections.Generic;

namespace Llyn.Core;

/// <summary>
/// The display vocabulary one language pack declares: the parts of speech the language uses and the
/// morphology features each of those parts of speech takes. Loaded from
/// <c>languages/&lt;Lang&gt;/vocabulary.json</c> and written into <c>part_of_speech_value</c> and
/// <c>morphology_value</c>, which are keyed by language, so a pack is the only place a language's
/// vocabulary is stated and no such fact is compiled in.
/// <para>
/// The two lists travel together because the morphology rows are keyed by the part of speech they
/// belong to: reading one without the other leaves rows that name a part of speech nothing declared.
/// </para>
/// </summary>
/// <param name="LSpeechPackValues">The parts of speech, in the order the pack lists them.</param>
/// <param name="LSpeechPackMorphology">The morphology rows, in the order the pack lists them.</param>
public sealed record LSpeechPack(
    IReadOnlyList<LSpeechValue> LSpeechPackValues,
    IReadOnlyList<LMorphology> LSpeechPackMorphology);
