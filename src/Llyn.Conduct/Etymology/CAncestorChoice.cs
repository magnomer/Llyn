using System;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CAncestorChoice(
    long? CAncestorChoiceId,
    string CAncestorChoiceKey,
    string CAncestorChoiceName,
    bool CAncestorChoiceCustom,
    bool CAncestorChoiceAddition)
{
    internal static CAncestorChoice CAncestorChoiceRead(LAncestorRelation relation)
    {
        ArgumentNullException.ThrowIfNull(relation);

        return new CAncestorChoice(
            relation.LAncestorRelationId,
            relation.LAncestorRelationKey,
            relation.LAncestorRelationName,
            !relation.LAncestorRelationBuiltin,
            false);
    }
}
