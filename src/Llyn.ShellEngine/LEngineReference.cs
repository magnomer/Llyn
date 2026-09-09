using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LReference LEngineReferenceCreate(LReference reference)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(reference);
            return new LReferenceArchive(_lEngineDatabase).LReferenceCreate(reference);
        }
    }

    public LReference? LEngineReferenceRead(string id)
    {
        lock (_lEngineGate)
        {
            return new LReferenceArchive(_lEngineDatabase).LReferenceRead(id);
        }
    }

    public IReadOnlyList<LReference> LEngineReferenceRead()
    {
        lock (_lEngineGate)
        {
            return new LReferenceArchive(_lEngineDatabase).LReferenceAllRead();
        }
    }

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);
            query = query.Trim();

            IReadOnlyList<LReference> read = new LReferenceArchive(_lEngineDatabase).LReferenceAllRead();
            IReadOnlyDictionary<string, IReadOnlyList<LAuthor>> credits =
                new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead();
            IReadOnlyDictionary<string, int> usage =
                new LReferenceUsage(_lEngineDatabase).LReferenceUsageRead();

            List<LCatalogReference> rows = [];
            foreach (LReference reference in read)
            {
                credits.TryGetValue(reference.LReferenceId, out IReadOnlyList<LAuthor>? credited);
                usage.TryGetValue(reference.LReferenceId, out int counted);

                LCatalogReference row = LCatalogReference.LCatalogReferenceCreate(reference, credited, counted);
                if (row.LCatalogReferenceMatch(query))
                {
                    rows.Add(row);
                }
            }

            return LCatalogReference.LCatalogReferenceSort(rows, order);
        }
    }

    public IReadOnlyList<LReference> LEngineReferenceRead(string ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LReferenceArchive references = new(_lEngineDatabase);
            switch (owner)
            {
                case LOwner.LOwnerExample:
                    LReference? cited = references.LReferenceExampleRead(ownerId);
                    return cited is null ? [] : [cited];
                default:
                    throw LEngineOwnerRaise(owner);
            }
        }
    }

    public void LEngineReferenceUpdate(LReference reference)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceUpdate(reference);
        }
    }

    public void LEngineReferenceAttach(string ownerId, string referenceId, int position, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LReferenceArchive references = new(_lEngineDatabase);
            switch (owner)
            {
                case LOwner.LOwnerExample:
                    references.LReferenceExampleAttach(ownerId, referenceId);
                    return;
                default:
                    throw LEngineOwnerRaise(owner);
            }
        }
    }

    public void LEngineReferenceDetach(string ownerId, string referenceId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LReferenceArchive references = new(_lEngineDatabase);
            switch (owner)
            {
                case LOwner.LOwnerExample:
                    references.LReferenceExampleDetach(ownerId);
                    return;
                default:
                    throw LEngineOwnerRaise(owner);
            }
        }
    }

    public void LEngineReferenceDelete(string id)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    public void LEngineReferenceDelete(string id, bool detach)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    public LAuthor LEngineAuthorCreate(LAuthor author)
    {
        LAuthor created;
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(author);
            ArgumentException.ThrowIfNullOrWhiteSpace(author.LAuthorName);
            created = new LAuthorArchive(_lEngineDatabase).LAuthorCreate(author);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, created.LAuthorId);
        return created;
    }

    public LAuthor? LEngineAuthorRead(string id)
    {
        lock (_lEngineGate)
        {
            return new LAuthorArchive(_lEngineDatabase).LAuthorRead(id);
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead()
    {
        lock (_lEngineGate)
        {
            return new LAuthorArchive(_lEngineDatabase).LAuthorAllRead();
        }
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead(string ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            if (owner != LOwner.LOwnerReference)
            {
                throw LEngineOwnerRaise(owner);
            }

            return new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead(ownerId);
        }
    }

    public IReadOnlyDictionary<string, IReadOnlyList<LAuthor>> LEngineAuthorRead(LOwner owner)
    {
        lock (_lEngineGate)
        {
            if (owner != LOwner.LOwnerReference)
            {
                throw LEngineOwnerRaise(owner);
            }

            return new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead();
        }
    }

    public void LEngineAuthorUpdate(LAuthor author)
    {
        lock (_lEngineGate)
        {
            new LAuthorArchive(_lEngineDatabase).LAuthorUpdate(author);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, author.LAuthorId);
    }

    public void LEngineAuthorAttach(string referenceId, string authorId, int position)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceAuthorAttach(referenceId, authorId, position);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    public void LEngineAuthorDetach(string referenceId, string authorId)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceAuthorDetach(referenceId, authorId);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    public void LEngineAuthorDelete(string id)
    {
        lock (_lEngineGate)
        {
            new LAuthorArchive(_lEngineDatabase).LAuthorDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }

    public void LEngineAuthorDelete(string id, bool detach)
    {
        lock (_lEngineGate)
        {
            new LAuthorArchive(_lEngineDatabase).LAuthorDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }
}
