using Llyn.Conduct;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.Tests;

internal static class TInterfaceGate
{
    internal static void TQuillAuthorSet(this CDesk desk, string name) => desk.CDeskQuill!.LQuillAuthorSet(name);

    internal static CSituationDraft? TAtlasSituationRead(LSituation? situation) =>
        LAtlas.LAtlasSituationRead(situation);

    internal static IReadOnlyList<CStem> TXieshengGroveBuild(IReadOnlyList<LStem> rows) =>
        LXiesheng.LXieshengGroveBuild(rows);

    internal static CStemPage TXieshengPageBuild(LStemPage page) => LXiesheng.LXieshengPageBuild(page);

    internal static LStem TStemCreate(long id, string language, string key, int count, bool chosen) =>
        new(id, language, key, count, chosen);

    internal static LStemPage TStemPageCreate(string language, string key, IReadOnlyList<string> characters) =>
        new(language, key, characters);

    internal static LStemPage TStemPageBlank => LStemPage.LStemPageBlank;

    internal static CDiweiPage TYunjingPageBuild(LDiweiPage page) => LYunjing.LYunjingPageBuild(page);

    internal static LDiweiPage TDiweiPageCreate(bool respelled) =>
        new(
            "Middle Chinese",
            "來",
            [
                new LDiweiSection(
                    "一",
                    [new LDiweiLine("/l/", "寒", true, 0, ["爛", "蘭"])],
                    [
                        new LTallyLine(
                            "Cantonese", "literary", [new LTallyMark("l", ["爛"])], [new LTallyMark("L", ["蘭"])]),
                    ],
                    true,
                    respelled),
            ]);

    internal static LDiweiPage TDiweiPageBlank => LDiweiPage.LDiweiPageBlank;
}
