using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtelierRespelling
{
    [Theory]
    [InlineData(false, false, "")]
    [InlineData(true, false, "")]
    [InlineData(false, true, "/")]
    [InlineData(true, true, "/")]
    public void RespellingReflexScan_Modes_SlashesOnlyPhonemic(bool respelled, bool phonemic, string slash)
    {
        LReflexPort phonology = TEngineFake.TEngineCreate<LReflexPort>(
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineGuiseRead"] = _ => new[] { TInterface.TReflexGuiseCreate(respelled, phonemic, false, "ipa") },
            });

        CReflex reflex = Assert.Single(
            TInterfaceConductSound.TRespellingReflexScan(
                phonology, "Chinese", [TAtelierReflexCreate(1, "Wu", "ipa", "")]));

        Assert.Equal(new CRespellingMark(respelled, slash, slash), reflex.CReflexMark);
    }

    [Fact]
    public void RespellingReflexScan_TwoRows_AnswersEachRowReadyToShow()
    {
        List<object?[]?> asked = [];
        LReflexPort phonology = TEngineFake.TEngineCreate<LReflexPort>(
            new Dictionary<string, Func<object?[]?, object?>>
            {
                ["LEngineGuiseRead"] = args =>
                {
                    asked.Add(args);
                    return new[]
                    {
                        TInterface.TReflexGuiseCreate(true, false, false, "respelt"),
                        TInterface.TReflexGuiseCreate(true, false, true, "ipa"),
                    };
                },
            });

        IReadOnlyList<CReflex> reflexes = TInterfaceConductSound.TRespellingReflexScan(
            phonology,
            "Chinese",
            [TAtelierReflexCreate(4, " Wu", "ipa", "respelt"), TAtelierReflexCreate(5, "Jin", "ipa", "")]);

        Assert.Equal("Chinese", asked.Single()![0]);
        Assert.Equal(
            [" Wu", "Jin"],
            ((IReadOnlyList<LReflexDraft>)asked.Single()![1]!).Select(static reflex => reflex.LReflexDraftLanguage));
        Assert.Equal([4L, 5L], reflexes.Select(static reflex => reflex.CReflexId));
        Assert.Equal(["respelt", "ipa"], reflexes.Select(static reflex => reflex.CReflexText));
        Assert.Equal([false, true], reflexes.Select(static reflex => reflex.CReflexFolded));
        Assert.Equal([true, true], reflexes.Select(static reflex => reflex.CReflexLead));
        Assert.Equal(" Wu", reflexes[0].CReflexLanguage);
        Assert.Equal([7L], reflexes[0].CReflexAnchors);
    }

    private static LReflexDraft TAtelierReflexCreate(long id, string language, string text, string respelling)
    {
        return TInterface.TReflexDraftCreate(
            language, string.Empty, text, id: id, respelling: respelling, anchors: [7]);
    }
}
