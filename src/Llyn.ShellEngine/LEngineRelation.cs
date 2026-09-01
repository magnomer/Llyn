using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The relation half of the engine: the seam onto a Meaning's lexical relations and a Collocation's
/// synonym interlinks, and onto the lookup that turns typed text into the targets one of them may point
/// at. It sits in its own file because it answers one question the rest of the engine does not — which
/// stored row a link points at, and how a caller finds that row before it writes.
/// <para>
/// Both kinds of link target an Entry <b>or</b> a Meaning, exclusively. That is a rule of the model, not
/// of the store: <c>relation_entry</c> XOR <c>relation_sense</c> and the <c>CHECK</c> on
/// <c>collocation_synonym</c> enforce it underneath, and every write here refuses before it opens a
/// session when a request names both targets, neither, or one that is not in this workspace.
/// </para>
/// <para>
/// <b>The engine never turns text into a target.</b> A card field holds free text and a link needs an id,
/// so the two are bridged by a lookup the caller runs first — <see cref="LEngineEntryFind"/> for entries,
/// <see cref="LEngineSenseFind"/> for meanings — and the caller passes back the id it chose. Text that
/// matched nothing has no id to pass, and the write is refused with <see cref="LRefusal.LRefusalTarget"/>
/// rather than fabricating a target or dropping what was typed. A guess written here would be a link the
/// user never made, pointing at a word they never chose.
/// </para>
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Returns the Meanings whose owning entry's headword contains <paramref name="query"/>, ordered by
    /// headword and then by each Meaning's place in its entry, or every Meaning when
    /// <paramref name="query"/> is empty or all whitespace.
    /// <para>
    /// This is the meaning-level twin of <see cref="LEngineEntryFind"/> and the second half of the
    /// resolution step: a caller holding typed text gets back the Meanings that text could name and picks
    /// one, so what it hands to <see cref="LEngineRelationCreate"/> or <see cref="LEngineSynonymCreate"/>
    /// is a target the user chose. A query matching nothing returns an empty list — which is the answer
    /// "this text names no Meaning", not a reason to invent one.
    /// </para>
    /// </summary>
    public IReadOnlyList<LSense> LEngineSenseFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);
        return new LSenseArchive(_lEngineDatabase).LSenseFind(query);
    }

    /// <summary>
    /// Creates <paramref name="relation"/> at the end of its origin Meaning's relations and returns it
    /// with its assigned id and position. The target must be one resolved row of this workspace: naming
    /// both an Entry and a Meaning, naming neither, or naming one that is not stored is refused with
    /// <see cref="LRefusal.LRefusalTarget"/> before anything is written.
    /// </summary>
    public LRelation LEngineRelationCreate(LRelation relation)
    {
        ArgumentNullException.ThrowIfNull(relation);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
        LEngineTargetValidate(relation.LRelationTargetEntry, relation.LRelationTargetSense);

        LRelation stored = new LRelationArchive(_lEngineDatabase).LRelationCreate(relation);

        session.LDatabaseSessionCommit();
        return stored;
    }

    /// <summary>
    /// Reads the relations originating from the Meaning identified by <paramref name="senseId"/>, in
    /// stored order, each carrying the single target it points at.
    /// </summary>
    public IReadOnlyList<LRelation> LEngineRelationRead(string senseId)
    {
        return new LRelationArchive(_lEngineDatabase).LRelationRead(senseId);
    }

    /// <summary>
    /// Updates the type and labels of the relation <paramref name="relation"/> identifies. Its target and
    /// its place among its siblings are not touched here — a relation is re-pointed by deleting it and
    /// creating the one that replaces it, so a target is only ever written alongside the check that
    /// resolved it, and <see cref="LEngineRelationMove"/> owns the order.
    /// </summary>
    public void LEngineRelationUpdate(LRelation relation)
    {
        new LRelationArchive(_lEngineDatabase).LRelationUpdate(relation);
    }

    /// <summary>
    /// Moves the relation identified by <paramref name="id"/> to <paramref name="position"/> among the
    /// relations of its origin Meaning, renumbering the set so its positions stay contiguous.
    /// </summary>
    public void LEngineRelationMove(string id, int position)
    {
        new LRelationArchive(_lEngineDatabase).LRelationMove(id, position);
    }

    /// <summary>
    /// Deletes the relation identified by <paramref name="id"/>. The Entry or Meaning it pointed at is
    /// untouched — a link going does not take what it linked to with it.
    /// </summary>
    public void LEngineRelationDelete(string id)
    {
        new LRelationArchive(_lEngineDatabase).LRelationDelete(id);
    }

    /// <summary>
    /// Creates <paramref name="synonym"/> at the end of its origin Collocation's synonyms and returns it
    /// with its assigned id and position. The target is checked on the same terms as a relation's:
    /// exactly one Entry or Meaning, and one that is stored, or the write is refused with
    /// <see cref="LRefusal.LRefusalTarget"/>.
    /// </summary>
    public LSynonym LEngineSynonymCreate(LSynonym synonym)
    {
        ArgumentNullException.ThrowIfNull(synonym);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
        LEngineTargetValidate(synonym.LSynonymTargetEntry, synonym.LSynonymTargetSense);

        LSynonym stored = new LSynonymArchive(_lEngineDatabase).LSynonymCreate(synonym);

        session.LDatabaseSessionCommit();
        return stored;
    }

    /// <summary>
    /// Reads the synonym interlinks hanging from the Collocation identified by
    /// <paramref name="collocationId"/>, in stored order, each carrying the single target it points at.
    /// </summary>
    public IReadOnlyList<LSynonym> LEngineSynonymRead(string collocationId)
    {
        return new LSynonymArchive(_lEngineDatabase).LSynonymRead(collocationId);
    }

    /// <summary>
    /// Re-points the synonym <paramref name="synonym"/> identifies at the target it now names. A synonym
    /// holds nothing but its target, so the new target is resolved here on the same terms as a create:
    /// both, neither, or an unstored target is refused with <see cref="LRefusal.LRefusalTarget"/>.
    /// </summary>
    public void LEngineSynonymUpdate(LSynonym synonym)
    {
        ArgumentNullException.ThrowIfNull(synonym);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
        LEngineTargetValidate(synonym.LSynonymTargetEntry, synonym.LSynonymTargetSense);

        new LSynonymArchive(_lEngineDatabase).LSynonymUpdate(synonym);

        session.LDatabaseSessionCommit();
    }

    /// <summary>
    /// Moves the synonym identified by <paramref name="id"/> to <paramref name="position"/> in its
    /// Collocation's order, renumbering the set so its positions stay contiguous.
    /// </summary>
    public void LEngineSynonymMove(string id, int position)
    {
        new LSynonymArchive(_lEngineDatabase).LSynonymMove(id, position);
    }

    /// <summary>
    /// Deletes the synonym interlink identified by <paramref name="id"/>. The Entry or Meaning it pointed
    /// at is untouched.
    /// </summary>
    public void LEngineSynonymDelete(string id)
    {
        new LSynonymArchive(_lEngineDatabase).LSynonymDelete(id);
    }

    // The one place a link's target is checked, for both kinds of link: exactly one of the two ids is
    // set, and the row it names is in this workspace. Both conditions are the same refusal, because they
    // are the same failure seen from two sides — the caller did not hand over one resolved target. It is
    // a refusal rather than an exception because it is a request the user can correct: the text they
    // typed names nothing, and the shell says so instead of the save quietly writing nothing.
    private void LEngineTargetValidate(string? entryId, string? senseId)
    {
        bool hasEntry = !string.IsNullOrWhiteSpace(entryId);
        bool hasSense = !string.IsNullOrWhiteSpace(senseId);
        if (hasEntry == hasSense)
        {
            throw new LRefusal(LRefusal.LRefusalTarget);
        }

        bool found = hasEntry
            ? new LEntryArchive(_lEngineDatabase).LEntryRead(entryId!) is not null
            : new LSenseArchive(_lEngineDatabase).LSenseSingleRead(senseId!) is not null;
        if (!found)
        {
            throw new LRefusal(LRefusal.LRefusalTarget);
        }
    }
}
