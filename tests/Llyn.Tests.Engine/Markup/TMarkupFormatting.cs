using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupFormatting
{
    [Fact]
    public void MarkupFormat_ParsedEntry_RoundTripsEqual()
    {
        IReadOnlyList<LMarkupEntry> original = TInterface.TMarkupParse(TMarkupSample.TMarkupSampleText);

        string written = TInterface.TMarkupFormat(original);
        IReadOnlyList<LMarkupEntry> again = TInterface.TMarkupParse(
            written, out IReadOnlyList<LMarkupOmission> omissions);

        Assert.Empty(omissions);
        Assert.Equal(original, again);
        Assert.Equal(written, TInterface.TMarkupFormat(again));
    }

    [Fact]
    public void MarkupFormat_UnspecifiedFields_OmitsElements()
    {
        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <language>en</language>
                <meaning>
                  <title>glow</title>
                </meaning>
              </entry>
            </llyn>
            """;

        string written = TInterface.TMarkupFormat(TInterface.TMarkupParse(text));

        Assert.Contains("<title>glow</title>", written);
        Assert.DoesNotContain("definition", written);
        Assert.DoesNotContain("expression", written);
        Assert.DoesNotContain("note", written);
        Assert.DoesNotContain("pronunciation", written);
        Assert.DoesNotContain("<?xml", written);
    }

    [Fact]
    public void MarkupFormat_UnknownValue_WritesStateAttribute()
    {
        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <meaning>
                  <definition state="unknown" />
                </meaning>
              </entry>
            </llyn>
            """;

        string written = TInterface.TMarkupFormat(TInterface.TMarkupParse(text));

        Assert.Contains("<definition state=\"unknown\" />", written);
    }

    [Fact]
    public void MarkupFormat_ControlCharacter_DropsIt()
    {
        LMarkupEntry entry = TInterface.TMarkupEntryCreate("em\u0001ber", "en", note: "a\fnote");

        string written = TInterface.TMarkupFormat([entry]);
        LMarkupEntry again = Assert.Single(TInterface.TMarkupParse(written));

        Assert.Equal("ember", again.LMarkupEntryHeadword);
        Assert.Equal("anote", again.LMarkupEntryNote);
    }

    [Fact]
    public void MarkupFormat_EmptyLocal_RoundTripsEmpty()
    {
        LMarkupEntry entry = TInterface.TMarkupEntryCreate(
            "ember", "en", [TInterface.TFormCreate(0, 0, "embers", string.Empty, "plural")]);

        LMarkupEntry again = Assert.Single(TInterface.TMarkupParse(TInterface.TMarkupFormat([entry])));

        Assert.Equal(string.Empty, Assert.Single(again.LMarkupEntryForm).LFormLocal);
    }
}
