using System.Collections.Generic;

namespace Llyn.Core;

/// <summary>
/// One Example — the first independent shared entity in the lexicon. An Example is owned by nothing: no
/// Entry, Meaning, or Collocation contains it. Any number of them <em>reference</em> it instead, and the
/// order an Example appears in lives on each reference rather than here, so the same Example can sit
/// first under one Entry and third under a Meaning. <see cref="LExampleId"/> is the identity — an opaque,
/// program-generated stable id; <see cref="LExampleText"/> is display text and never identity, so two
/// Examples with identical text remain distinct rows.
/// <para>
/// An Example owns its <see cref="LExampleTranslations"/> and references <em>at most one</em> Source
/// through <see cref="LExampleSourceId"/>. That reference is a pointer, not ownership: clearing it or
/// deleting the Example never touches the Source.
/// </para>
/// </summary>
/// <param name="LExampleId">Opaque, program-generated stable id.</param>
/// <param name="LExampleLanguage">Language code the example text is written in.</param>
/// <param name="LExampleText">The example text; display text, never identity.</param>
/// <param name="LExampleLocal">Optional local-script rendering of the text; <c>null</c> when absent.</param>
/// <param name="LExampleSourceId">Id of the single referenced Source, or <c>null</c> when the Example cites none.</param>
/// <param name="LExampleTranslations">The Example's owned translations, in order.</param>
public sealed record LExample(
    string LExampleId,
    string LExampleLanguage,
    string LExampleText,
    string? LExampleLocal,
    string? LExampleSourceId,
    IReadOnlyList<LTranslation> LExampleTranslations);
