using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupReference
{
    [Fact]
    public void MarkupImport_ReferenceWithoutTitle_CreatesOwnReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LReference blank = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindOther,
            TInterfaceState.TStateValueCreate("a note"),
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));

        string text = TMarkupLink.TMarkupOwn.Replace(
            "</example>",
            "<reference><kind>web</kind></reference></example>");
        string path = TInterface.TMarkupSave(workspace, text);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        LEntryDraft draft = Assert.IsType<LEntryDraft>(
            engine.TEngineEntryLoad(Assert.Single(outcome.TMarkupOutcomeEntry).LEntryId));
        long? cited = TMarkupLink.TExampleRead(Assert.Single(draft.LEntryDraftMeanings))
            .LExampleDraftReference.LStateAnchorId;
        Assert.NotNull(cited);
        Assert.NotEqual(blank.LReferenceId, cited);
    }

    [Fact]
    public void MarkupImport_TextlessReference_CreatesNoReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        const string text = """
            <llyn>
              <entry>
                <headword>glow</headword>
                <language>English</language>
                <meaning>
                  <definition>give out light</definition>
                  <sentence>
                    <particle>up</particle>
                    <dependence>on</dependence>
                    <example>
                      <reference><title>A New Book</title></reference>
                    </example>
                  </sentence>
                </meaning>
              </entry>
            </llyn>
            """;
        int before = engine.TEngineReferenceRead().Count;
        string path = TInterface.TMarkupSave(workspace, text);
        TMarkupOutcome outcome = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);

        Assert.Single(outcome.TMarkupOutcomeEntry);
        Assert.Equal(before, engine.TEngineReferenceRead().Count);
    }

    [Fact]
    public void MarkupImport_MergeTwiceBareReference_KeepsOneReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string text = TMarkupLink.TMarkupOwn.Replace(
            "</example>",
            "<reference><kind>web</kind></reference></example>");
        string path = TInterface.TMarkupSave(workspace, text);
        TMarkupOutcome first = engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path), [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew)]);
        int stored = engine.TEngineReferenceRead().Count;

        long glowId = Assert.Single(first.TMarkupOutcomeEntry).LEntryId;
        engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeMerge, glowId)]);

        Assert.Equal(stored, engine.TEngineReferenceRead().Count);
    }
}
