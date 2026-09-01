using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The Meaning and Collocation halves of the engine: the two card kinds an Entry is made of, reached
/// one row at a time rather than through the whole-form save. The form writes and rewrites a whole
/// draft, which is the only path the input panel needs; these seams are the model's own — create one
/// Meaning, move it among its siblings, delete it — and they exist whether or not a control does.
/// <para>
/// Both are ordered within their Entry, so both carry a move, and both delete downwards: a Meaning
/// takes its subordinate Meanings and its association rows with it, a Collocation takes its synonym
/// interlinks and its association rows, and the independent Examples, Tags and Situations either
/// referenced are left standing. Only the links go.
/// </para>
/// <para>
/// A Meaning is refused deletion while a relation from outside the subtree, or any Collocation
/// synonym, still points at it: a link is a statement about a Meaning that exists, and a cascade must
/// not decide on its own to unmake someone else's statement.
/// </para>
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Creates <paramref name="sense"/> at the end of its Entry's Meanings — or of its parent's
    /// subordinate Meanings — and returns it with its assigned id and position.
    /// </summary>
    public LSense LEngineSenseCreate(LSense sense)
    {
        ArgumentNullException.ThrowIfNull(sense);
        return new LSenseArchive(_lEngineDatabase).LSenseCreate(sense);
    }

    /// <summary>Reads the Meaning for <paramref name="id"/>, or <c>null</c> when none has that id.</summary>
    public LSense? LEngineSenseRead(string id)
    {
        return new LSenseArchive(_lEngineDatabase).LSenseSingleRead(id);
    }

    /// <summary>
    /// Reads the Meanings of the Entry identified by <paramref name="ownerId"/>, in stored order.
    /// </summary>
    public IReadOnlyList<LSense> LEngineSenseRead(string ownerId, LOwner owner)
    {
        if (owner != LOwner.LOwnerEntry)
        {
            throw LEngineOwnerRaise(owner);
        }

        return new LSenseArchive(_lEngineDatabase).LSenseRead(ownerId);
    }

    /// <summary>
    /// Rewrites the fields of the Meaning <paramref name="sense"/> identifies. Its place among its
    /// siblings is not touched here — <see cref="LEngineSenseMove"/> owns the order.
    /// </summary>
    public void LEngineSenseUpdate(LSense sense)
    {
        new LSenseArchive(_lEngineDatabase).LSenseUpdate(sense);
    }

    /// <summary>
    /// Moves the Meaning identified by <paramref name="id"/> to <paramref name="position"/> among its
    /// siblings, renumbering the group so the positions stay contiguous. A position outside the group
    /// is clamped into it.
    /// </summary>
    public void LEngineSenseMove(string id, int position)
    {
        new LSenseArchive(_lEngineDatabase).LSenseMove(id, position);
    }

    /// <summary>
    /// Deletes the Meaning identified by <paramref name="id"/> with everything it owns: its
    /// subordinate Meanings, the relations originating inside that subtree, and its Example, Tag and
    /// Situation association rows. Refused while a relation from outside the subtree or a Collocation
    /// synonym still points at one of these Meanings.
    /// </summary>
    public void LEngineSenseDelete(string id)
    {
        new LSenseArchive(_lEngineDatabase).LSenseDelete(id);
    }

    /// <summary>
    /// Creates <paramref name="collocation"/> at the end of its Entry's Collocations and returns it
    /// with its assigned id and position.
    /// </summary>
    public LCollocation LEngineCollocationCreate(LCollocation collocation)
    {
        ArgumentNullException.ThrowIfNull(collocation);
        return new LCollocationArchive(_lEngineDatabase).LCollocationCreate(collocation);
    }

    /// <summary>
    /// Reads the Collocations of the Entry identified by <paramref name="ownerId"/>, in stored order.
    /// </summary>
    public IReadOnlyList<LCollocation> LEngineCollocationRead(string ownerId, LOwner owner)
    {
        if (owner != LOwner.LOwnerEntry)
        {
            throw LEngineOwnerRaise(owner);
        }

        return new LCollocationArchive(_lEngineDatabase).LCollocationRead(ownerId);
    }

    /// <summary>Rewrites the title, expression and meaning of the Collocation <paramref name="collocation"/> identifies.</summary>
    public void LEngineCollocationUpdate(LCollocation collocation)
    {
        new LCollocationArchive(_lEngineDatabase).LCollocationUpdate(collocation);
    }

    /// <summary>
    /// Moves the Collocation identified by <paramref name="id"/> to <paramref name="position"/> in its
    /// Entry's card order, renumbering the set so the positions stay contiguous.
    /// </summary>
    public void LEngineCollocationMove(string id, int position)
    {
        new LCollocationArchive(_lEngineDatabase).LCollocationMove(id, position);
    }

    /// <summary>
    /// Deletes the Collocation identified by <paramref name="id"/>. Its synonym interlinks and its
    /// association rows go with it; the independent Examples, Tags and Situations they pointed at are
    /// left standing, and the Collocations left under the Entry are renumbered.
    /// </summary>
    public void LEngineCollocationDelete(string id)
    {
        new LCollocationArchive(_lEngineDatabase).LCollocationDelete(id);
    }
}
