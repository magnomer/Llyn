using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TAnthologySeal
{
    [Fact]
    public void AnthologyExampleRead_NoExample_ReturnsNone()
    {
        Assert.Null(TInterfaceGate.TAnthologyExampleRead(null));
    }

    [Fact]
    public void AnthologyExampleRead_SoundText_LinksExcerpt()
    {
        LExample example = TInterface.TExampleCreate(
                5, "English", TInterface.TStateValueCreate("a cat"), null, TInterface.TStateAnchorRead(9))
            .TExampleMentionAdd(TInterface.TMentionCreate(1, 2, 3, 40));

        CExample? held = TInterfaceGate.TAnthologyExampleRead(example);

        Assert.NotNull(held);
        Assert.Equal("English", held!.CExampleLanguage);
        Assert.Equal("a cat", held.CExampleText.CStateValueText);
        Assert.Equal(9, held.CExampleSource);
        Assert.Equal([40L], held.CExampleMention.Select(mention => mention.CMentionDraftEntry));
        Assert.Equal([40L], held.CExampleExcerpt.Select(mention => mention.CMentionEntry));
    }

    [Fact]
    public void AnthologyExampleRead_UnknownText_LeavesExcerptUnlinked()
    {
        LExample example = TInterface.TExampleCreate(
                5, "English", TInterface.TStateValueResolve(null, true), null, TInterface.TStateAnchorRead(null))
            .TExampleMentionAdd(TInterface.TMentionCreate(1, 2, 3, 40));

        CExample? held = TInterfaceGate.TAnthologyExampleRead(example);

        Assert.NotNull(held);
        Assert.True(held!.CExampleText.CStateValueUncertain);
        Assert.Null(held.CExampleSource);
        Assert.Single(held.CExampleMention);
        Assert.Empty(held.CExampleExcerpt);
    }

    [Fact]
    public void AnthologyMentionRead_StoredMention_CarriesLink()
    {
        CMentionResult held = TInterfaceGate.TAnthologyMentionRead(2, TInterface.TMentionCreate(1, 2, 3, 40, 7));

        Assert.Equal(2, held.CMentionResultOffset);
        Assert.Equal(40, held.CMentionResultStored?.CMentionEntry);
        Assert.Equal(7, held.CMentionResultStored?.CMentionSense);
        Assert.False(held.CMentionResultSingle);
        Assert.Equal(0, held.CMentionResultFirst);
    }
}
