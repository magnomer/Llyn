using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The Situation half of the engine: the context a Meaning or Collocation is used in, created, read,
/// rewritten, referenced and let go of. A Situation is independent data on the same terms as a Tag —
/// owned by nothing, referenced by any number of cards, each holding the order it takes — so the seams
/// mirror the Tag ones exactly, including the two ways of parting a card from one.
/// <para>
/// Rewriting a Situation is a single seam rather than a read the caller edits and writes back: title,
/// description and kind are one row, and every card referencing it sees the new wording at once
/// because a reference points at the row and never at a copy of its text.
/// </para>
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Creates <paramref name="situation"/> and returns it with its assigned id. Its title is not
    /// identity: the same words saved twice are two Situations.
    /// </summary>
    public LSituation LEngineSituationCreate(LSituation situation)
    {
        ArgumentNullException.ThrowIfNull(situation);
        return new LSituationArchive(_lEngineDatabase).LSituationCreate(situation);
    }

    /// <summary>Reads the Situation for <paramref name="id"/>, or <c>null</c> when none has that id.</summary>
    public LSituation? LEngineSituationRead(string id)
    {
        return new LSituationArchive(_lEngineDatabase).LSituationRead(id);
    }

    /// <summary>
    /// Reads the Situations the Meaning or Collocation identified by <paramref name="ownerId"/>
    /// references, in the order that side holds them.
    /// </summary>
    public IReadOnlyList<LSituation> LEngineSituationRead(string ownerId, LOwner owner)
    {
        LSituationArchive situations = new(_lEngineDatabase);
        return LEngineOwnerCheck(owner)
            ? situations.LSituationCollocationRead(ownerId)
            : situations.LSituationSenseRead(ownerId);
    }

    /// <summary>Rewrites the title, description and kind of the Situation <paramref name="situation"/> identifies.</summary>
    public void LEngineSituationUpdate(LSituation situation)
    {
        new LSituationArchive(_lEngineDatabase).LSituationUpdate(situation);
    }

    /// <summary>
    /// References the Situation identified by <paramref name="situationId"/> from the Meaning or
    /// Collocation identified by <paramref name="ownerId"/> at <paramref name="position"/> in that
    /// side's order, renumbering the set around it.
    /// </summary>
    public void LEngineSituationAttach(string ownerId, string situationId, int position, LOwner owner)
    {
        LSituationArchive situations = new(_lEngineDatabase);
        if (LEngineOwnerCheck(owner))
        {
            situations.LSituationCollocationAttach(ownerId, situationId, position);
            return;
        }

        situations.LSituationSenseAttach(ownerId, situationId, position);
    }

    /// <summary>
    /// Removes one side's reference to a Situation. The Situation and its other references survive —
    /// the rule a card edit follows.
    /// </summary>
    public void LEngineSituationDetach(string ownerId, string situationId, LOwner owner)
    {
        LSituationArchive situations = new(_lEngineDatabase);
        if (LEngineOwnerCheck(owner))
        {
            situations.LSituationCollocationDetach(ownerId, situationId);
            return;
        }

        situations.LSituationSenseDetach(ownerId, situationId);
    }

    /// <summary>
    /// Removes one side's reference to a Situation and deletes the Situation when that was its last
    /// reference. Detach, count and delete share one session, so the row is judged against the
    /// references as they stand at that moment and no caller can compose the steps itself.
    /// </summary>
    public void LEngineSituationRemove(string ownerId, string situationId, LOwner owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(situationId);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEngineSituationDetach(ownerId, situationId, owner);

        LSituationArchive situations = new(_lEngineDatabase);
        if (situations.LSituationReferenceRead(situationId) == 0)
        {
            situations.LSituationDelete(situationId);
        }

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Deletes the Situation identified by <paramref name="id"/>. Refused while any Meaning or
    /// Collocation still references it; <see cref="LEngineSituationRemove"/> is the seam that deletes
    /// one as its last reference goes.
    /// </summary>
    public void LEngineSituationDelete(string id)
    {
        new LSituationArchive(_lEngineDatabase).LSituationDelete(id);
    }
}
