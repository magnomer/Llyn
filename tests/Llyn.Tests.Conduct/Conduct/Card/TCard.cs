using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TCard
{
    [Fact]
    public void CardStateRead_UnknownValue_CarriesTheUncertainVerdict()
    {
        Assert.Equal(
            new CStateValue(string.Empty, true),
            TInterfaceConductCard.TCardStateRead(LStateValue.LStateValueUnknown));
    }

    [Fact]
    public void CardEntryRead_UnknownBlankAndWrittenFields_WordsEachFieldWithItsSheetHint()
    {
        LEntryDraft draft = TInterface.TEntryDraftCreate(
            "water",
            "English",
            string.Empty,
            string.Empty,
            [
                TInterface.TCardCreate("a liquid", 1) with
                {
                    LCardDraftTitle = LStateValue.LStateValueUnknown,
                    LCardDraftRegister = [TInterface.TRegisterDraftCreate(LStateValue.LStateValueUnknown, 3)],
                },
            ],
            [TInterface.TCardCreate(string.Empty, 1)]);

        CEntryDraft shaped = TInterfaceConductCard.TCardEntryRead(
            draft, new Dictionary<long, IReadOnlyList<LTranslationTarget>> { [0] = [] });

        CCardDraft meaning = Assert.Single(shaped.CEntryDraftMeanings);
        Assert.Equal(
            new CStateWording(string.Empty, "Display.Unknown", false, "Display.Unknown"), meaning.CCardDraftTitle);
        Assert.Equal(new CStateWording(string.Empty, null, true, "Card.ExpressionHint"), meaning.CCardDraftExpression);
        Assert.Equal(new CStateWording("a liquid", null, false, "Card.DefinitionHint"), meaning.CCardDraftMeaning);
        Assert.Equal(
            new CStateWording(string.Empty, "Display.Unknown", false, "Display.Unknown"),
            Assert.Single(meaning.CCardDraftRegister).CRegisterDraftName);
        CCardDraft collocation = Assert.Single(shaped.CEntryDraftCollocations);
        Assert.Equal(new CStateWording(string.Empty, null, true, null), collocation.CCardDraftTitle);
        Assert.Equal(new CStateWording(string.Empty, null, true, "Card.MeaningHint"), collocation.CCardDraftMeaning);
    }

    [Fact]
    public void CardEntryRead_SentenceWithoutExample_WordsTextParticleAndDependence()
    {
        LCardDraft card = TInterface.TCardCreate("a liquid", 1) with
        {
            LCardDraftSentence =
            [
                TInterfaceExample.TSentenceDraftCreate(LStateValue.LStateValueUnknown, null),
                TInterfaceExample.TSentenceDraftCreate("the cat sat"),
            ],
        };
        LEntryDraft draft = TInterface.TEntryDraftCreate("water", "English", string.Empty, string.Empty, [card], []);

        CEntryDraft shaped = TInterfaceConductCard.TCardEntryRead(
            draft, new Dictionary<long, IReadOnlyList<LTranslationTarget>> { [0] = [] });

        IReadOnlyList<CSentenceDraft> rows = Assert.Single(shaped.CEntryDraftMeanings).CCardDraftSentence;
        Assert.Equal(new CStateWording(string.Empty, null, true, "Card.ExampleHint"), rows[0].CSentenceDraftText);
        Assert.Equal(
            new CStateWording(string.Empty, "Display.Unknown", false, "Display.Unknown"),
            rows[0].CSentenceDraftParticle);
        Assert.Equal(
            new CStateWording(string.Empty, null, true, "Card.DependenceHint"), rows[0].CSentenceDraftDependence);
        Assert.Equal(new CStateWording("the cat sat", null, false, "Card.ExampleHint"), rows[1].CSentenceDraftText);
    }

    [Fact]
    public void DraftWording_CitedAndUncitedRows_MarksOnlyTheCitedRowCited()
    {
        LCardDraft card = TInterface.TCardCreate("a liquid", 1) with
        {
            LCardDraftSentence =
            [
                TInterfaceExample.TSentenceDraftCreate(
                    TInterfaceState.TStateValueCreate("Smith 1990"), 0, TInterface.TStateAnchorCreate(7)),
                TInterfaceExample.TSentenceDraftCreate("the cat sat"),
                TInterfaceExample.TSentenceDraftCreate(null, null),
            ],
        };
        LEntryDraft draft = TInterface.TEntryDraftCreate("water", "English", string.Empty, string.Empty, [card], []);

        CEntryDraft shaped = TInterfaceConductCard.TCardEntryRead(
            draft, new Dictionary<long, IReadOnlyList<LTranslationTarget>> { [0] = [] });

        IReadOnlyList<CSentenceDraft> rows = Assert.Single(shaped.CEntryDraftMeanings).CCardDraftSentence;
        Assert.True(rows[0].CSentenceDraftCited);
        Assert.False(rows[1].CSentenceDraftCited);
        Assert.False(rows[2].CSentenceDraftCited);
    }

    [Fact]
    public void DraftWording_GlossImageAndSituation_WordsEachValueForItsRow()
    {
        CStateValue unknown = new(string.Empty, true);
        CStateValue written = new("le chat", false);

        Assert.Equal(
            new CStateWording("le chat", null, false, "Example.Translation"),
            new CGlossDraft(1, "French", written, true).CGlossDraftWording);
        Assert.Equal(
            new CStateWording(string.Empty, null, true, "Example.Translation"),
            new CGlossDraft(1, "French", CStateValue.CStateValueEmpty, true).CGlossDraftWording);
        Assert.Equal(
            new CStateWording(string.Empty, "Display.Unknown", false, "Display.Unknown"),
            new CImageDraft(2, unknown, false, null).CImageDraftWording);
        Assert.Equal(
            new CStateWording(string.Empty, null, true, "Card.LocationHint"),
            new CImageDraft(2, CStateValue.CStateValueEmpty, true, null).CImageDraftWording);
        Assert.Equal(
            new CStateWording(string.Empty, "Display.Unknown", false, "Display.Unknown"),
            new CSituationDraft(4, unknown, written, written, [], []).CSituationDraftWording);
        Assert.Equal(
            new CStateWording("le chat", null, false, null),
            new CSituationDraft(4, written, unknown, unknown, [], []).CSituationDraftWording);
    }

    [Fact]
    public void PanelRowRead_EngineRow_CopiesEveryFieldAndTheMark()
    {
        CVistaRow row = TInterfaceConductPanel.TPanelRowRead(TInterfaceConductPanel.TVistaRowCreate(4, "gloss", true));

        Assert.Equal(new CVistaRow(4, "aqua", "Latin", "gloss", "aqua (1)", true), row);
    }

    internal static (CDesk TCardDesk, CCard TCardCard) TCardPrepare(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        CDesk desk = TInterfaceConductDesk.TDeskCreate(
            engine, "Input", TEnvoyFake.TEnvoyCreate(false, []), "Input", CSubject.CSubjectEntry);
        desk.CDeskStart(null);
        return (desk, TInterfaceConductCard.TCardCreate(engine, desk, TEnvoyFake.TEnvoyCreate(false, [])));
    }

    internal static long TCardSheetAdd(CDesk desk)
    {
        desk.TDeskDefer(TInterface.TRequestAdditionCreate(desk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        return desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
    }

    internal static IReadOnlyList<long> TCardTranslationRead(CDesk desk, long sheet)
    {
        return TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet).LCardDraftTranslation;
    }

    internal static (long TCardSheet, long TCardSentence) TCardSentenceAdd(CDesk desk)
    {
        desk.TDeskDefer(TInterface.TRequestAdditionCreate(desk.CDeskId, LCardKind.LCardKindMeaning, 0, int.MaxValue));
        long sheet = desk.TDeskRead()!.LDraftContent.LEntryDraftMeanings[^1].LCardDraftId;
        desk.TDeskDefer(TInterface.TSentenceAdditionCreate(desk.CDeskId, sheet, 0));
        LCardDraft carded = TInterface.TRequestCardFind(desk.TDeskRead()!.LDraftContent, sheet);
        return (sheet, carded.LCardDraftSentence[0].LSentenceDraftId);
    }
}
