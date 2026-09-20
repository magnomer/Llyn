using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupNode
{
    [Fact]
    public void MarkupFileFormat_ParsedSample_RoundTripsByteForByte()
    {
        string sample = TMarkupSample.TMarkupSampleText.ReplaceLineEndings("\n");

        LMarkupNode root = TInterface.TMarkupNodeParse(sample);
        string written = TInterface.TMarkupNodeFormat(root);

        Assert.Equal(sample, written);
        Assert.Equal(sample, TInterface.TMarkupNodeFormat(TInterface.TMarkupNodeParse(written)));
    }

    [Fact]
    public void MarkupFileParse_Sample_CarriesLinesAndAttributes()
    {
        LMarkupNode root = TInterface.TMarkupNodeParse(TMarkupSample.TMarkupSampleText);

        Assert.Equal("llyn", root.LMarkupNodeName);
        Assert.Equal(1, root.LMarkupNodeLine);
        LMarkupNode entry = Assert.Single(root.LMarkupNodeChild);
        Assert.Equal(2, entry.LMarkupNodeLine);
        Assert.Equal("kindle", entry.LMarkupNodeChild[0].LMarkupNodeText);
        LMarkupNode? dependence = TMarkupNodeFind(entry, "dependence");
        Assert.NotNull(dependence);
        Assert.Equal("unknown", dependence.LMarkupNodeAttribute["state"]);
        Assert.Empty(dependence.LMarkupNodeChild);
    }

    [Fact]
    public void MarkupFileParse_NoRoot_Refuses()
    {
        LRefusal refusal = Assert.Throws<LRefusal>(() => TInterface.TMarkupNodeParse("not markup"));

        Assert.Equal(LRefusal.LRefusalMarkup, refusal.Message);
    }

    [Fact]
    public void MarkupFileFormat_ControlCharacter_DropsIt()
    {
        LMarkupNode root = TInterface.TMarkupNodeCreate(
            "llyn", [TInterface.TMarkupNodeCreate("note", "em\u0001ber")]);

        string written = TInterface.TMarkupNodeFormat(root);

        Assert.Equal("<llyn>\n  <note>ember</note>\n</llyn>", written);
    }

    private static LMarkupNode? TMarkupNodeFind(LMarkupNode node, string name)
    {
        foreach (LMarkupNode child in node.LMarkupNodeChild)
        {
            if (child.LMarkupNodeName == name)
            {
                return child;
            }

            if (TMarkupNodeFind(child, name) is LMarkupNode found)
            {
                return found;
            }
        }

        return null;
    }
}
