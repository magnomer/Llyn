using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The Example half of the engine: an Example with its translations created, read, rewritten and let
/// go of, and the references an Entry, a Meaning or a Collocation holds to it. An Example is
/// independent data owned by nothing, so three sides may reference the same one and each holds its own
/// order over the Examples it references.
/// <para>
/// Three sides is why the side arrives as an <see cref="LOwner"/> and not in the method's name; unlike
/// a Tag, an Example may hang from an Entry as well, and a seam handed a side no association table
/// serves refuses rather than picking one.
/// </para>
/// <para>
/// The Source an Example cites is set through its own seam. Only the citation moves there: the
/// Reference row is never created, changed or deleted by pointing an Example at it or away from it.
/// </para>
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Creates <paramref name="example"/> with its translations as its ordered child rows and returns
    /// it with its assigned id.
    /// </summary>
    public LExample LEngineExampleCreate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        return new LExampleArchive(_lEngineDatabase).LExampleCreate(example);
    }

    /// <summary>
    /// Reads the Example for <paramref name="id"/> with its translations in stored order, or
    /// <c>null</c> when no Example has that id.
    /// </summary>
    public LExample? LEngineExampleRead(string id)
    {
        return new LExampleArchive(_lEngineDatabase).LExampleRead(id);
    }

    /// <summary>
    /// Reads the Examples the Entry, Meaning or Collocation identified by <paramref name="ownerId"/>
    /// references, in the order that side holds them, each with its translations.
    /// </summary>
    public IReadOnlyList<LExample> LEngineExampleRead(string ownerId, LOwner owner)
    {
        LExampleLink examples = new(_lEngineDatabase);
        return owner switch
        {
            LOwner.LOwnerEntry => examples.LExampleEntryRead(ownerId),
            LOwner.LOwnerSense => examples.LExampleSenseRead(ownerId),
            LOwner.LOwnerCollocation => examples.LExampleCollocationRead(ownerId),
            _ => throw LEngineOwnerRaise(owner),
        };
    }

    /// <summary>
    /// Rewrites the text, its local rendering and the translations of the Example
    /// <paramref name="example"/> identifies. The Source it cites is not touched here — that is
    /// <see cref="LEngineExampleUpdate(string, string?)"/>.
    /// </summary>
    public void LEngineExampleUpdate(LExample example)
    {
        new LExampleArchive(_lEngineDatabase).LExampleUpdate(example);
    }

    /// <summary>
    /// Sets or clears the single Reference the Example identified by <paramref name="exampleId"/>
    /// cites — pass <c>null</c> to clear it. Only the citation moves: the Reference row itself is
    /// neither created, changed, nor deleted here.
    /// </summary>
    public void LEngineExampleUpdate(string exampleId, string? referenceId)
    {
        new LExampleArchive(_lEngineDatabase).LExampleSourceUpdate(exampleId, referenceId);
    }

    /// <summary>
    /// References the Example identified by <paramref name="exampleId"/> from the Entry, Meaning or
    /// Collocation identified by <paramref name="ownerId"/> at <paramref name="position"/> in that
    /// side's order, renumbering the set around it so the positions stay contiguous.
    /// </summary>
    public void LEngineExampleAttach(string ownerId, string exampleId, int position, LOwner owner)
    {
        LExampleLink examples = new(_lEngineDatabase);
        switch (owner)
        {
            case LOwner.LOwnerEntry:
                examples.LExampleEntryAttach(ownerId, exampleId, position);
                return;
            case LOwner.LOwnerSense:
                examples.LExampleSenseAttach(ownerId, exampleId, position);
                return;
            case LOwner.LOwnerCollocation:
                examples.LExampleCollocationAttach(ownerId, exampleId, position);
                return;
            default:
                throw LEngineOwnerRaise(owner);
        }
    }

    /// <summary>
    /// Removes one side's reference to an Example. The Example and its other references survive — the
    /// rule a card edit follows, where dropping an Example from a card leaves what other cards quote.
    /// </summary>
    public void LEngineExampleDetach(string ownerId, string exampleId, LOwner owner)
    {
        LExampleLink examples = new(_lEngineDatabase);
        switch (owner)
        {
            case LOwner.LOwnerEntry:
                examples.LExampleEntryDetach(ownerId, exampleId);
                return;
            case LOwner.LOwnerSense:
                examples.LExampleSenseDetach(ownerId, exampleId);
                return;
            case LOwner.LOwnerCollocation:
                examples.LExampleCollocationDetach(ownerId, exampleId);
                return;
            default:
                throw LEngineOwnerRaise(owner);
        }
    }

    /// <summary>
    /// Removes one side's reference to an Example and deletes the Example, with its translations, when
    /// that was its last reference. An Example quoted by nothing is unreachable data, so the reference
    /// going takes it; an Example another card still quotes stays as it is.
    /// <para>
    /// The detach, the count and the delete share one session, so the row is judged against the
    /// references as they stand at that moment. That is why this is one seam and not three the shell
    /// composes: between any two steps the answer to "does anything still quote this" can change.
    /// </para>
    /// </summary>
    public void LEngineExampleRemove(string ownerId, string exampleId, LOwner owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEngineExampleDetach(ownerId, exampleId, owner);

        LExampleArchive examples = new(_lEngineDatabase);
        if (examples.LExampleReferenceRead(exampleId) == 0)
        {
            examples.LExampleDelete(exampleId);
        }

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Deletes the Example identified by <paramref name="id"/> together with its translations. Refused
    /// while any Entry, Meaning or Collocation still references it; <see cref="LEngineExampleRemove"/>
    /// is the seam that deletes one as its last reference goes. A Reference it cited is left standing.
    /// </summary>
    public void LEngineExampleDelete(string id)
    {
        new LExampleArchive(_lEngineDatabase).LExampleDelete(id);
    }
}
