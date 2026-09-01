using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The Tag half of the engine: a Tag created, read, renamed, referenced from a card and let go of.
/// A Tag is independent data owned by nothing — any number of Meanings and Collocations reference it,
/// each holding the order it takes there — so the seams here separate the row from the references to
/// it, and none of them lets one decide the other's fate by accident.
/// <para>
/// Which side an id names arrives as an <see cref="LOwner"/> rather than in the method's name: a name
/// carries three components after its prefix, and a method per side would need four. A Tag hangs from
/// a Meaning or a Collocation and from nothing else, so any other side is refused rather than guessed.
/// </para>
/// <para>
/// Two ways of parting a card from a Tag exist because they are two intentions.
/// <see cref="LEngineTagDetach"/> removes one reference and leaves the row standing, which is what
/// editing a card does — the same rule the card path follows, where dropping a Tag from a card never
/// deletes what other cards may still use. <see cref="LEngineTagRemove"/> removes the reference and,
/// when it was the last one, the row with it, so a Tag no card mentions any more does not linger as
/// data nothing can reach. Both hold one session, so no caller can compose them into a half-done state.
/// </para>
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Creates <paramref name="tag"/> and returns it with its assigned id. The text is never identity:
    /// creating a Tag whose words were saved before is a second Tag, not the first one again.
    /// </summary>
    public LTag LEngineTagCreate(LTag tag)
    {
        ArgumentNullException.ThrowIfNull(tag);
        return new LTagArchive(_lEngineDatabase).LTagCreate(tag);
    }

    /// <summary>Reads the Tag for <paramref name="id"/>, or <c>null</c> when no Tag has that id.</summary>
    public LTag? LEngineTagRead(string id)
    {
        return new LTagArchive(_lEngineDatabase).LTagRead(id);
    }

    /// <summary>
    /// Reads the Tags the Meaning or Collocation identified by <paramref name="ownerId"/> references,
    /// in the order that side holds them.
    /// </summary>
    public IReadOnlyList<LTag> LEngineTagRead(string ownerId, LOwner owner)
    {
        LTagArchive tags = new(_lEngineDatabase);
        return LEngineOwnerCheck(owner)
            ? tags.LTagCollocationRead(ownerId)
            : tags.LTagSenseRead(ownerId);
    }

    /// <summary>
    /// Renames the Tag <paramref name="tag"/> identifies. Every card referencing it reads the new text:
    /// the references point at the row, never at a copy of its words.
    /// </summary>
    public void LEngineTagUpdate(LTag tag)
    {
        new LTagArchive(_lEngineDatabase).LTagUpdate(tag);
    }

    /// <summary>
    /// References the Tag identified by <paramref name="tagId"/> from the Meaning or Collocation
    /// identified by <paramref name="ownerId"/> at <paramref name="position"/> in that side's order,
    /// renumbering the set around it so the positions stay contiguous.
    /// </summary>
    public void LEngineTagAttach(string ownerId, string tagId, int position, LOwner owner)
    {
        LTagArchive tags = new(_lEngineDatabase);
        if (LEngineOwnerCheck(owner))
        {
            tags.LTagCollocationAttach(ownerId, tagId, position);
            return;
        }

        tags.LTagSenseAttach(ownerId, tagId, position);
    }

    /// <summary>
    /// Removes one side's reference to a Tag. The Tag itself and every other reference to it survive,
    /// which is the rule a card edit follows — see <see cref="LEngineTagRemove"/> for the other
    /// intention.
    /// </summary>
    public void LEngineTagDetach(string ownerId, string tagId, LOwner owner)
    {
        LTagArchive tags = new(_lEngineDatabase);
        if (LEngineOwnerCheck(owner))
        {
            tags.LTagCollocationDetach(ownerId, tagId);
            return;
        }

        tags.LTagSenseDetach(ownerId, tagId);
    }

    /// <summary>
    /// Removes one side's reference to a Tag and deletes the Tag when that was its last reference —
    /// the Tag a card was the only holder of goes with the reference, and a Tag other cards still
    /// reference stays exactly as it was.
    /// <para>
    /// The detach, the count and the delete share one session, so the decision is made against the
    /// references as they stand at that moment: nothing can attach the Tag between the count and the
    /// delete, and a failure part-way leaves the reference in place rather than a Tag nothing points at.
    /// This is why the shell cannot be given the three steps to compose — between any two of them the
    /// answer to "does anything still reference this" can change.
    /// </para>
    /// </summary>
    public void LEngineTagRemove(string ownerId, string tagId, LOwner owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEngineTagDetach(ownerId, tagId, owner);

        LTagArchive tags = new(_lEngineDatabase);
        if (tags.LTagReferenceRead(tagId) == 0)
        {
            tags.LTagDelete(tagId);
        }

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Deletes the Tag identified by <paramref name="id"/>. Refused while any Meaning or Collocation
    /// still references it: the cards that mention a Tag are not this seam's to rewrite, so the way to
    /// delete a referenced Tag is to part each card from it — <see cref="LEngineTagRemove"/> deletes it
    /// as the last reference goes.
    /// </summary>
    public void LEngineTagDelete(string id)
    {
        new LTagArchive(_lEngineDatabase).LTagDelete(id);
    }
}
