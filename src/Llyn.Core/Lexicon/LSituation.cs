namespace Llyn.Core;

/// <summary>
/// One Situation — the usage context a Meaning or Collocation belongs to, and independent data owned by
/// nothing. No Entry, Meaning, or Collocation contains a Situation; any number of Meanings and
/// Collocations <em>reference</em> it instead, and the order a Situation appears in lives on each
/// reference rather than here. <see cref="LSituationId"/> is the identity — an opaque,
/// program-generated stable id; the title, description, and kind are visible data and never identity, so
/// editing any of them leaves the id and every reference to it untouched.
/// </summary>
/// <param name="LSituationId">Opaque, program-generated stable id.</param>
/// <param name="LSituationTitle">The situation title; display text, never identity.</param>
/// <param name="LSituationDescription">Description of the situation, or <c>null</c> when absent.</param>
/// <param name="LSituationKind">Situation/context classification, or <c>null</c> when unclassified.</param>
public sealed record LSituation(
    string LSituationId,
    string LSituationTitle,
    string? LSituationDescription,
    string? LSituationKind);
