using System.Collections.Generic;

namespace Llyn.Core;

public interface LAncestorVault
{
    IReadOnlyList<LAncestor> LAncestorRead(long entryId);

    IReadOnlyList<LAncestor> LAncestorSave(long entryId, IReadOnlyList<LAncestor> ancestors);

    IReadOnlyList<LAncestorRelation> LAncestorRelationRead();

    LAncestorRelation LAncestorRelationCreate(string name);

    bool LAncestorRelationDelete(long relationId);

    IReadOnlyList<LAncestorTongue> LAncestorTongueRead();

    LAncestorTongue LAncestorTongueCreate(string name);

    bool LAncestorTongueDelete(long tongueId);
}
