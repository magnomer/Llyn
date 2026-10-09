using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LCardDraft TDraftCardCreate(string title)
    {
        return TInterface.TCardDraftCreate(
            TInterfaceState.TStateValueCreate(title),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            [],
            [],
            [],
            [],
            [],
            0);
    }

    internal static LEntryDraft TDraftPlainCreate(string headword)
    {
        LCardDraft meaning = TInterface.TCardDraftCreate(
            TInterfaceState.TStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            TInterfaceState.TStateValueCreate("to set something burning"),
            [],
            [],
            [],
            [],
            [],
            1);

        return TInterface.TEntryDraftCreate(headword, "English", string.Empty, string.Empty, [meaning], []);
    }

    internal static LCourt TDraftLinkCreate(long owner, long target, string headword)
    {
        return TInterface.TCourtLinkCreate(TInterface.TIdentityCreate(), owner, target, headword, "English");
    }

    internal static LDraft TDraftNestedCreate(string origin, string headword)
    {
        LSentenceDraft sentence = TInterfaceExample.TSentenceDraftCreate("she knelt to kindle the damp logs");
        LSituationDraft situation = TInterface.TSituationDraftCreate("around a hearth");
        LCardDraft meaning = TInterface.TCardDraftCreate(
            TInterfaceState.TStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            TInterfaceState.TStateValueCreate("to set something burning"),
            [
                sentence with
                {
                    LSentenceDraftId = TInterface.TIdentityCreate(),
                    LSentenceDraftExample = sentence.LSentenceDraftExample! with
                    {
                        LExampleDraftId = TInterface.TIdentityCreate(),
                    },
                },
            ],
            [situation with { LSituationDraftId = TInterface.TIdentityCreate() }],
            [],
            [],
            [],
            0,
            TInterface.TIdentityCreate()) with
        {
            LCardDraftTag =
                [TInterface.TTagDraftCreate("literal")[0] with { LTagDraftId = TInterface.TIdentityCreate() }],
        };

        LEntryDraft content = TInterface.TEntryDraftCreate(
            headword,
            "English",
            "/ˈkɪnd(ə)l/",
            "Chiefly literary.",
            [meaning],
            []);
        content = content with
        {
            LEntryDraftPronunciations =
            [
                content.LEntryDraftPronunciation! with { LPronunciationDraftId = TInterface.TIdentityCreate() },
            ],
        };

        return TInterface.TDraftCreate(
            TInterface.TIdentityCreate(),
            origin,
            0,
            content,
            DateTimeOffset.UtcNow);
    }

    internal static bool TDraftMatch(LEntryDraft one, LEntryDraft other) =>
        LDraftClerkEquality.LDraftMatch(one, other);

    internal static LEntryDraft TReflexDraftCreate(IReadOnlyList<LReflexDraft> reflexes)
    {
        return TInterface.TEntryDraftCreate(
            "弄",
            "English",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "a state", [], [], [], [], [], 1)],
            [],
            reflexes: reflexes);
    }
}
