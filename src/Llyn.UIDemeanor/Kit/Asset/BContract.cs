using System;
using System.Collections.Generic;

namespace Llyn.UIDemeanor;

internal static class BContract
{
    internal static BBlueprintPart BContractBlueprintFind(BBlueprint scope, string id)
    {
        ArgumentNullException.ThrowIfNull(scope);
        ArgumentException.ThrowIfNullOrEmpty(id);

        return scope.BBlueprintParts.TryGetValue(id, out BBlueprintPart? found)
            ? found
            : throw new InvalidOperationException($"The contract blueprint '{id}' is missing.");
    }

    internal static BBlueprintPart BContractFind(BBlueprintPart scope, string id)
    {
        ArgumentNullException.ThrowIfNull(scope);
        ArgumentException.ThrowIfNullOrEmpty(id);

        Stack<BBlueprintPart> parts = new([scope]);
        while (parts.TryPop(out BBlueprintPart? part))
        {
            if (part.BBlueprintPartName == id)
            {
                return part;
            }

            foreach (BBlueprintPart child in part.BBlueprintPartChildren)
            {
                parts.Push(child);
            }
        }

        throw new InvalidOperationException($"The contract part '{id}' is missing.");
    }
}
