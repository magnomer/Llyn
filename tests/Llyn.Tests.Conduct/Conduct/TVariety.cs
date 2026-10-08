using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TVariety
{
    [Fact]
    public void VarietyRead_NamedOrBlankVariety_KeysTheLabelAndTheFlag()
    {
        Assert.Equal(
            new CVariety("Scottish", "Variety.Scottish", "English/Scottish"),
            CVariety.CVarietyRead("English", "Scottish"));
        Assert.Equal(
            new CVariety(string.Empty, "Variety.", string.Empty),
            CVariety.CVarietyRead("English", string.Empty));
    }
}
