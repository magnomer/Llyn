using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LReference LEngineReferenceCreate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        return new LReferenceArchive(_lEngineDatabase).LReferenceCreate(reference);
    }

    public LReference? LEngineReferenceRead(string id)
    {
        return new LReferenceArchive(_lEngineDatabase).LReferenceRead(id);
    }

    public IReadOnlyList<LReference> LEngineReferenceRead()
    {
        return new LReferenceArchive(_lEngineDatabase).LReferenceAllRead();
    }

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

    public void LEngineReferenceUpdate(LReference reference)
    {
        new LReferenceArchive(_lEngineDatabase).LReferenceUpdate(reference);
    }

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

    public void LEngineReferenceDelete(string id)
    {
        new LReferenceArchive(_lEngineDatabase).LReferenceDelete(id);
    }

    public LAuthor LEngineAuthorCreate(LAuthor author)
    {
        ArgumentNullException.ThrowIfNull(author);
        return new LAuthorArchive(_lEngineDatabase).LAuthorCreate(author);
    }

    public LAuthor? LEngineAuthorRead(string id)
    {
        return new LAuthorArchive(_lEngineDatabase).LAuthorRead(id);
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead(string ownerId, LOwner owner)
    {
        if (owner != LOwner.LOwnerReference)
        {
            throw LEngineOwnerRaise(owner);
        }

        return new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead(ownerId);
    }

    public void LEngineAuthorUpdate(LAuthor author)
    {
        new LAuthorArchive(_lEngineDatabase).LAuthorUpdate(author);
    }

    public void LEngineAuthorAttach(string referenceId, string authorId, int position)
    {
        new LReferenceArchive(_lEngineDatabase).LReferenceAuthorAttach(referenceId, authorId, position);
    }

    public void LEngineAuthorDetach(string referenceId, string authorId)
    {
        new LReferenceArchive(_lEngineDatabase).LReferenceAuthorDetach(referenceId, authorId);
    }

    public void LEngineAuthorDelete(string id)
    {
        new LAuthorArchive(_lEngineDatabase).LAuthorDelete(id);
    }
}
