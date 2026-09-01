namespace Llyn.Core;

/// <summary>
/// One bibliographic Reference — the material an Entry or an Example cites. A Reference is
/// independent: no Entry, Example, or Author owns it, and it owns none of them. Entries reach it
/// through ordered reference rows and an Example through its single source column, so deleting a
/// citing row never touches the Reference and deleting the Reference never touches its citers.
/// <para>
/// There is no source-type discriminator. One shape describes an article, a television episode, a
/// video, or a picture alike: the fields that mean something for the material at hand are specified
/// and the rest stay <see cref="LState.LStateUnspecified"/>. Each field is an
/// <see cref="LReferenceValue"/> so "never entered", "recorded as unknown", and "this value" stay
/// distinct. <see cref="LReferenceAuthorState"/> is that same distinction for the authorship as a
/// whole — the Authors themselves are separate rows attached in order, so an
/// <see cref="LState.LStateSpecified"/> authorship may name one Author, several, or the specified
/// Author <c>Anonymous</c>.
/// </para>
/// </summary>
/// <param name="LReferenceId">Opaque, program-generated stable id.</param>
/// <param name="LReferenceTitle">Title of the material; for a television episode, the episode name.</param>
/// <param name="LReferenceProgram">Program name, when the material belongs to one.</param>
/// <param name="LReferenceChannel">Channel name, when the material belongs to one.</param>
/// <param name="LReferenceYear">Publication or release year.</param>
/// <param name="LReferenceUrl">Address the material was found at.</param>
/// <param name="LReferenceAuthorState">Whether the authorship is unspecified, unknown, or specified by attached Authors.</param>
public sealed record LReference(
    string LReferenceId,
    LReferenceValue LReferenceTitle,
    LReferenceValue LReferenceProgram,
    LReferenceValue LReferenceChannel,
    LReferenceValue LReferenceYear,
    LReferenceValue LReferenceUrl,
    LState LReferenceAuthorState);
