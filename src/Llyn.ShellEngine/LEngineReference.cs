using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LReference LEngineReferenceCreate(LReference reference)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(reference);
            return new LReferenceArchive(_lEngineDatabase).LReferenceCreate(reference);
        }
    }

    public LReference LEngineCitationCreate(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        LReference stored;
        lock (_lEngineGate)
        {
            LStateValue named = LEngineValueRead(new LStateWritten(title.Trim(), false));
            stored = new LReferenceArchive(_lEngineDatabase).LReferenceCreate(new LReference(
                0,
                named,
                LStateValue.LStateValueUnspecified,
                LReferenceKind.LReferenceKindUnspecified,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified,
                LStateMark.LStateMarkUnspecified));
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, stored.LReferenceId);
        return stored;
    }

    internal LReference? LEngineReferenceRead(long id)
    {
        lock (_lEngineGate)
        {
            return new LReferenceArchive(_lEngineDatabase).LReferenceRead(id);
        }
    }

    internal IReadOnlyList<LReference> LEngineReferenceRead()
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
            IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits =
                new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead();
            IReadOnlyDictionary<long, int> usage =
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

    public IReadOnlyList<LCatalogReference> LEngineReferenceFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogReference> found = LEngineReferenceFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogReference> rows = new(found.Count);
        string[] names = LEngineTwinRead(found, row => row.LCatalogReferenceName, row => row.LCatalogReferenceStored.LReferenceId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogReference row = found[index];
            rows.Add(row with
            {
                LCatalogReferenceName = names[index],
                LCatalogReferenceChosen = row.LCatalogReferenceStored.LReferenceId == vista.LVistaChosen,
            });
        }

        return rows;
    }

    internal IReadOnlyList<LReference> LEngineReferenceRead(long ownerId, LOwner owner)
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

    internal void LEngineReferenceUpdate(LReference reference)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceUpdate(reference);
        }
    }

    internal void LEngineReferenceAttach(long ownerId, long referenceId, int position, LOwner owner)
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

    internal void LEngineReferenceDetach(long ownerId, long referenceId, LOwner owner)
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

    internal void LEngineReferenceDelete(long id)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    internal void LEngineReferenceDelete(long id, bool detach)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, id);
    }

    internal LAuthor LEngineAuthorCreate(LAuthor author)
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

    public LAuthor? LEngineAuthorRead(long id)
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

    public IReadOnlyList<LAuthor> LEngineAuthorFind(string query)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);

            string written = query.Trim();
            List<LAuthor> found = [];
            foreach (LAuthor author in new LAuthorArchive(_lEngineDatabase).LAuthorAllRead())
            {
                if (LCatalog.LCatalogTextMatch(author.LAuthorName, written))
                {
                    found.Add(author);
                }
            }

            return found;
        }
    }

    public IReadOnlyList<LAuthor> LEngineBylineFind(long draft, string query, int limit)
    {
        if (draft == 0)
        {
            return [];
        }

        IReadOnlyList<LAuthor> credited = LEngineDraftRead(draft)?.LDraftAuthor ?? [];
        List<LAuthor> offered = [];
        foreach (LAuthor author in LEngineAuthorFind(query))
        {
            if (!author.LAuthorNamed || LEngineAuthorCheck(credited, author.LAuthorId))
            {
                continue;
            }

            offered.Add(author with { LAuthorName = author.LAuthorName.Trim() });
            if (offered.Count == limit)
            {
                break;
            }
        }

        return offered;
    }

    private static bool LEngineAuthorCheck(IReadOnlyList<LAuthor> credited, long id)
    {
        foreach (LAuthor author in credited)
        {
            if (author.LAuthorMatch(id))
            {
                return true;
            }
        }

        return false;
    }

    public IReadOnlyList<LAuthor> LEngineAuthorRead(long ownerId, LOwner owner)
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

    public IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> LEngineAuthorRead(LOwner owner)
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

    internal void LEngineAuthorUpdate(LAuthor author)
    {
        lock (_lEngineGate)
        {
            new LAuthorArchive(_lEngineDatabase).LAuthorUpdate(author);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, author.LAuthorId);
    }

    internal void LEngineAuthorAttach(long referenceId, long authorId, int position)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceAuthorAttach(referenceId, authorId, position);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    internal void LEngineAuthorDetach(long referenceId, long authorId)
    {
        lock (_lEngineGate)
        {
            new LReferenceArchive(_lEngineDatabase).LReferenceAuthorDetach(referenceId, authorId);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, authorId);
    }

    internal void LEngineAuthorDelete(long id)
    {
        lock (_lEngineGate)
        {
            new LAuthorArchive(_lEngineDatabase).LAuthorDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }

    internal void LEngineAuthorDelete(long id, bool detach)
    {
        lock (_lEngineGate)
        {
            new LAuthorArchive(_lEngineDatabase).LAuthorDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, id);
    }
}
