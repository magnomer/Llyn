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
    public void GlossInsert_PlaceGiven_AddsGlossInTheGlossLanguageAtThePlace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));

        tenure.TTenureGlossInsert(0);

        LGloss added = Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss);
        Assert.Equal(engine.TEngineGlossRead(), added.LGlossLanguage);
    }

    [Fact]
    public void GlossRemove_AddedGloss_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillExampleStart(engine, TInterface.TStateValueCreate("a cat"));
        LQuill quill = tenure.TQuillCreate();

        tenure.TTenureGlossInsert(0);
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

        tenure.TTenureGlossInsert(0);
        long gloss = Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss).LGlossId;
        quill.TQuillGlossSet(gloss, null, "un chat");
        quill.TQuillGlossSet(gloss, "French", null);

        LGloss held = Assert.Single(tenure.TTenureRead()!.LDraftExample!.LExampleGloss);
        Assert.Equal("French", held.LGlossLanguage);
        Assert.Equal("un chat", held.LGlossText.TStateValueShow());
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

        LQuillSituation quill = tenure.TQuillSituationCreate();
        quill.TQuillTitleSet("Fire");
        quill.TQuillDescriptionSet(string.Empty);
        quill.TQuillKindSet(string.Empty);

        LSituation held = tenure.TTenureRead()!.LDraftSituation!;
        Assert.Equal("Fire", held.LSituationTitle.TStateValueShow());
        Assert.True(held.LSituationKind.LStateValueUncertain);
        Assert.True(held.LSituationDescription.LStateValueEmpty);
    }

    [Fact]
    public void EtymologySet_TextTyped_WritesNarrative()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TQuillEntryStart(engine);

        tenure.TQuillCreate().TQuillEtymologySet("from Latin");

        Assert.Equal("from Latin", tenure.TTenureRead()!.LDraftContent.LEntryDraftEtymology.LEtymologyDraftText);
    }

    [Fact]
    public void EtymonAdd_TwoSources_KeepsTheGivenOrder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cattus", "Latin").LEntryId;
        long dog = engine.TEngineTranslationCreate("canis", "Latin").LEntryId;
        LTenure tenure = TQuillEntryStart(engine);
        LQuill quill = tenure.TQuillCreate();

        quill.TQuillEtymonAdd(cat, int.MaxValue);
        quill.TQuillEtymonAdd(dog, 0);

        Assert.Equal([dog, cat], tenure.TTenureRead()!.LDraftContent.LEntryDraftEtymology.LEtymologyDraftEtymons);
    }

    [Fact]
    public void EtymonRemove_AddedSource_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cattus", "Latin").LEntryId;
        LTenure tenure = TQuillEntryStart(engine);
        LQuill quill = tenure.TQuillCreate();
        quill.TQuillEtymonAdd(cat, int.MaxValue);

        quill.TQuillEtymonRemove(cat);

        Assert.Empty(tenure.TTenureRead()!.LDraftContent.LEntryDraftEtymology.LEtymologyDraftEtymons);
    }

    [Fact]
    public void MentionSave_SpanOverEntry_LinksNarrativeSpan()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        long cat = engine.TEngineTranslationCreate("cattus", "Latin").LEntryId;
        LTenure tenure = TQuillEntryStart(engine);
        LQuill quill = tenure.TQuillCreate();
        quill.TQuillEtymologySet("from cattus");

        quill.TQuillMentionSave(5, 6, cat);

        LMentionDraft linked = Assert.Single(
            tenure.TTenureRead()!.LDraftContent.LEntryDraftEtymology.LEtymologyDraftMentions);
        Assert.Equal(cat, linked.LMentionDraftEntry);
        Assert.Equal(5, linked.LMentionDraftOffset);
    }

    [Fact]
    public void CitationSet_SourcePicked_CitesSentence()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LReference notes = engine.TEngineCitationCreate("Field notes");
        LTenure tenure = TQuillEntryStart(engine);
        LDraft carded = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(tenure.LTenureId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        long card = carded.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
        LDraft rowed = engine.TEngineRequestApply(TInterface.TSentenceAdditionCreate(tenure.LTenureId, card, 0));
        long sentence = TInterface.TRequestCardFind(rowed.LDraftContent, card).LCardDraftSentence[0].LSentenceDraftId;

        tenure.TQuillCreate().TQuillCitationSet(card, sentence, notes.LReferenceId);

        LExampleDraft? example = TInterface.TRequestCardFind(tenure.TTenureRead()!.LDraftContent, card)
            .LCardDraftSentence[0].LSentenceDraftExample;
        Assert.Equal(notes.LReferenceId, example?.LExampleDraftReference.LStateAnchorShown);
    }

    private static LTenure TQuillEntryStart(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        return engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
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
