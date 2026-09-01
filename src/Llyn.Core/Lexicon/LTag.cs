namespace Llyn.Core;

/// <summary>
/// One Tag — independent data owned by nothing. No Entry, Meaning, or Collocation contains a Tag; any
/// number of Meanings and Collocations <em>reference</em> it instead, and the order a Tag appears in
/// lives on each reference rather than here, so the same Tag can sit first under one Meaning and third
/// under a Collocation. <see cref="LTagId"/> is the identity — an opaque, program-generated stable id;
/// <see cref="LTagText"/> is display text and never identity, so editing the text leaves the id and
/// every reference to it untouched, and two Tags reading alike remain distinct rows.
/// </summary>
/// <param name="LTagId">Opaque, program-generated stable id.</param>
/// <param name="LTagText">The tag text; display text, never identity.</param>
public sealed record LTag(
    string LTagId,
    string LTagText);
