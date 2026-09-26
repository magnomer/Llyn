using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TWindowMeaning
{
    [Fact]
    public void WindowMeaningSort_NestedMeanings_OrdersDepthFirst()
    {
        LStateValue blank = LStateValue.LStateValueUnspecified;
        LMeaning[] meanings =
        [
            TInterface.TMeaningCreate(2, 1, null, 2, blank, blank),
            TInterface.TMeaningCreate(3, 1, 2, 1, TInterface.TStateValueCreate("child"), blank),
            TInterface.TMeaningCreate(1, 1, null, 1, TInterface.TStateValueCreate("first"), blank),
        ];

        (long, string, int)[] expected = [(1, "first", 0), (2, "?", 0), (3, "child", 1)];

        Assert.Equal(expected, TInterfaceDeportment.TWindowMeaningSort(meanings, "?"));
    }
}
