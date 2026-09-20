using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);
            string written = query.Trim();

            IReadOnlyList<LReference> references = _lEngineReferences.LReferenceAllRead();
            IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits =
                _lEngineAuthors.LAuthorReferenceRead();
            IReadOnlyDictionary<long, int> usage = _lEngineReferences.LReferenceUsageRead();

            Dictionary<long, List<LReference>> works = LEngineWorkRead(references, credits);

            List<LCatalogAuthor> rows = [];
            foreach (LAuthor author in _lEngineAuthors.LAuthorAllRead())
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

    public LCatalogAuthor? LEngineAuthorFind(long id)
    {
        lock (_lEngineGate)
        {
            LAuthor? author = _lEngineAuthors.LAuthorRead(id);
            if (author is null)
            {
                return null;
            }

            IReadOnlyList<LReference> references = _lEngineReferences.LReferenceAllRead();
            IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits =
                _lEngineAuthors.LAuthorReferenceRead();
            IReadOnlyDictionary<long, int> usage = _lEngineReferences.LReferenceUsageRead();
            LEngineWorkRead(references, credits).TryGetValue(id, out List<LReference>? credited);
            return LCatalogAuthor.LCatalogAuthorCreate(author, credited, usage);
        }
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(string query, long except, int limit)
    {
        List<LCatalogAuthor> rows = [];
        foreach (LCatalogAuthor row in LEngineAuthorFind(query, LCatalogOrder.LCatalogOrderName))
        {
            if (row.LCatalogAuthorStored.LAuthorMatch(except))
            {
                continue;
            }

            rows.Add(row);
            if (rows.Count == limit)
            {
                break;
            }
        }

        return rows;
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista, string uncredited)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(uncredited);

        IReadOnlyList<LCatalogAuthor> rows = LEngineAuthorFind(vista);
        if (vista.LVistaQueried)
        {
            return rows;
        }

        IReadOnlyList<LCatalogReference> orphan = LEngineOeuvreFind(
            0, string.Empty, LCatalogFilter.LCatalogFilterEmpty, LCatalogOrder.LCatalogOrderName);
        if (orphan.Count == 0)
        {
            return rows;
        }

        int cited = 0;
        foreach (LCatalogReference row in orphan)
        {
            cited += row.LCatalogReferenceUsage;
        }

        return [new LCatalogAuthor(new LAuthor(0, uncredited), orphan.Count, cited, vista.LVistaMatch(0)), .. rows];
    }

    public IReadOnlyList<LCatalogAuthor> LEngineAuthorFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogAuthor> found = LEngineAuthorFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogAuthor> rows = new(found.Count);
        string[] names = LEngineTwinRead(
            found, row => row.LCatalogAuthorName, row => row.LCatalogAuthorStored.LAuthorId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogAuthor row = found[index];
            rows.Add(row with
            {
                LCatalogAuthorName = names[index],
                LCatalogAuthorChosen = row.LCatalogAuthorStored.LAuthorId == vista.LVistaChosen,
            });
        }

        return rows;
    }

    public IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista roll, LVista oeuvre)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(oeuvre);
        IReadOnlyList<LCatalogReference> found = LEngineOeuvreFind(
            roll.LVistaChosen, oeuvre.LVistaQuery, roll.LVistaFilter, oeuvre.LVistaOrder);
        List<LCatalogReference> rows = new(found.Count);
        string[] names = LEngineTwinRead(
            found, row => row.LCatalogReferenceName, row => row.LCatalogReferenceStored.LReferenceId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogReference row = found[index];
            rows.Add(row with
            {
                LCatalogReferenceName = names[index],
                LCatalogReferenceChosen = row.LCatalogReferenceStored.LReferenceId == oeuvre.LVistaChosen,
            });
        }

        return rows;
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

            IReadOnlyList<LReference> references = _lEngineReferences.LReferenceAllRead();
            IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits =
                _lEngineAuthors.LAuthorReferenceRead();
            IReadOnlyDictionary<long, int> usage = _lEngineReferences.LReferenceUsageRead();

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

    public IReadOnlyList<LFellow> LEngineFellowFind(long authorId)
    {
        lock (_lEngineGate)
        {
            IReadOnlyList<LReference> references = _lEngineReferences.LReferenceAllRead();
            IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits =
                _lEngineAuthors.LAuthorReferenceRead();

            Dictionary<long, LFellow> shared = [];
            foreach (LReference reference in references)
            {
                if (!credits.TryGetValue(reference.LReferenceId, out IReadOnlyList<LAuthor>? credited)
                    || !LEngineOeuvreMatch(authorId, credited))
                {
                    continue;
                }

                foreach (LAuthor credit in credited)
                {
                    if (credit.LAuthorId == authorId)
                    {
                        continue;
                    }

                    shared[credit.LAuthorId] = shared.TryGetValue(credit.LAuthorId, out LFellow? held)
                        ? held with { LFellowShared = held.LFellowShared + 1 }
                        : new LFellow(credit.LAuthorId, credit.LAuthorName, 1);
                }
            }

            List<LFellow> fellows = [.. shared.Values];
            fellows.Sort((left, right) =>
            {
                int order = right.LFellowShared.CompareTo(left.LFellowShared);
                return order != 0
                    ? order
                    : string.Compare(left.LFellowName, right.LFellowName, StringComparison.CurrentCultureIgnoreCase);
            });
            return fellows;
        }
    }

    public void LEngineAuthorAbsorb(long kept, long dropped)
    {
        lock (_lEngineGate)
        {
            _lEngineAuthors.LAuthorAbsorb(kept, dropped);
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
