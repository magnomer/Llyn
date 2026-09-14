using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);
            string written = query.Trim();

            IReadOnlyList<LReference> references = new LReferenceArchive(_lEngineDatabase).LReferenceAllRead();
            IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits =
                new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead();
            IReadOnlyDictionary<long, int> usage = new LReferenceUsage(_lEngineDatabase).LReferenceUsageRead();

            Dictionary<long, List<LReference>> works = LEngineWorkRead(references, credits);

            List<LCatalogAuthor> rows = [];
            foreach (LAuthor author in new LAuthorArchive(_lEngineDatabase).LAuthorAllRead())
            {
                works.TryGetValue(author.LAuthorId, out List<LReference>? credited);
                LCatalogAuthor row = LCatalogAuthor.LCatalogAuthorCreate(author, credited, usage);
                if (row.LCatalogAuthorMatch(written))
                {
                    rows.Add(row);
                }
            }

            return LCatalogAuthor.LCatalogAuthorSort(rows, order);
        }
    }

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(
        long? author,
        string query,
        LCatalogFilter kind,
        LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(kind);
            string written = query.Trim();

            IReadOnlyList<LReference> references = new LReferenceArchive(_lEngineDatabase).LReferenceAllRead();
            IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits =
                new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead();
            IReadOnlyDictionary<long, int> usage = new LReferenceUsage(_lEngineDatabase).LReferenceUsageRead();

            List<LCatalogReference> rows = [];
            foreach (LReference reference in references)
            {
                credits.TryGetValue(reference.LReferenceId, out IReadOnlyList<LAuthor>? credited);
                if (!LEngineOeuvreMatch(author, credited))
                {
                    continue;
                }

                if (!kind.LCatalogFilterMatch(LReference.LReferenceKindFormat(reference.LReferenceKind)))
                {
                    continue;
                }

                usage.TryGetValue(reference.LReferenceId, out int counted);
                LCatalogReference row = LCatalogReference.LCatalogReferenceCreate(reference, credited, counted);
                if (row.LCatalogReferenceMatch(written))
                {
                    rows.Add(row);
                }
            }

            return LCatalogReference.LCatalogReferenceSort(rows, order);
        }
    }

    public void LEngineAuthorAbsorb(long kept, long dropped)
    {
        lock (_lEngineGate)
        {
            new LAuthorArchive(_lEngineDatabase).LAuthorAbsorb(kept, dropped);
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, dropped);
        LEngineBulletinRaise(LSubject.LSubjectAuthor, kept);
    }

    private static bool LEngineOeuvreMatch(long? author, IReadOnlyList<LAuthor>? credited)
    {
        if (author is not long wanted)
        {
            return true;
        }

        if (wanted <= 0)
        {
            return credited is null || credited.Count == 0;
        }

        if (credited is null)
        {
            return false;
        }

        foreach (LAuthor credit in credited)
        {
            if (credit.LAuthorId == wanted)
            {
                return true;
            }
        }

        return false;
    }

    private static Dictionary<long, List<LReference>> LEngineWorkRead(
        IReadOnlyList<LReference> references,
        IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits)
    {
        Dictionary<long, List<LReference>> works = [];
        foreach (LReference reference in references)
        {
            if (!credits.TryGetValue(reference.LReferenceId, out IReadOnlyList<LAuthor>? credited))
            {
                continue;
            }

            foreach (LAuthor author in credited)
            {
                if (!works.TryGetValue(author.LAuthorId, out List<LReference>? held))
                {
                    held = [];
                    works[author.LAuthorId] = held;
                }

                held.Add(reference);
            }
        }

        return works;
    }
}
