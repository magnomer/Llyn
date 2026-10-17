using Llyn.UIDemeanor;

namespace Llyn.Tests;

internal sealed class TBlueprintFake : BBlueprint
{
    internal TBlueprintFake()
    {
        BBlueprintLoad(this);
    }
}
