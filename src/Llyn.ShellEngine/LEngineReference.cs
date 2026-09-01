using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

/// <summary>
/// The bibliographic half of the engine: a Reference — the work an Entry or an Example is drawn from —
/// and the Authors credited on it. Both are independent shared data: several Entries may cite one
/// Reference, and several References may credit one Author, so a citation and a credit are references
/// to a row, never ownership of it.
/// <para>
/// The two sides of a citation differ in shape, which is why <see cref="LOwner"/> says which is meant.
/// An Entry cites any number of References and holds their order; an Example cites at most one, a
/// single column with nothing to order, so attaching there replaces whatever it cited before.
/// </para>
/// <para>
/// Nothing here deletes a row on the strength of a reference going: a Reference no Entry cites and an
/// Author no Reference credits are both still deliberate data — unlike a card's Examples and Tags,
/// they are not created as a side effect of typing, so they go only when something says to delete them.
/// </para>
/// </summary>
public sealed partial class LEngine
{
    /// <summary>
    /// Creates <paramref name="reference"/> and returns it with its assigned id. Its fields carry the
    /// three-state distinction the model gives them: unspecified, deliberately unknown, or a value.
    /// </summary>
    public LReference LEngineReferenceCreate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        return new LReferenceArchive(_lEngineDatabase).LReferenceCreate(reference);
    }

    /// <summary>Reads the Reference for <paramref name="id"/>, or <c>null</c> when none has that id.</summary>
    public LReference? LEngineReferenceRead(string id)
    {
        return new LReferenceArchive(_lEngineDatabase).LReferenceRead(id);
    }

    /// <summary>
    /// Reads the References the Entry or Example identified by <paramref name="ownerId"/> cites, in
    /// citation order. An Example cites at most one, so its list holds either that one or nothing.
    /// </summary>
    public IReadOnlyList<LReference> LEngineReferenceRead(string ownerId, LOwner owner)
    {
        LReferenceArchive references = new(_lEngineDatabase);
        switch (owner)
        {
            case LOwner.LOwnerEntry:
                return references.LReferenceEntryRead(ownerId);
            case LOwner.LOwnerExample:
                LReference? cited = references.LReferenceExampleRead(ownerId);
                return cited is null ? [] : [cited];
            default:
                throw LEngineOwnerRaise(owner);
        }
    }

    /// <summary>Rewrites the fields of the Reference <paramref name="reference"/> identifies.</summary>
    public void LEngineReferenceUpdate(LReference reference)
    {
        new LReferenceArchive(_lEngineDatabase).LReferenceUpdate(reference);
    }

    /// <summary>
    /// Cites the Reference identified by <paramref name="referenceId"/> from the Entry or Example
    /// identified by <paramref name="ownerId"/>. An Entry holds the citation at
    /// <paramref name="position"/> among its own; an Example holds one citation, so the position is not
    /// used and whatever it cited before is replaced.
    /// </summary>
    public void LEngineReferenceAttach(string ownerId, string referenceId, int position, LOwner owner)
    {
        LReferenceArchive references = new(_lEngineDatabase);
        switch (owner)
        {
            case LOwner.LOwnerEntry:
                references.LReferenceEntryAttach(ownerId, referenceId, position);
                return;
            case LOwner.LOwnerExample:
                references.LReferenceExampleAttach(ownerId, referenceId);
                return;
            default:
                throw LEngineOwnerRaise(owner);
        }
    }

    /// <summary>
    /// Removes an Entry's citation of the Reference identified by <paramref name="referenceId"/>, or
    /// clears the one an Example cites. The Reference and its other citations survive.
    /// </summary>
    public void LEngineReferenceDetach(string ownerId, string referenceId, LOwner owner)
    {
        LReferenceArchive references = new(_lEngineDatabase);
        switch (owner)
        {
            case LOwner.LOwnerEntry:
                references.LReferenceEntryDetach(ownerId, referenceId);
                return;
            case LOwner.LOwnerExample:
                references.LReferenceExampleDetach(ownerId);
                return;
            default:
                throw LEngineOwnerRaise(owner);
        }
    }

    /// <summary>
    /// Deletes the Reference identified by <paramref name="id"/> together with the author credits it
    /// owns. Refused while any Entry or Example still cites it — remove those citations first. The
    /// Authors it credited survive; only the credits between them and this Reference go.
    /// </summary>
    public void LEngineReferenceDelete(string id)
    {
        new LReferenceArchive(_lEngineDatabase).LReferenceDelete(id);
    }

    /// <summary>Creates <paramref name="author"/> and returns it with its assigned id.</summary>
    public LAuthor LEngineAuthorCreate(LAuthor author)
    {
        ArgumentNullException.ThrowIfNull(author);
        return new LAuthorArchive(_lEngineDatabase).LAuthorCreate(author);
    }

    /// <summary>Reads the Author for <paramref name="id"/>, or <c>null</c> when none has that id.</summary>
    public LAuthor? LEngineAuthorRead(string id)
    {
        return new LAuthorArchive(_lEngineDatabase).LAuthorRead(id);
    }

    /// <summary>
    /// Reads the Authors credited on the Reference identified by <paramref name="ownerId"/>, in that
    /// Reference's own author order.
    /// </summary>
    public IReadOnlyList<LAuthor> LEngineAuthorRead(string ownerId, LOwner owner)
    {
        if (owner != LOwner.LOwnerReference)
        {
            throw LEngineOwnerRaise(owner);
        }

        return new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead(ownerId);
    }

    /// <summary>Renames the Author <paramref name="author"/> identifies. Every credit reads the new name.</summary>
    public void LEngineAuthorUpdate(LAuthor author)
    {
        new LAuthorArchive(_lEngineDatabase).LAuthorUpdate(author);
    }

    /// <summary>
    /// Credits the Author identified by <paramref name="authorId"/> on the Reference identified by
    /// <paramref name="referenceId"/> at <paramref name="position"/> in that Reference's author order.
    /// The Author row stays available to every other Reference.
    /// </summary>
    public void LEngineAuthorAttach(string referenceId, string authorId, int position)
    {
        new LReferenceArchive(_lEngineDatabase).LReferenceAuthorAttach(referenceId, authorId, position);
    }

    /// <summary>
    /// Removes one Reference's credit for an Author. The Author and its other credits survive: an
    /// Author is deliberate data, so losing a credit is not a reason to delete the person.
    /// </summary>
    public void LEngineAuthorDetach(string referenceId, string authorId)
    {
        new LReferenceArchive(_lEngineDatabase).LReferenceAuthorDetach(referenceId, authorId);
    }

    /// <summary>
    /// Deletes the Author identified by <paramref name="id"/>. Refused while any Reference still
    /// credits the Author — detach every credit first. Deleting an Author never deletes a Reference.
    /// </summary>
    public void LEngineAuthorDelete(string id)
    {
        new LAuthorArchive(_lEngineDatabase).LAuthorDelete(id);
    }
}
