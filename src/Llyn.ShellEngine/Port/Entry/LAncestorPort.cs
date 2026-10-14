using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LAncestorPort
{
    IReadOnlyList<LLineage> LEngineLineageRead(long entryId, IReadOnlyList<LAncestorDraft> map);

    IReadOnlyList<LAncestorRelation> LEngineRelationRead();

    LAncestorRelation LEngineRelationCreate(string name);

    int? LEngineRelationDelete(long relationId);

    IReadOnlyList<LAncestorLanguage> LEngineLanguageRead();

    LAncestorLanguage LEngineTongueCreate(string name);

    int? LEngineTongueDelete(long tongueId);
}
