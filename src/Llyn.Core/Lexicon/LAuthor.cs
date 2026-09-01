namespace Llyn.Core;

/// <summary>
/// One Author — independent data owned by nothing. No Reference contains an Author; any number of
/// References <em>reference</em> it instead, and the order an Author takes lives on each reference
/// rather than here, so the same Author can be first on one Reference and third on another.
/// <see cref="LAuthorId"/> is the identity — an opaque, program-generated stable id;
/// <see cref="LAuthorName"/> is display text and never identity, so renaming an Author leaves every
/// Reference to it untouched and two Authors reading alike remain distinct rows.
/// <para>
/// <c>Anonymous</c> is an ordinary specified Author like any other. It is never a stand-in for a
/// Reference whose author is unspecified or unknown — those are states on the Reference itself
/// (<see cref="LReference.LReferenceAuthorState"/>).
/// </para>
/// </summary>
/// <param name="LAuthorId">Opaque, program-generated stable id.</param>
/// <param name="LAuthorName">The author's name; display text, never identity.</param>
public sealed record LAuthor(
    string LAuthorId,
    string LAuthorName);
