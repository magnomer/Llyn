using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineMention
{
    [Fact]
    public void MentionResolve_SenseNamed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "pattern",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate("model", string.Empty, "a thing copied", [], [], [], [], [], 1)],
            []));
        long sense = engine.TEngineEntryLoad(entry.LEntryId)!.LEntryDraftMeanings[0].LCardDraftId;

        IReadOnlyList<LMentionLabel> labels = engine.TEngineMentionResolve(
            "a pattern here",
            [
                TInterface.TMentionDraftCreate(7, 2, 7, entry.LEntryId, sense),
                TInterface.TMentionDraftCreate(8, 10, 4, 0),
            ]);

        Assert.Equal(
            [
                TInterface.TMentionLabelCreate(7, "pattern", entry.LEntryId, "pattern", "model"),
                TInterface.TMentionLabelCreate(8, "here", 0, string.Empty, string.Empty),
            ],
            labels);
    }
}
