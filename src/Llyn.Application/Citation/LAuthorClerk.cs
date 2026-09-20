using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LAuthorClerk
{
    private readonly LAuthorVault _lAuthorClerkAuthors;
    private readonly LReferenceVault _lAuthorClerkReferences;

    public LAuthorClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lAuthorClerkAuthors = rig.LRigAuthors;
        _lAuthorClerkReferences = rig.LRigReferences;
    }

    public LAuthor LAuthorClerkCreate(LAuthor author)
    {
        ArgumentNullException.ThrowIfNull(author);
        ArgumentException.ThrowIfNullOrWhiteSpace(author.LAuthorName);
        return _lAuthorClerkAuthors.LAuthorCreate(author);
    }

    public LAuthor? LAuthorClerkRead(long id)
    {
        return _lAuthorClerkAuthors.LAuthorRead(id);
    }

    public IReadOnlyList<LAuthor> LAuthorClerkRead()
    {
        return _lAuthorClerkAuthors.LAuthorAllRead();
    }

    public IReadOnlyList<LAuthor> LAuthorClerkRead(long ownerId, LOwner owner)
    {
        if (owner != LOwner.LOwnerReference)
        {
            throw LAuthorOwnerRaise(owner);
        }

        return _lAuthorClerkAuthors.LAuthorReferenceRead(ownerId);
    }

    public IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> LAuthorClerkRead(LOwner owner)
    {
        if (owner != LOwner.LOwnerReference)
        {
            throw LAuthorOwnerRaise(owner);
        }

        return _lAuthorClerkAuthors.LAuthorReferenceRead();
    }

    public void LAuthorClerkUpdate(LAuthor author)
    {
        ArgumentNullException.ThrowIfNull(author);
        _lAuthorClerkAuthors.LAuthorUpdate(author);
    }

    public void LAuthorClerkAttach(long referenceId, long authorId, int position)
    {
        _lAuthorClerkReferences.LReferenceAuthorAttach(referenceId, authorId, position);
    }

    public void LAuthorClerkDetach(long referenceId, long authorId)
    {
        _lAuthorClerkReferences.LReferenceAuthorDetach(referenceId, authorId);
    }

    public void LAuthorClerkDelete(long id)
    {
        _lAuthorClerkAuthors.LAuthorDelete(id);
    }

    public void LAuthorClerkDelete(long id, bool detach)
    {
        _lAuthorClerkAuthors.LAuthorDelete(id, detach);
    }

    public void LAuthorClerkAbsorb(long kept, long dropped)
    {
        _lAuthorClerkAuthors.LAuthorAbsorb(kept, dropped);
    }

    public IReadOnlyList<LAuthor> LAuthorClerkFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        string written = query.Trim();
        List<LAuthor> found = [];
        foreach (LAuthor author in _lAuthorClerkAuthors.LAuthorAllRead())
        {
            if (LCatalog.LCatalogTextMatch(author.LAuthorName, written))
            {
                found.Add(author);
            }
        }

        return found;
    }

    public IReadOnlyList<LCatalogAuthor> LAuthorClerkFind(string query, LCatalogOrder order)
    {
        ArgumentNullException.ThrowIfNull(query);
        string written = query.Trim();

        IReadOnlyList<LReference> references = _lAuthorClerkReferences.LReferenceAllRead();
        IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits = _lAuthorClerkAuthors.LAuthorReferenceRead();
        IReadOnlyDictionary<long, int> usage = _lAuthorClerkReferences.LReferenceUsageRead();

        Dictionary<long, List<LReference>> works = LAuthorWorkRead(references, credits);

        List<LCatalogAuthor> rows = [];
        foreach (LAuthor author in _lAuthorClerkAuthors.LAuthorAllRead())
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

    public LCatalogAuthor? LAuthorClerkFind(long id)
    {
        LAuthor? author = _lAuthorClerkAuthors.LAuthorRead(id);
        if (author is null)
        {
            return null;
        }

        IReadOnlyList<LReference> references = _lAuthorClerkReferences.LReferenceAllRead();
        IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits = _lAuthorClerkAuthors.LAuthorReferenceRead();
        IReadOnlyDictionary<long, int> usage = _lAuthorClerkReferences.LReferenceUsageRead();
        LAuthorWorkRead(references, credits).TryGetValue(id, out List<LReference>? credited);
        return LCatalogAuthor.LCatalogAuthorCreate(author, credited, usage);
    }

    public IReadOnlyList<LCatalogAuthor> LAuthorClerkFind(string query, long except, int limit)
    {
        List<LCatalogAuthor> rows = [];
        foreach (LCatalogAuthor row in LAuthorClerkFind(query, LCatalogOrder.LCatalogOrderName))
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

    public IReadOnlyList<LAuthor> LBylineFind(IReadOnlyList<LAuthor> credited, string query, int limit)
    {
        ArgumentNullException.ThrowIfNull(credited);

        List<LAuthor> offered = [];
        foreach (LAuthor author in LAuthorClerkFind(query))
        {
            if (!author.LAuthorNamed || LAuthorCheck(credited, author.LAuthorId))
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

    public IReadOnlyList<LFellow> LFellowFind(long authorId)
    {
        IReadOnlyList<LReference> references = _lAuthorClerkReferences.LReferenceAllRead();
        IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits = _lAuthorClerkAuthors.LAuthorReferenceRead();

        Dictionary<long, LFellow> shared = [];
        foreach (LReference reference in references)
        {
            if (!credits.TryGetValue(reference.LReferenceId, out IReadOnlyList<LAuthor>? credited)
                || !LOeuvreMatch(authorId, credited))
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

    public IReadOnlyList<LAuthor> LAuthorReferenceRead(long referenceId)
    {
        return _lAuthorClerkAuthors.LAuthorReferenceRead(referenceId);
    }

    public void LAuthorReferenceSave(long referenceId, IReadOnlyList<LAuthor> authors)
    {
        ArgumentNullException.ThrowIfNull(authors);

        List<long> kept = new(authors.Count);
        foreach (LAuthor author in authors)
        {
            long id = author.LAuthorId switch
            {
                > 0 when _lAuthorClerkAuthors.LAuthorRead(author.LAuthorId) is not null => author.LAuthorId,
                > 0 => throw new LRefusal(LRefusal.LRefusalLink),
                _ => _lAuthorClerkAuthors.LAuthorCreate(new LAuthor(0, author.LAuthorName)).LAuthorId,
            };

            if (kept.Contains(id))
            {
                continue;
            }

            _lAuthorClerkReferences.LReferenceAuthorAttach(referenceId, id, kept.Count);
            kept.Add(id);
        }

        foreach (LAuthor credited in _lAuthorClerkAuthors.LAuthorReferenceRead(referenceId))
        {
            if (!kept.Contains(credited.LAuthorId))
            {
                _lAuthorClerkReferences.LReferenceAuthorDetach(referenceId, credited.LAuthorId);
            }
        }
    }

    public static bool LAuthorClerkMatch(IReadOnlyList<LAuthor> one, IReadOnlyList<LAuthor> other)
    {
        ArgumentNullException.ThrowIfNull(one);
        ArgumentNullException.ThrowIfNull(other);

        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LAuthorId != other[index].LAuthorId)
            {
                return false;
            }
        }

        return true;
    }

    public static bool LOeuvreMatch(long? author, IReadOnlyList<LAuthor>? credited)
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

    private static bool LAuthorCheck(IReadOnlyList<LAuthor> credited, long id)
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

    private static Dictionary<long, List<LReference>> LAuthorWorkRead(
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

    private static ArgumentOutOfRangeException LAuthorOwnerRaise(LOwner owner)
    {
        return new ArgumentOutOfRangeException(
            nameof(owner), owner, "This entity has no reference from that kind of row.");
    }
}
