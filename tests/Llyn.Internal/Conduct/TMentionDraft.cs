using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TMentionDraft
{
    [Fact]
    public void MentionDraftLinked_EntryHeld_IsTrue()
    {
        Assert.True(new CMentionDraft(3, 7, 0, 2, 0).CMentionDraftLinked);
    }

    [Fact]
    public void MentionDraftLinked_NoEntry_IsFalse()
    {
        Assert.False(new CMentionDraft(3, 0, 0, 2, 0).CMentionDraftLinked);
    }
}
