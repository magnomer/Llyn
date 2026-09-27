using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TWindowMeaning
{
    [Fact]
    public void WorkspaceMeaningSort_NestedMeanings_OrdersDepthFirst()
    {
        LStateValue blank = LStateValue.LStateValueUnspecified;
        LMeaning[] meanings =
        [
            TInterface.TMeaningCreate(2, 1, null, 2, blank, blank),
            TInterface.TMeaningCreate(3, 1, 2, 1, TInterface.TStateValueCreate("child"), blank),
            TInterface.TMeaningCreate(1, 1, null, 1, TInterface.TStateValueCreate("first"), blank),
        ];

        CMeaning[] expected = [new(1, "first", 0), new(2, "?", 0), new(3, "child", 1)];

        Assert.Equal(expected, TInterfaceDeportment.TWorkspaceMeaningSort(meanings, "?"));
    }
}
