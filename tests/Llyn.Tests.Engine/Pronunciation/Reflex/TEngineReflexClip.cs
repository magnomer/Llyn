using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineReflexClip
{
    [Fact]
    public async Task ReflexRead_InvalidClip_LoadsRuleWithoutClip()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "reflex": [
              { "language": "Korean", "url": "https://example.test/wiki/{word}", "match": "(?<text>x)",
                "epithet": "{text}", "clip": "(" },
              { "language": "Korean", "url": "https://example.test/other/{word}", "match": "(?<text>y)",
                "epithet": "{text}", "clip": "\\(.*\\)" } ] }
            """);
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart(TPronunciationHelper.TSourceClientCreate(
            string.Empty, System.Net.HttpStatusCode.NotFound));

        await TReflexFixture.TReflexFetchRead(engine, "x", pack.TLanguageFixtureName);
        IReadOnlyList<LReflexRule> rules = engine.TEngineReflexRead(pack.TLanguageFixtureName);

        Assert.Equal([null, "\\(.*\\)"], rules.Select(rule => rule.LReflexRuleClip));
    }
}
