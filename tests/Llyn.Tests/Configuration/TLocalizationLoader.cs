using System.Text.Json;
using Xunit;

namespace Llyn.Tests;

public sealed class TLocalizationLoader
{
    [Fact]
    public void LocalizationLoaderOpen_EmbeddedEnglish_YieldsJsonObject()
    {
        using TextReader reader = TInterface.TLocalizationLoaderOpen("en");

        using JsonDocument document = JsonDocument.Parse(reader.ReadToEnd());

        Assert.Equal(JsonValueKind.Object, document.RootElement.ValueKind);
    }

    [Fact]
    public void LocalizationLoaderOpen_UnknownLanguage_Throws()
    {
        Assert.Throws<InvalidDataException>(() => TInterface.TLocalizationLoaderOpen("fr"));
    }
}
