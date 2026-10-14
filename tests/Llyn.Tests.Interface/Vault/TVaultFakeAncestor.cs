using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeAncestor : LAncestorVault
{
    private readonly Dictionary<long, IReadOnlyList<LAncestor>> _tVaultFakeMaps = [];
    private readonly List<LAncestorRelation> _tVaultFakeRelations =
    [
        new(1, "borrowed", true),
        new(2, "compound", true),
        new(3, "derived", true),
        new(4, "calque", true),
        new(5, "blend", true),
    ];

    private readonly List<LAncestorTongue> _tVaultFakeTongues = [];
    private long _tVaultFakeNext = 100;

    public IReadOnlyList<LAncestor> LAncestorRead(long entryId) =>
        _tVaultFakeMaps.TryGetValue(entryId, out IReadOnlyList<LAncestor>? held) ? held : [];

    public IReadOnlyList<LAncestor> LAncestorSave(long entryId, IReadOnlyList<LAncestor> ancestors)
    {
        Dictionary<long, long> stored = [];
        foreach (LAncestor ancestor in ancestors)
        {
            stored[ancestor.LAncestorId] = ++_tVaultFakeNext;
        }

        List<LAncestor> written = [];
        foreach (LAncestor ancestor in ancestors)
        {
            written.Add(ancestor with
            {
                LAncestorId = stored[ancestor.LAncestorId],
                LAncestorChildId = ancestor.LAncestorChildId is long child ? stored[child] : null,
            });
        }

        _tVaultFakeMaps[entryId] = written;
        return written;
    }

    public IReadOnlyList<LAncestorRelation> LAncestorRelationRead() =>
        _tVaultFakeRelations
            .Select(relation => relation with { LAncestorRelationUsage = TVaultFakeScan(
                ancestor => ancestor.LAncestorRelationId == relation.LAncestorRelationId) })
            .ToList();

    public LAncestorRelation LAncestorRelationCreate(string name)
    {
        LAncestorRelation? held = _tVaultFakeRelations.Find(relation => relation.LAncestorRelationName == name.Trim());
        if (held is not null)
        {
            return held;
        }

        LAncestorRelation created = new(++_tVaultFakeNext, name.Trim());
        _tVaultFakeRelations.Add(created);
        return created;
    }

    public bool LAncestorRelationDelete(long relationId)
    {
        int removed = _tVaultFakeRelations.RemoveAll(
            relation => relation.LAncestorRelationId == relationId && !relation.LAncestorRelationBuiltin);
        TVaultFakeClear(
            ancestor => removed > 0 && ancestor.LAncestorRelationId == relationId,
            ancestor => ancestor with { LAncestorRelationId = null });
        return removed > 0;
    }

    public IReadOnlyList<LAncestorTongue> LAncestorTongueRead() =>
        _tVaultFakeTongues
            .Select(tongue => tongue with { LAncestorTongueUsage = TVaultFakeScan(
                ancestor => ancestor.LAncestorTongueId == tongue.LAncestorTongueId) })
            .ToList();

    public LAncestorTongue LAncestorTongueCreate(string name)
    {
        LAncestorTongue? held = _tVaultFakeTongues.Find(tongue => tongue.LAncestorTongueName == name.Trim());
        if (held is not null)
        {
            return held;
        }

        LAncestorTongue created = new(++_tVaultFakeNext, name.Trim());
        _tVaultFakeTongues.Add(created);
        return created;
    }

    public bool LAncestorTongueDelete(long tongueId)
    {
        int removed = _tVaultFakeTongues.RemoveAll(tongue => tongue.LAncestorTongueId == tongueId);
        TVaultFakeClear(
            ancestor => removed > 0 && ancestor.LAncestorTongueId == tongueId,
            ancestor => ancestor with { LAncestorTongueId = null });
        return removed > 0;
    }

    private int TVaultFakeScan(System.Func<LAncestor, bool> uses) =>
        _tVaultFakeMaps.Values.Sum(map => map.Count(uses));

    private void TVaultFakeClear(System.Func<LAncestor, bool> uses, System.Func<LAncestor, LAncestor> cleared)
    {
        foreach (long entryId in _tVaultFakeMaps.Keys.ToList())
        {
            _tVaultFakeMaps[entryId] = _tVaultFakeMaps[entryId]
                .Select(ancestor => uses(ancestor) ? cleared(ancestor) : ancestor)
                .ToList();
        }
    }
}
