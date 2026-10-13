using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TCandidate
{
    [Fact]
    public void CandidateRegional_VarietyNamed_IsTrue()
    {
        Assert.True(new CCandidate("Wiktionary", "ˈhæpi", 0, true, "American", null).CCandidateRegional);
    }

    [Fact]
    public void CandidateRegional_NoVariety_IsFalse()
    {
        Assert.False(new CCandidate("Wiktionary", "ˈhæpi", 0, true, string.Empty, null).CCandidateRegional);
    }
}
