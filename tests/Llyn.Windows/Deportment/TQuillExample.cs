using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TQuillExample
{
    [Fact]
    public void ExampleChange_TextTypedOverUnknown_WritesKnownText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDesk desk = TQuillExampleStart(engine, TInterface.TStateValueResolve(null, true));

        desk.TQuillExampleChange(CExampleField.CExampleFieldText, "a dog");

        LExample held = desk.TDeskRead()!.LDraftExample!;
        Assert.Equal("a dog", held.LExampleText.TStateValueShow());
        Assert.False(held.LExampleText.LStateValueUncertain);
        Assert.True(desk.TDeskChangeCheck());
    }

    [Fact]
    public void ExampleChange_LanguagePicked_WritesLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDesk desk = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        desk.TQuillExampleChange(CExampleField.CExampleFieldLanguage, "French");

        Assert.Equal("French", desk.TDeskRead()!.LDraftExample!.LExampleLanguage);
    }

    [Fact]
    public void CitationSet_ReferencePicked_CitesReference()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("Bestiary"),
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnknown,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
        LDesk desk = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        desk.TQuillCitationSet(reference.LReferenceId);

        Assert.Equal(reference.LReferenceId, desk.TDeskRead()!.LDraftExample!.LExampleSource.TStateAnchorShow());
    }

    [Fact]
    public void GlossAdd_ThenTextAndLanguageChanged_WritesGloss()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDesk desk = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        desk.TQuillGlossAdd("English", 0);
        long gloss = Assert.Single(desk.TDeskRead()!.LDraftExample!.LExampleGloss).LGlossId;
        desk.TQuillGlossChange(gloss, CGlossField.CGlossFieldText, "un chat");
        desk.TQuillGlossChange(gloss, CGlossField.CGlossFieldLanguage, "French");

        LGloss held = Assert.Single(desk.TDeskRead()!.LDraftExample!.LExampleGloss);
        Assert.Equal("French", held.LGlossLanguage);
        Assert.Equal("un chat", held.LGlossText.TStateValueShow());
    }

    [Fact]
    public void GlossRemove_AddedGloss_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDesk desk = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        desk.TQuillGlossAdd("English", 0);
        desk.TQuillGlossRemove(Assert.Single(desk.TDeskRead()!.LDraftExample!.LExampleGloss).LGlossId);

        Assert.Empty(desk.TDeskRead()!.LDraftExample!.LExampleGloss);
    }

    [Fact]
    public void MentionAdd_ThenSenseChangedAndRemoved_FollowsTheMention()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "cat",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a small feline", [], [], [], [], [], 1)],
            []));
        long sense = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        LDesk desk = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        desk.TQuillMentionAdd(2, 3, entry.LEntryId);
        LMention added = Assert.Single(desk.TDeskRead()!.LDraftExample!.LExampleMention);
        desk.TQuillMentionChange(added.LMentionId, sense);
        LMention sensed = Assert.Single(desk.TDeskRead()!.LDraftExample!.LExampleMention);
        desk.TQuillMentionRemove(added.LMentionId);

        Assert.Equal(2, added.LMentionOffset);
        Assert.Equal(3, added.LMentionLength);
        Assert.Equal(entry.LEntryId, added.LMentionEntryId);
        Assert.Equal(sense, sensed.LMentionSenseId);
        Assert.Empty(desk.TDeskRead()!.LDraftExample!.LExampleMention);
    }

    private static LDesk TQuillExampleStart(LEngine engine, LStateValue text)
    {
        LExample stored = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(0, "English", text, null, LStateAnchor.LStateAnchorUnspecified));
        LDesk desk = TInterfaceDeportment.TDeskCreate(engine, "Example", static () => false);
        desk.TDeskExampleStart(stored.LExampleId);
        return desk;
    }
}
