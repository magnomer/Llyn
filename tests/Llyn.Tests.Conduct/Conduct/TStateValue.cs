using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TStateValue
{
    [Fact]
    public void StateValueShown_WrittenText_ReadsTheText()
    {
        Assert.Equal("wolf", new CStateValue("wolf", false).CStateValueShown);
    }

    [Fact]
    public void StateValueShown_EmptyValue_ReadsNothing()
    {
        Assert.Null(CStateValue.CStateValueEmpty.CStateValueShown);
    }
}
