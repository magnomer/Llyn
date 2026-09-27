using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TQuill
{
    [Fact]
    public void AuthorSet_FreshAuthor_WritesName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectAuthor, null);

        tenure.TQuillCreate().TQuillAuthorSet("Ada");

        Assert.Equal("Ada", tenure.TTenureRead()!.LDraftAuthorName);
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);
    }

    [Fact]
    public void ExampleSet_TextTypedOverUnknown_WritesKnownText()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueResolve(null, true));

        tenure.TQuillCreate().TQuillExampleSet("a dog");

        LExample held = tenure.TTenureRead()!.LDraftExample!;
        Assert.Equal("a dog", held.LExampleText.TStateValueShow());
        Assert.False(held.LExampleText.LStateValueUncertain);
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);
    }

    [Fact]
    public void SpeakerSet_LanguagePicked_WritesLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        tenure.TQuillCreate().TQuillSpeakerSet("French");

        Assert.Equal("French", tenure.TTenureRead()!.LDraftExample!.LExampleLanguage);
    }

    [Fact]
    public void ReferenceSet_ReferencePicked_CitesReference()
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
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        tenure.TQuillCreate().TQuillReferenceSet(reference.LReferenceId);

        Assert.Equal(
            reference.LReferenceId, tenure.TTenureRead()!.LDraftExample!.LExampleSource.TStateAnchorShow());
    }

    [Fact]
    public void GlossAdd_LanguageGiven_AddsGlossInLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        tenure.TQuillCreate().TQuillGlossAdd("English", 0);

        Assert.Equal("English", Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss).LGlossLanguage);
    }

    [Fact]
    public void GlossRemove_AddedGloss_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));
        LQuill quill = tenure.TQuillCreate();

        quill.TQuillGlossAdd("English", 0);
        quill.TQuillGlossRemove(Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss).LGlossId);

        Assert.Empty(tenure.TTenureRead()!.LDraftExample!.LExampleGloss);
    }

    [Fact]
    public void GlossSet_TextThenLanguage_WritesBoth()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));
        LQuill quill = tenure.TQuillCreate();

        quill.TQuillGlossAdd("English", 0);
        long gloss = Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss).LGlossId;
        quill.TQuillGlossSet(gloss, null, "un chat");
        quill.TQuillGlossSet(gloss, "French", null);

        LGloss held = Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss);
        Assert.Equal("French", held.LGlossLanguage);
        Assert.Equal("un chat", held.LGlossText.TStateValueShow());
    }

    [Fact]
    public void MentionAdd_SpanOverEntry_LinksWithoutSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TQuillEntrySave(engine);
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        tenure.TQuillCreate().TQuillMentionAdd(2, 3, entry.LEntryId);

        LMention added = Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleMention);
        Assert.Equal(2, added.LMentionOffset);
        Assert.Equal(3, added.LMentionLength);
        Assert.Equal(entry.LEntryId, added.LMentionEntryId);
        Assert.Equal(0, added.LMentionSenseId);
    }

    [Fact]
    public void MentionRemove_AddedMention_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TQuillEntrySave(engine);
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));
        LQuill quill = tenure.TQuillCreate();

        quill.TQuillMentionAdd(2, 3, entry.LEntryId);
        quill.TQuillMentionRemove(Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleMention).LMentionId);

        Assert.Empty(tenure.TTenureRead()!.LDraftExample!.LExampleMention);
    }

    [Fact]
    public void MentionSet_SensePicked_PointsMentionAtSense()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = TQuillEntrySave(engine);
        long sense = engine.TEngineMeaningRead(entry.LEntryId, LOwner.LOwnerEntry)[0].LMeaningId;
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));
        LQuill quill = tenure.TQuillCreate();

        quill.TQuillMentionAdd(2, 3, entry.LEntryId);
        quill.TQuillMentionSet(Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleMention).LMentionId, sense);

        Assert.Equal(sense, Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleMention).LMentionSenseId);
    }

    [Fact]
    public void SituationSet_UnknownKindLeftEmpty_KeepsKindUnknown()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        LSituation stored = engine.TEngineSituationCreate(
            TInterface.TSituationCreate(0, "Hearth", null, LStateValue.LStateValueUnknown));
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectSituation, stored.LSituationId);

        tenure.TQuillCreate().TQuillSituationSet("Fire", string.Empty, string.Empty);

        LSituation held = tenure.TTenureRead()!.LDraftSituation!;
        Assert.Equal("Fire", held.LSituationTitle.TStateValueShow());
        Assert.True(held.LSituationKind.LStateValueUncertain);
        Assert.True(held.LSituationDescription.LStateValueEmpty);
    }

    private static LTenure TQuillExampleStart(LEngine engine, LStateValue text)
    {
        engine.TEngineDelaySet(0);
        LExample stored = engine.TEngineExampleCreate(
            TInterface.TExampleCreate(0, "English", text, null, LStateAnchor.LStateAnchorUnspecified));
        return engine.TEngineTenureStart("test", LSubject.LSubjectExample, stored.LExampleId);
    }

    private static LEntry TQuillEntrySave(LEngine engine)
    {
        return engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "cat",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a small feline", [], [], [], [], [], 1)],
            []));
    }
}
